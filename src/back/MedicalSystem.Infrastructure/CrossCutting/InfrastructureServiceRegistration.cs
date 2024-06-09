using MedicalSystem.Application.Configurations;
using MedicalSystem.Application.Interfaces.Repositories;
using MedicalSystem.Application.Interfaces.Services;
using MedicalSystem.Domain.Entities;
using MedicalSystem.Infrastructure.Extensions;
using MedicalSystem.Infrastructure.Persistence;
using MedicalSystem.Infrastructure.Repositories;
using MedicalSystem.Infrastructure.Services;
using MedicalSystem.Infrastructure.TimerSchedulers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace MedicalSystem.Infrastructure.CrossCutting
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            //var sqlServer = configuration.GetSection("SqlConnection:Host").Value ?? string.Empty;
            //var sqlPort = configuration.GetSection("SqlConnection:Port").Value ?? string.Empty;
            //var sqlUser = configuration.GetSection("SqlConnection:User").Value ?? string.Empty;
            //var sqlPassword = configuration.GetSection("SqlConnection:Password").Value ?? string.Empty;
            //var sqlDatabase = configuration.GetSection("SqlConnection:DbName").Value ?? string.Empty;

            var sqlServer = Environment.GetEnvironmentVariable("DB_HOST") ?? string.Empty;
            var sqlPort = Environment.GetEnvironmentVariable("DB_PORT") ?? string.Empty;
            var sqlUser = Environment.GetEnvironmentVariable("DB_USER") ?? string.Empty;
            var sqlPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? string.Empty;
            var sqlDatabase = Environment.GetEnvironmentVariable("DB_NAME") ?? string.Empty;


            var sqlConnectionString = $"Server={sqlServer};Database={sqlDatabase};TrustServerCertificate=True; User={sqlUser};Password={sqlPassword};";

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(sqlConnectionString));

            services.AddDbContext<IdentityDataContext>(options =>
                options.UseSqlServer(sqlConnectionString));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityDataContext>()
                .AddDefaultTokenProviders();

            services.AddIdentityCore<Doctor>().AddEntityFrameworkStores<IdentityDataContext>();
            services.AddIdentityCore<Patient>().AddEntityFrameworkStores<IdentityDataContext>();

            //Register Repositories
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            services.AddScoped<IDoctorRepository, DoctorRepository>();
            services.AddScoped<IPatientRepository, PatientRepository>();

            // Register Services
            services.AddScoped<IEmailService, EmailService>();

            //Add Configurations
            services.Configure<EmailSettings>(options =>
            {
                options.FromAddress = configuration.GetSection(nameof(EmailSettings))[nameof(EmailSettings.FromAddress)];
                options.FromName = configuration.GetSection(nameof(EmailSettings))[nameof(EmailSettings.FromName)];
                options.Provider = configuration.GetSection(nameof(EmailSettings))[nameof(EmailSettings.Provider)];
                options.Password = configuration.GetSection(nameof(EmailSettings))[nameof(EmailSettings.Password)];
                options.Port = int.Parse(configuration.GetSection(nameof(EmailSettings))[nameof(EmailSettings.Port)]);
            });

            //Add CronJobs
            //# ┌───────────── minute (0–59)
            //# │ ┌───────────── hour (0–23)
            //# │ │ ┌───────────── day of the month (1–31)
            //# │ │ │ ┌───────────── month (1–12)
            //# │ │ │ │ ┌───────────── day of the week (0–6) (Sunday to Saturday;
            //# │ │ │ │ │                                   7 is also Sunday on some systems)
            //# │ │ │ │ │
            //# │ │ │ │ │
            //# * * * * * <command to execute>
            services.AddCronJob<EmailNotification>(c => c.CronExpression = @"0 */1 * * * *");

            return services;
        }

        public static IServiceCollection AddCronJob<T>(this IServiceCollection services, Action<IScheduleConfig<T>> options) where T : CronJobExtensions
        {
            var config = new ScheduleConfig<T>();
            options.Invoke(config);

            services.AddSingleton<IScheduleConfig<T>>(config);

            services.AddHostedService<T>();

            return services;
        }
    }
}
