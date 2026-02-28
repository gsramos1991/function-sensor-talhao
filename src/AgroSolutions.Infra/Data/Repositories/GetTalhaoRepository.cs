using AgroSolutions.Domain.Dto;
using AgroSolutions.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace AgroSolutions.Infra.Data.Repositories
{
    public class GetTalhaoRepository : IGetTalhaoRepository
    {
        private readonly IHttpClientFactory _httpClientFactory; 
        private readonly ILogger<GetTalhaoRepository> _logger;  
        
        public GetTalhaoRepository(IHttpClientFactory httpClientFactory, ILogger<GetTalhaoRepository> logger)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<TalhaoDto>> GetTalhoesByPropriedade(Guid propriedadeId, string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Token não fornecido para requisição de talhões");
                return new List<TalhaoDto>();
            }

            _logger.LogInformation("Obtendo talhões da propriedade: {PropriedadeId}...", propriedadeId);
            
            try
            {
                var httpClient = _httpClientFactory.CreateClient("ApiTalhoes");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                
                var response = await httpClient.GetAsync($"api/cadastro-agricola/propriedades/{propriedadeId.ToString().ToUpper()}/talhoes?incluirExcluidos=false");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Falha ao obter talhões da propriedade {PropriedadeId}. Status: {StatusCode}", propriedadeId, response.StatusCode);
                    return new List<TalhaoDto>();
                }

                var content = await response.Content.ReadFromJsonAsync<List<TalhaoDto>>();

                if (content == null || content.Count == 0)
                {
                    _logger.LogWarning("Nenhum talhão foi encontrado para a propriedade {PropriedadeId}", propriedadeId);
                    return new List<TalhaoDto>();
                }

                _logger.LogInformation("Quantidade de talhões localizados para a propriedade {PropriedadeId}: {Count}", propriedadeId, content.Count);
                return content;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Erro de rede ao buscar talhões da propriedade {PropriedadeId}", propriedadeId);
                return new List<TalhaoDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao buscar talhões da propriedade {PropriedadeId}", propriedadeId);
                return new List<TalhaoDto>();
            }
        }
    }
}
