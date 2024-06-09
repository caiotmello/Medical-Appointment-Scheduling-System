using MedicalSystem.Api.Extensions;
using MedicalSystem.Application.CrossCutting;
using MedicalSystem.Application.Interfaces.Services;
using MedicalSystem.Infrastructure.CrossCutting;
using MedicalSystem.Infrastructure.Persistence;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "MedicalSystem API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme.
                      Enter 'Bearer' [space] and then your token in the test input below.
                      Example: 'Bearer 12331lkdfsdk'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });

});

// Project specific services
var configuration = builder.Configuration;
builder.Services.AddInfrastructureServices(configuration);
builder.Services.AddApplicationServices(configuration);


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

//Add migration for identity
app.MigrateDatabase<IdentityDataContext>((context, services) =>
{
    var logger = services.GetService<ILogger<IdentityDataContextSeed>>();
    var userservice = services.GetService<IUserService>();
    IdentityDataContextSeed
        .SeedAsync(context, logger, userservice)
        .Wait();
});

//Add migration for AppDbContext
app.MigrateDatabase<AppDbContext>((context, services) =>
{
    var logger = services.GetService<ILogger<AppDbContextSeed>>();
    AppDbContextSeed
        .SeedAsync(context, logger)
        .Wait();
});

app.Run();

