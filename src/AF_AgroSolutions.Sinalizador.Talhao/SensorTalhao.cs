using AgroSolutions.Busines.Interface;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AF_AgroSolutions.Sinalizador.Talhao;

public class SensorTalhao
{
    private readonly ILogger<SensorTalhao> _logger;
    private readonly IProcessarService _processarService;
    private readonly IConfiguration _config;

    public SensorTalhao(ILogger<SensorTalhao> logger, IConfiguration config, IProcessarService processarService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _processarService = processarService ?? throw new ArgumentNullException(nameof(processarService));
    }

    [Function("SensorTalhao")]
    public async Task Run([TimerTrigger("%SensorTalhaoSchedule%")] TimerInfo myTimer)
    {
        _logger.LogInformation("========== INICIANDO AZURE FUNCTION - SENSOR TALHAO ==========");
        _logger.LogInformation("Timestamp: {Timestamp}", DateTime.UtcNow);

        if (myTimer.ScheduleStatus is not null)
        {
            _logger.LogInformation("Ultima execucao: {Last}", myTimer.ScheduleStatus.Last);
            _logger.LogInformation("Proxima execucao: {Next}", myTimer.ScheduleStatus.Next);
        }

        try
        {
            var username = _config["EmailService"];
            var password = _config["PasswordService"];

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                _logger.LogError("Credenciais nao configuradas. Verifique UserService e PasswordService no appsettings.");
                return;
            }

            // Processar todos os talhoes (Login -> Obter Talhoes -> Obter Tempo -> Enviar Dados)
            await _processarService.ProcessarDadosTalhoes(username, password);

            _logger.LogInformation("========== AZURE FUNCTION FINALIZADA COM SUCESSO ==========");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro critico na execucao da Azure Function");
            throw;
        }
    }
}