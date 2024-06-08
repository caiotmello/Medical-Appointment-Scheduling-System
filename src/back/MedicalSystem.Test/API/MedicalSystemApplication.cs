using MedicalSystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace MedicalSystem.Test.API
{
    public class MedicalSystemApplication : WebApplicationFactory<Program>
    {
        public string DefaultUserId { get; set; } = "1";
        protected override IHost CreateHost(IHostBuilder builder)
        {
            var root = new InMemoryDatabaseRoot();

            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
                services.RemoveAll(typeof(DbContextOptions<IdentityDataContext>));


                services.AddDbContext<AppDbContext>(options =>
                  options.UseInMemoryDatabase("MedicalSystemDb", root));

                //COnfigurations to bypass Authentication and Authorization
                //services.Configure<TestAuthHandlerOptions>(options => options.DefaultUserId = DefaultUserId);

                services.AddDbContext<IdentityDataContext>(options =>
                    options.UseInMemoryDatabase("MedicalSystemDb", root));

            });

            return base.CreateHost(builder);
        }
    }
}
