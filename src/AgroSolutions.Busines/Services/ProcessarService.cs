using AgroSolutions.Busines.Interface;
using AgroSolutions.Busines.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroSolutions.Busines.Services
{
    public class ProcessarService : IProcessarService
    {
        private readonly ILogger<ProcessarService> _logger;
        private readonly IAutenticateService _autenticateService;
        private readonly IGetPropriedadesService _getPropriedadesService;
        private readonly IGetTalhaoService _getTalhaoService;
        private readonly IGetTempoService _getTempoService;
        private readonly ISendDataSensorService _sendDataSensorService;

        public ProcessarService(
            ILogger<ProcessarService> logger,
            IAutenticateService autenticateService,
            IGetPropriedadesService getPropriedadesService,
            IGetTalhaoService getTalhaoService,
            IGetTempoService getTempoService,
            ISendDataSensorService sendDataSensorService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _autenticateService = autenticateService ?? throw new ArgumentNullException(nameof(autenticateService));
            _getPropriedadesService = getPropriedadesService ?? throw new ArgumentNullException(nameof(getPropriedadesService));
            _getTalhaoService = getTalhaoService ?? throw new ArgumentNullException(nameof(getTalhaoService));
            _getTempoService = getTempoService ?? throw new ArgumentNullException(nameof(getTempoService));
            _sendDataSensorService = sendDataSensorService ?? throw new ArgumentNullException(nameof(sendDataSensorService));
        }

        public async Task ProcessarDadosTalhoes(string username, string password)
        {
            _logger.LogInformation("========== INICIANDO PROCESSAMENTO DE DADOS DOS TALHÕES ==========");

            try
            {
                // PASSO 1: Realizar login e obter token JWT
                _logger.LogInformation("PASSO 1: Autenticando usuário...");
                var token = await _autenticateService.Autenticar(username, password);
                _logger.LogInformation("Token JWT obtido: {Token}", token != null ? "✓ Token recebido" : "✗ Falha ao obter token");
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogError("Falha na autenticação. Processamento abortado.");
                    return;
                }

                _logger.LogInformation("✓ Autenticação realizada com sucesso!");

                // PASSO 2: Obter lista de propriedades (GET com token JWT)
                _logger.LogInformation("PASSO 2: Buscando propriedades...");
                var propriedades = await _getPropriedadesService.GetPropriedades(token);

                if (propriedades == null || propriedades.Count == 0)
                {
                    _logger.LogWarning("Nenhuma propriedade encontrada. Processamento finalizado.");
                    return;
                }

                _logger.LogInformation("✓ Total de propriedades encontradas: {Count}", propriedades.Count);

                var totalSucesso = 0;
                var totalFalha = 0;

                // Processar cada propriedade
                foreach (var propriedade in propriedades)
                {
                    _logger.LogInformation("========================================");
                    _logger.LogInformation("Processando Propriedade: {PropriedadeId} - {PropriedadeNome}", 
                        propriedade.Id, propriedade.Nome);

                    // PASSO 3: Obter talhões da propriedade (GET com token JWT)
                    _logger.LogInformation("PASSO 3: Buscando talhões da propriedade {PropriedadeNome}...", propriedade.Nome);
                    var talhoes = await _getTalhaoService.GetTalhoesByPropriedade(propriedade.Id, token);

                    if (talhoes == null || talhoes.Count == 0)
                    {
                        _logger.LogWarning("Nenhum talhão encontrado para a propriedade {PropriedadeNome}", propriedade.Nome);
                        continue;
                    }

                    _logger.LogInformation("✓ Total de talhões encontrados: {Count}", talhoes.Count);

                    // Processar cada talhão
                    foreach (var talhao in talhoes)
                    {
                        _logger.LogInformation("----------------------------------------");
                        _logger.LogInformation("Processando Talhão: {TalhaoId} - {TalhaoNome}", 
                            talhao.Id, talhao.Nome);

                        try
                        {
                            // PASSO 4: Obter dados do tempo para o talhão
                            // Usar o nome da propriedade como localização
                            _logger.LogInformation("PASSO 4: Buscando dados meteorológicos para: {LocalTalhao}", 
                                propriedade.Nome);

                            var dadosTempo = await _getTempoService.RequestDadosTempo(propriedade.Nome);

                            if (dadosTempo?.location == null || dadosTempo?.current == null)
                            {
                                _logger.LogWarning("Dados meteorológicos não encontrados para: {LocalTalhao}", 
                                    propriedade.Nome);
                                totalFalha++;
                                continue;
                            }

                            // Log dos dados meteorológicos coletados
                            _logger.LogInformation(
                                "Dados meteorológicos coletados - Temp={Temperature}°C, Umidade={Humidity}%, " +
                                "Vento={WindSpeed}km/h, UV={UVIndex}",
                                dadosTempo.current.temp_c,
                                dadosTempo.current.humidity,
                                dadosTempo.current.wind_kph,
                                dadosTempo.forecast?.forecastday?.FirstOrDefault()?.day?.uv);

                            // Converter para SendDataSensor
                            var sensorData = _getTempoService.ConvertToSendDataSensor(
                                dadosTempo,
                                talhao.Id);

                            if (sensorData == null)
                            {
                                _logger.LogWarning("Falha ao converter dados do sensor para talhão: {TalhaoId}", 
                                    talhao.Id);
                                totalFalha++;
                                continue;
                            }

                            // PASSO 5: Enviar dados para API de Telemetria (POST)
                            _logger.LogInformation("PASSO 5: Enviando dados para API de Telemetria...");
                            var sucesso = await _sendDataSensorService.SendDadosTalhao(sensorData);

                            if (sucesso)
                            {
                                _logger.LogInformation("✓ Dados enviados com sucesso para talhão: {TalhaoId}",
                                    talhao.Id);
                                totalSucesso++;
                            }
                            else
                            {
                                _logger.LogWarning("✗ Falha ao enviar dados para talhão: {TalhaoId}",
                                    talhao.Id);
                                totalFalha++;
                            }
                            await Task.Delay(2000);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Erro ao processar talhão: {TalhaoId} - {TalhaoNome}", 
                                talhao.Id, talhao.Nome);
                            totalFalha++;
                        }
                    }
                    
                }

                // Resumo final
                _logger.LogInformation("========== PROCESSAMENTO FINALIZADO ==========");
                _logger.LogInformation("Total de talhões processados com sucesso: {Sucesso}", totalSucesso);
                _logger.LogInformation("Total de falhas: {Falha}", totalFalha);
                _logger.LogInformation("===============================================");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro crítico no processamento de dados dos talhões");
                throw;
            }
        }
    }
}
