using AgroSolutions.Busines.Interface;
using AgroSolutions.Busines.Model;
using AgroSolutions.Domain.Dto;
using AgroSolutions.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgroSolutions.Busines.Services
{
    public class GetTalhaoService : IGetTalhaoService
    {
        private readonly IGetTalhaoRepository _repository;
        private readonly ILogger<GetTalhaoService> _logger;

        public GetTalhaoService(IGetTalhaoRepository repository, ILogger<GetTalhaoService> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<TalhaoDto>> GetTalhoesByPropriedade(Guid propriedadeId, string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Token não fornecido para obter talhões");
                return new List<TalhaoDto>();
            }

            _logger.LogInformation("Iniciando request para obter talhões da propriedade: {PropriedadeId}", propriedadeId);
            
            try
            {
                var talhoes = await _repository.GetTalhoesByPropriedade(propriedadeId, token);

                if (talhoes == null || talhoes.Count == 0)
                {
                    _logger.LogWarning("Nenhum talhão encontrado para a propriedade {PropriedadeId}", propriedadeId);
                    return new List<TalhaoDto>();
                }

                _logger.LogInformation("Total de talhões recuperados para a propriedade {PropriedadeId}: {Count}", propriedadeId, talhoes.Count);
                return talhoes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao recuperar talhões da propriedade {PropriedadeId}", propriedadeId);
                return new List<TalhaoDto>();
            }
        }
    }
}
