using AgroSolutions.Domain.Dto;
using AgroSolutions.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace AgroSolutions.Infra.Data.Repositories
{
    public class SendDataSensorRepository : ISendDataSensorRepository
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<SendDataSensorRepository> _logger;
        private readonly IConfiguration _config;
        public SendDataSensorRepository(IHttpClientFactory httpClientFactory, ILogger<SendDataSensorRepository> logger, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _config = configuration;
        }
        
        public async Task<bool> SendDadosTalhao(SendDataSensorDto sendDataSensor)
        {
            _logger.LogInformation("Enviando dados do sensor para o talhão: {TalhaoId}", sendDataSensor.TalhaoId);

            try
            {
                var httpClient = _httpClientFactory.CreateClient("ApiGravaTalhao");
                
                var response = await httpClient.PostAsJsonAsync("api/RegistraDados", sendDataSensor);

                response.EnsureSuccessStatusCode();

                _logger.LogInformation("Dados do sensor enviados com sucesso para o talhão: {TalhaoId}", sendDataSensor.TalhaoId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao enviar dados do sensor para o talhão: {TalhaoId}", sendDataSensor.TalhaoId);
                return false;
            }
        }
    }
}
