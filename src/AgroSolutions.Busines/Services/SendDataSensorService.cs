using AgroSolutions.Busines.Interface;
using AgroSolutions.Busines.Model;
using AgroSolutions.Domain.Dto;
using AgroSolutions.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace AgroSolutions.Busines.Services
{
    public class SendDataSensorService : ISendDataSensorService
    {
        private readonly ILogger<SendDataSensorService> _logger;
        private readonly ISendDataSensorRepository _repository;

        public SendDataSensorService(ILogger<SendDataSensorService> logger, ISendDataSensorRepository repository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<bool> SendDadosTalhao(SendDataSensor sendDataSensor)
        {
            if (sendDataSensor == null)
            {
                _logger.LogWarning("SendDataSensor é nulo");
                return false;
            }

            var dto = ConvertModelToDto(sendDataSensor);
            var result = await _repository.SendDadosTalhao(dto);

            if (result)
            {
                _logger.LogInformation("Dados enviados com sucesso para o talhão: {TalhaoId}", sendDataSensor.TalhaoId);
            }
            else
            {
                _logger.LogWarning("Falha ao enviar dados para o talhão: {TalhaoId}", sendDataSensor.TalhaoId);
            }

            return result;
        }

        private SendDataSensorDto ConvertModelToDto(SendDataSensor model)
        {
            return new SendDataSensorDto
            {
                TalhaoId = model.TalhaoId,
                Umidade = model.Umidade,
                DataAfericao = model.DataAfericao,
                Temperatura = model.Temperatura,
                IndiceUv = model.IndiceUv,
                VelocidadeVento = model.VelocidadeVento
            };
        }
    }
}
