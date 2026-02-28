using AgroSolutions.Domain.Dto;
using AgroSolutions.Domain.Interfaces;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AgroSolutions.Infra.Data.Repositories
{
    public class SendDataSensorServiceBusRepository : ISendDataSensorRepository
    {
        // Altere aqui para redirecionar para outra fila durante testes/debug
        private const string QueueName = "salvar-dados-sensor";

        private readonly ServiceBusClient _serviceBusClient;
        private readonly ILogger<SendDataSensorServiceBusRepository> _logger;

        public SendDataSensorServiceBusRepository(
            ServiceBusClient serviceBusClient,
            ILogger<SendDataSensorServiceBusRepository> logger)
        {
            _serviceBusClient = serviceBusClient ?? throw new ArgumentNullException(nameof(serviceBusClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> SendDadosTalhao(SendDataSensorDto sendDataSensor)
        {
            _logger.LogInformation(
                "Preparando envio para fila '{QueueName}' | TalhaoId={TalhaoId} | DataAfericao={DataAfericao}",
                QueueName, sendDataSensor.TalhaoId, sendDataSensor.DataAfericao);

            try
            {
                var payload = JsonSerializer.Serialize(sendDataSensor, new JsonSerializerOptions
                {
                    WriteIndented = false
                });

                // Nível Debug: exibe o payload completo — útil durante debug local.
                // Para ativar: altere "default" para "Debug" no host.json > logging > logLevel.
                _logger.LogDebug(
                    "Payload da mensagem para fila '{QueueName}': {Payload}",
                    QueueName, payload);

                var message = new ServiceBusMessage(payload)
                {
                    ContentType = "application/json",

                    // CorrelationId permite rastrear a mensagem por TalhaoId no Service Bus Explorer e App Insights
                    CorrelationId = sendDataSensor.TalhaoId.ToString(),

                    // ApplicationProperties permitem filtrar mensagens sem precisar abrir o body
                    ApplicationProperties =
                    {
                        ["TalhaoId"]      = sendDataSensor.TalhaoId.ToString(),
                        ["DataAfericao"]  = sendDataSensor.DataAfericao.ToString("O")
                    }
                };

                // await using garante que o sender é fechado/descartado corretamente após o envio.
                // O ServiceBusClient (Singleton) mantém a conexão subjacente aberta.
                await using var sender = _serviceBusClient.CreateSender(QueueName);
                await sender.SendMessageAsync(message);

                _logger.LogInformation(
                    "✓ Mensagem enviada para fila '{QueueName}' | TalhaoId={TalhaoId} | MessageId={MessageId}",
                    QueueName, sendDataSensor.TalhaoId, message.MessageId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "✗ Falha ao enviar mensagem para fila '{QueueName}' | TalhaoId={TalhaoId}",
                    QueueName, sendDataSensor.TalhaoId);
                return false;
            }
        }
    }
}
