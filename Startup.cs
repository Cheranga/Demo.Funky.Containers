using System;
using Azure.Identity;
using FluentValidation;
using FunkyContainers;
using FunkyContainers.Core;
using FunkyContainers.Features;
using FunkyContainers.Features.ConfirmReservation;
using FunkyContainers.Features.ReserveHotel;
using FunkyContainers.Infrastructure.CustomerApi;
using FunkyContainers.Infrastructure.DataAccess;
using FunkyContainers.Infrastructure.Email;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

[assembly: FunctionsStartup(typeof(Startup))]

namespace FunkyContainers
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var configuration = GetConfiguration(builder);
            var services = builder.Services;

            RegisterValidators(services);
            RegisterServices(services);
            RegisterAuthClients(services,configuration);
            RegisterConfigurations(services,configuration);
        }
        
        private void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<ICommandHandler<SaveReservationCommand>, SaveReservationCommandCommandHandler>();
            services.AddSingleton<IConfirmReservationService, ConfirmReservationService>();
            services.AddHttpClient<ICustomerApiService, CustomerApiService>();
        }
        
        private void RegisterAuthClients(IServiceCollection services, IConfigurationRoot configuration)
        {
            services.AddAzureClients(builder =>
            {
                // builder.AddQueueServiceClient(configuration.GetSection("QueueSource")).WithCredential(new DefaultAzureCredential(new DefaultAzureCredentialOptions
                // {
                //     ExcludeEnvironmentCredential = true,
                //     ExcludeAzurePowerShellCredential = true,
                //     ExcludeInteractiveBrowserCredential = true,
                //     ExcludeVisualStudioCredential = false,
                //     ExcludeManagedIdentityCredential = false
                // }));

                var config = new TableConfig();
                configuration.GetSection(nameof(TableConfig)).Bind(config);


                builder.AddTableServiceClient(config.ConnectionString);
                // builder.AddTableServiceClient(config.ConnectionString).WithCredential(new DefaultAzureCredential(new DefaultAzureCredentialOptions
                // {
                //     ExcludeEnvironmentCredential = true,
                //     ExcludeAzurePowerShellCredential = true,
                //     ExcludeInteractiveBrowserCredential = true,
                //     ExcludeVisualStudioCredential = false,
                //     ExcludeManagedIdentityCredential = false
                // }));
            });
        }
        
        private void RegisterConfigurations(IServiceCollection services, IConfigurationRoot configuration)
        {
            services.Configure<EmailConfig>(configuration.GetSection(nameof(EmailConfig)));
            services.Configure<TableConfig>(configuration.GetSection(nameof(TableConfig)));

            services.AddSingleton(provider =>
            {
                var config = provider.GetRequiredService<IOptionsSnapshot<EmailConfig>>().Value;
                return config;
            });

            services.AddSingleton(provider =>
            {
                var config = provider.GetRequiredService<IOptionsSnapshot<TableConfig>>().Value;
                return config;
            });
        }

        private void RegisterValidators(IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(typeof(ModelValidatorBase<>).Assembly);
        }

        protected virtual IConfigurationRoot GetConfiguration(IFunctionsHostBuilder builder)
        {
            var services = builder.Services;
            var executionContextOptions = services.BuildServiceProvider().GetService<IOptions<ExecutionContextOptions>>().Value;

            var configuration = new ConfigurationBuilder()
                .SetBasePath(executionContextOptions.AppDirectory)
                .AddJsonFile("local.settings.json", true, true)
                .AddEnvironmentVariables()
                .Build();

            return configuration;
        }
    }
}