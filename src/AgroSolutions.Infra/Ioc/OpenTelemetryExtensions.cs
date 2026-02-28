using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;
using System.Collections.Generic;

namespace AgroSolutions.Infra.Ioc
{
    public static class OpenTelemetryExtensions
    {
        public static IServiceCollection AddOpenTelemetryApp(this IServiceCollection services, IConfiguration configuration)
        {
            var otlpEndpoint    = configuration["OpenTelemetry:OtlpEndpoint"]   ?? "http://localhost:4317";
            // Cada aplicação define seu próprio service.name via configuração.
            // Fallback mantido apenas para esta Function; novas apps DEVEM sobrescrever nas suas
            // variáveis de ambiente: OpenTelemetry__ServiceName=<nome-do-servico>
            var serviceName     = configuration["OpenTelemetry:ServiceName"]    ?? "AzFunctions.RecebeDadosSensor";
            var serviceVersion  = configuration["OpenTelemetry:ServiceVersion"] ?? "1.0.0";
            // job agrupa todas as apps do ecossistema AgroSolutions nos datasources do Grafana.
            // Pode ser sobrescrito por app via OpenTelemetry__JobName se necessário.
            var jobName         = configuration["OpenTelemetry:JobName"]        ?? "agrosolutions";
            var environment     = configuration["ASPNETCORE_ENVIRONMENT"]       ?? "Production";

            var resource = ResourceBuilder.CreateDefault()
                .AddService(serviceName: serviceName, serviceVersion: serviceVersion)
                .AddAttributes(new Dictionary<string, object>
                {
                    ["environment"]            = environment,
                    ["deployment.environment"] = environment,
                    // "job" é preservado pelo OTel Collector (action: insert) e vira
                    // label no Prometheus e no Loki. Todas as apps AgroSolutions usam "agrosolutions".
                    ["job"] = jobName
                });

            services.AddOpenTelemetry()
                .WithTracing(builder =>
                {
                    builder
                        .SetResourceBuilder(resource)
                        .AddSource(serviceName)
                        
                        .AddHttpClientInstrumentation()
                        .AddOtlpExporter(options =>
                        {
                            options.Endpoint = new Uri(otlpEndpoint);
                            options.Protocol = OtlpExportProtocol.Grpc;
                        });
                })
                // NOTA: WithLogging() removido intencionalmente.
                // Os logs são encaminhados ao OTel Collector exclusivamente via
                // Serilog.Sinks.OpenTelemetry (configurado em SerilogExtensions).
                // Manter os dois caminhos causaria duplicação de logs no Loki.
                .WithMetrics(builder =>
                {
                    builder
                        .SetResourceBuilder(resource)
                        .AddMeter(serviceName)
                        .AddRuntimeInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddOtlpExporter(options =>
                        {
                            options.Endpoint = new Uri(otlpEndpoint);
                            options.Protocol = OtlpExportProtocol.Grpc;
                        });
                });

            return services;
        }
    }
}
