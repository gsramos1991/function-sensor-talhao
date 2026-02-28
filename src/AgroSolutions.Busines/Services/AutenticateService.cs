using AgroSolutions.Busines.Interface;
using AgroSolutions.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroSolutions.Busines.Services
{
    public class AutenticateService : IAutenticateService
    {
        private readonly ILogger<AutenticateService> _logger;
        private readonly IAutenticateApi _autenticateApi;

        public AutenticateService(ILogger<AutenticateService> logger, IAutenticateApi autenticateApi)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _autenticateApi = autenticateApi ?? throw new ArgumentNullException(nameof(autenticateApi));
        }

        public async Task<string> Autenticar(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                _logger.LogWarning("Tentativa de autenticação com username vazio");
                return null;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                _logger.LogWarning("Tentativa de autenticação com password vazio para o usuário: {Email}", username);
                return null;
            }

            _logger.LogInformation("Autenticando usuário: {Email}", username);
            var token = await _autenticateApi.GetToken(username, password);

            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Falha na autenticação do usuário: {Email}", username);
                return null;
            }

            _logger.LogInformation("Usuário autenticado com sucesso: {Email}", username);
            return token;
        }
    }
}
