using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Sinks.OpenTelemetry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroSolutions.Infra.Ioc
{
    public static class SerilogExtensions
    {
        public static IServiceCollection AddSerilogApp(this IServiceCollection services, IConfiguration configuration)
        {
            try
            {
                var environment    = configuration["ASPNETCORE_ENVIRONMENT"]       ?? "Production";
                var otlpEndpoint   = configuration["OpenTelemetry:OtlpEndpoint"] ?? "http://localhost:4317";
                // Mesmo par de chaves usado pelo OpenTelemetryExtensions — garante coerência.
                var serviceName    = configuration["OpenTelemetry:ServiceName"]    ?? "AzFunctions.RecebeDadosSensor";
                var serviceVersion = configuration["OpenTelemetry:ServiceVersion"] ?? "1.0.0";
                var jobName        = configuration["OpenTelemetry:JobName"]        ?? "agrosolutions";

                var loggerConfiguration = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .Enrich.WithProperty("Application", "AgroSolutions")
                    .Enrich.WithCorrelationId()
                    .Enrich.WithCorrelationIdHeader()
                    .Enrich.WithProperty("Enviroment", environment)
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .WriteTo.OpenTelemetry(options =>
                    {
                        options.Endpoint = otlpEndpoint;
                        options.Protocol = OtlpProtocol.Grpc;
                        options.ResourceAttributes = new Dictionary<string, object>
                        {
                            // service.name DEVE ser idêntico ao configurado no OpenTelemetryExtensions
                            // para que traces e logs aparecem sob o mesmo serviço no Grafana
                            ["service.name"]           = serviceName,
                            ["service.version"]        = serviceVersion,
                            ["deployment.environment"] = environment,
                            // label mapeada como job="agrosolutions" no Loki pelo OTel Collector
                            ["job"] = jobName
                        };
                    });

                if (environment != "Development")
                {
                    loggerConfiguration.MinimumLevel.Information();
                }


                Log.Logger = loggerConfiguration.CreateLogger();

                return services;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Aviso: Falha ao configurar OpenTelemetry: {ex.Message}");
            }
            return services;

        }
    }
}
