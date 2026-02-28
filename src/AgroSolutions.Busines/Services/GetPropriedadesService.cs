using AgroSolutions.Busines.Interface;
using AgroSolutions.Domain.Dto;
using AgroSolutions.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgroSolutions.Busines.Services
{
    public class GetPropriedadesService : IGetPropriedadesService
    {
        private readonly IGetPropriedadesRepository _repository;
        private readonly ILogger<GetPropriedadesService> _logger;

        public GetPropriedadesService(IGetPropriedadesRepository repository, ILogger<GetPropriedadesService> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<PropriedadeDto>> GetPropriedades(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Token não fornecido para obter propriedades");
                return new List<PropriedadeDto>();
            }

            _logger.LogInformation("Iniciando request para obter propriedades");

            try
            {
                var propriedades = await _repository.GetPropriedades(token);

                if (propriedades == null || propriedades.Count == 0)
                {
                    _logger.LogWarning("Nenhuma propriedade encontrada");
                    return new List<PropriedadeDto>();
                }

                _logger.LogInformation("Total de propriedades recuperadas: {Count}", propriedades.Count);
                return propriedades;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao recuperar propriedades");
                return new List<PropriedadeDto>();
            }
        }
    }
}
