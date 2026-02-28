using AgroSolutions.Infra.Ioc;
using Azure.Identity;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
var builder = FunctionsApplication.CreateBuilder(args);


Serilog.Debugging.SelfLog.Enable(Console.Error);

var keyVaultUrl = builder.Configuration["KeyVault:Url"];

if (!string.IsNullOrEmpty(keyVaultUrl))
{
    builder.Configuration.AddAzureKeyVault(
        new Uri(keyVaultUrl),
        new DefaultAzureCredential());
}

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights()
    .AddRegisterRepositories()
    .AddRegisterServices()
    .AddSerilogApp(builder.Configuration)
    .AddAzureSecrets(builder.Configuration)
    .AddServiceBus(builder.Configuration)
    .AddHttpClientApp(builder.Configuration)
    .AddOpenTelemetryApp(builder.Configuration);

builder.Services.AddLogging(lb => lb.AddSerilog(dispose: true));


var host = builder.Build();

try
{
    Log.Information("AgroSolutions Function Host iniciando...");
    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Erro fatal no Host da Function AgroSolutions");
}
finally
{
    await Log.CloseAndFlushAsync();
}