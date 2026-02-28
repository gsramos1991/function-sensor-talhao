using AgroSolutions.Domain.Dto;
using AgroSolutions.Domain.Interfaces;
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
    public class AutenticateApiRepository : IAutenticateApi
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<AutenticateApiRepository> _logger;

        public AutenticateApiRepository(IHttpClientFactory httpClientFactory, ILogger<AutenticateApiRepository> logger)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> GetToken(string EmailUser, string Passoword)
        {
            _logger.LogInformation("Iniciando autenticação para o usuário servico: {EmailUser}", EmailUser);

            try
            {
                var httpClient = _httpClientFactory.CreateClient("ApiLogin");

                var loginRequest = new LoginRequestDto
                {
                    email = EmailUser,
                    password = Passoword
                };

                _logger.LogInformation("URL de autenticação: {BaseAddress}{Route}", httpClient.BaseAddress, "api/v1/Auth/login");
                _logger.LogInformation("Payload: Email={Email}", loginRequest.email);

                var response = await httpClient.PostAsJsonAsync("api/v1/Auth/login", loginRequest);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Falha na autenticação para o usuário: {Email}. Status: {StatusCode}, Erro: {ErrorContent}", 
                        EmailUser, response.StatusCode, errorContent);
                    return null;
                }

                var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();

                if (loginResponse == null || string.IsNullOrEmpty(loginResponse.Token))
                {
                    _logger.LogWarning("Resposta de autenticação inválida para o usuário: {Email}", EmailUser);
                    return null;
                }

                _logger.LogInformation("Token obtido com sucesso para o usuário: {Email}", EmailUser);
                return loginResponse.Token;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Erro de rede ao tentar autenticar o usuário: {Email}", EmailUser);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao tentar autenticar o usuário: {Email}", EmailUser);
                return null;
            }
        }
    }
}
