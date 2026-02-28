using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AgroSolutions.Infra.Ioc
{
    public static class RegisterServiceBusExtensions
    {
        public static IServiceCollection AddServiceBus(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration["ConnectionStrings:AzureServiceBus"];

            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException(
                    "Connection string 'ConnectionStrings:AzureServiceBus' não encontrada. " +
                    "Verifique o Key Vault ou o local.settings.json.");

            // ServiceBusClient deve ser Singleton: thread-safe e gerencia o pool de conexões internamente.
            // Referência: https://learn.microsoft.com/azure/service-bus-messaging/service-bus-performance-improvements
            services.AddSingleton(new ServiceBusClient(connectionString));

            return services;
        }
    }
}
