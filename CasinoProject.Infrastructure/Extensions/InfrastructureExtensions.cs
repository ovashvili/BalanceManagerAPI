using Balances;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MediatR.Extensions.FluentValidation.AspNetCore;
using System;
using CasinoProject.Application.BalanceManager.Queries.GetCasinoBalance;
using CasinoProject.Infrastructure.Enumerations.BalanceManager;
using CasinoProject.Application.BalanceManager.Contracts;
using CasinoProject.Infrastructure.Services.BalanceManager;
using Newtonsoft.Json.Converters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using CasinoProject.Infrastructure.Options;
using CasinoProject.Infrastructure.Configurations.Swagger;
using System.IO;
using MediatR;
using FluentValidation;
using System.Linq;
using CasinoProject.Infrastructure.Middlewares;

namespace CasinoProject.Infrastructure.Extensions
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection InstallInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services), "IServiceCollection is null");
            }

            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration), "IConfiguration is null");
            }

            _ = services.AddControllers().AddNewtonsoftJson(options =>
                options.SerializerSettings.Converters.Add(new StringEnumConverter()));

            _ = services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            });

            _ = services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VV";
                options.SubstituteApiVersionInUrl = true;
            });

            _ = services.AddValidatorsFromAssembly(typeof(GetCasinoBalanceQuery).Assembly);

            _ = services.AddScoped<IBalanceManagerService, BalanceManagerService>();
            
            _ = services.AddScoped<CasinoBalanceManager>();

            _ = services.AddScoped<GameBalanceManager>();

            _ = services.AddScoped<Func<BalanceManagerType, IBalanceManager>>(serviceProvider => key =>
            {
                switch (key)
                {

                    case BalanceManagerType.Casino:
                        return serviceProvider.GetService<CasinoBalanceManager>();
                    case BalanceManagerType.Game:
                        return serviceProvider.GetService<GameBalanceManager>();
                    default:
                        return null;
                }
            });

            _ = services.AddTransient<ExceptionHandlingMiddleware>();

            _ = services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            _ = services.AddMediatR(typeof(GetCasinoBalanceQuery).Assembly);

            _ = services.Configure<RetryCountOption>(options => configuration.GetSection("RetryCount").Bind(options));

            _ = services.AddHealthChecks();

            _ = services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            _ = services.AddSwaggerGen(options =>
            {
                options.OperationFilter<SwaggerDefaultValues>();
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{AppDomain.CurrentDomain.GetAssemblies().First(x => x.FullName.Contains("CasinoProject,")).GetName().Name}.xml"));
                options.UseInlineDefinitionsForEnums();
            });

            _ = services.AddSwaggerGenNewtonsoftSupport();

            return services;
        }
    }
}
