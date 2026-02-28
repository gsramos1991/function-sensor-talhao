using AgroSolutions.Domain.Dto;
using AgroSolutions.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace AgroSolutions.Infra.Data.Repositories
{
    public class GetTempoRepository : IGetTempoRepository
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<GetTempoRepository> _logger;
        private readonly IConfiguration _config;

        public GetTempoRepository(IHttpClientFactory httpClient, ILogger<GetTempoRepository> logger, IConfiguration config)
        {
            _httpClientFactory = httpClient;
            _logger = logger;
            _config = config;
        }
        public async Task<GetTempoDto> RequestDadosTempo(string city)
        {
            _logger.LogInformation("Sensor carregando informacoes do tempo no talhao localizado: {city}", city);

            try
            {
                if(string.IsNullOrEmpty(city))
                {
                    _logger.LogWarning("Cidade nula ou vazia");
                    return new GetTempoDto();
                }

                var ApiKey = _config["PrevisaoTempo:ApiKey"];
                var httpClient = _httpClientFactory.CreateClient("ApiTempo");
                var response = await httpClient.PostAsync($"?Key={ApiKey}&q={city}&days=1&aqi=no&alerts=no", null);

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadFromJsonAsync<GetTempoDto>();

                if (content == null)
                {
                    _logger.LogWarning("Nenhum registro foi encontrado");
                    return new GetTempoDto();
                }


                return content;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha no request da api ");
                throw;
            }

        }
    }
}
