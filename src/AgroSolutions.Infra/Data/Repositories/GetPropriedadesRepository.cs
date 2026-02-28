using AgroSolutions.Domain.Dto;
using AgroSolutions.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace AgroSolutions.Infra.Data.Repositories
{
    public class GetPropriedadesRepository : IGetPropriedadesRepository
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<GetPropriedadesRepository> _logger;

        public GetPropriedadesRepository(IHttpClientFactory httpClientFactory, ILogger<GetPropriedadesRepository> logger)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<PropriedadeDto>> GetPropriedades(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Token não fornecido para requisição de propriedades");
                return new List<PropriedadeDto>();
            }

            _logger.LogInformation("Obtendo propriedades...");

            try
            {
                _logger.LogInformation("Token recebido: {TokenLength} caracteres", token.Length);
                _logger.LogInformation("{token}", token);
                var httpClient = _httpClientFactory.CreateClient("ApiTalhoes");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                _logger.LogInformation("Requisição HTTP preparada para obter propriedades");
                _logger.LogInformation("Enviando requisição para endpoint: {Endpoint}", "api/cadastro-agricola/propriedades?incluirExcluidos=false");
                var response = await httpClient.GetAsync("api/cadastro-agricola/propriedades?incluirExcluidos=false");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Resposta HTTP não bem-sucedida. Status: {Content}", response.Content);
                    _logger.LogWarning("Falha ao obter propriedades. Status: {StatusCode}", response.StatusCode);
                    return new List<PropriedadeDto>();
                }

                var content = await response.Content.ReadFromJsonAsync<List<PropriedadeDto>>();

                if (content == null || content.Count == 0)
                {
                    _logger.LogWarning("Nenhuma propriedade foi encontrada");
                    return new List<PropriedadeDto>();
                }

                _logger.LogInformation("Quantidade de propriedades localizadas: {Count}", content.Count);
                return content;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Erro de rede ao buscar propriedades");
                return new List<PropriedadeDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao buscar propriedades");
                return new List<PropriedadeDto>();
            }
        }
    }
}
