using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AgroSolutions.Infra.Ioc
{
    public static class HttpClientExtensions
    {
        public static IServiceCollection AddHttpClientApp(this IServiceCollection services, IConfiguration configuration)
        {

            var baseApiLogin = configuration["HttpClients:ApiLogin"];
            var baseApiPropriedades = configuration["HttpClients:ApiPropriedadesTalhoes"];
            var baseApiTempo = configuration["HttpClients:ApiTempo"];
            var baseApiTelemetry = configuration["HttpClients:ApiGravaTalhao"];
            var baseApiTalhoes = configuration["HttpClients:ApiPropriedadesTalhoes"];


            if (string.IsNullOrEmpty(baseApiTempo))
                throw new ArgumentNullException("HttpClients:ApiTempo", "Base address for ApiTempo HTTP client is not configured.");
            if (string.IsNullOrEmpty(baseApiTelemetry))
                throw new ArgumentNullException("HttpClients:ApiGravaTalhao", "Base address for ApiGravaTalhao HTTP client is not configured.");
            if (string.IsNullOrEmpty(baseApiPropriedades))
                throw new ArgumentNullException("HttpClients:baseApiPropriedades", "Base address for ApiPropriedadesTalhoes HTTP client is not configured.");
            if (string.IsNullOrEmpty(baseApiTalhoes))
                throw new ArgumentNullException("HttpClients:ApiPropriedadesTalhoes", "Base address for ApiPropriedadesTalhoes HTTP client is not configured.");
            if (string.IsNullOrEmpty(baseApiLogin))
                throw new ArgumentNullException("HttpClients:baseApiLogin", "Base address for baseApiLogin HTTP client is not configured.");


            services.AddHttpClient("ApiLogin", client =>
            {
                client.BaseAddress = new Uri(baseApiLogin);
            });

            services.AddHttpClient("ApiPropriedades", client =>
            {
                client.BaseAddress = new Uri(baseApiPropriedades);
            });
            
            services.AddHttpClient("ApiTalhoes", client =>
            {
                client.BaseAddress = new Uri(baseApiTalhoes);
            });

            services.AddHttpClient("ApiTempo", client =>
            {
                client.BaseAddress = new Uri(baseApiTempo);
            });

            
            
            services.AddHttpClient("ApiGravaTalhao", client =>
            {
                client.BaseAddress = new Uri(baseApiTelemetry);
            });


            return services;
        }
    }
}
