using AgroSolutions.Busines.Interface;
using AgroSolutions.Busines.Model;
using AgroSolutions.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroSolutions.Busines.Services
{
    public class GetTempoService : IGetTempoService
    {
        private readonly ILogger<GetTempoService> _logger;
        private readonly IGetTempoRepository _repository;
        private readonly IMemoryCache _cache;
        private static readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(60);
        private static readonly string[] _cidades = new[]
        {
                "Riyadh",
                "Las Vegas",
                "Phoenix",
                "Windhoek",
                "Timbuktu",
                "Alice Springs",
                "Khartoum",
                "Teheran",
                "Dubai",
                "Abu Dhabi",
                "Baghdad",
                "Amman",
                "Denver",
                "Doha",
                "Marrakech",
                "Jerusalem",
                "Kuwait City",
                "Kabul",
                "Damascus",
                "El Paso",
                "Salt Lake City",
                "Yuma",
                "Luxor",
                "Medina",
                "Jeddah",
                "Muscat",
                "Nouakchott",
                "Cairo",
                "Chihuahua",
                "Mexicali",
                "Tucson",
                "Bikaner",
                "Aswan",
                "Upington",
                "Quetta",
                "Adrar",
                "Manaus",
                "Singapore",
                "Belem",
                "Kuala Lumpur",
                "Bangkok",
                "Jakarta",
                "Mumbai",
                "Panama City",
                "Manila",
                "Tokyo",
                "Miami",
                "Rio de Janeiro",
                "Hong Kong",
                "Seoul",
                "Buenos Aires",
                "Sydney",
                "Ho Chi Minh City",
                "Taipei",
                "Osaka",
                "Vancouver"
        };

        public GetTempoService(ILogger<GetTempoService> logger, IGetTempoRepository repository, IMemoryCache cache)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        private string GetCidadeAleatoria()
        {
            var random = new Random();
            var index = random.Next(0, 55);
            var cidadeEscolhida = _cidades[index];
            _logger.LogInformation("Cidade aleatória selecionada: {Cidade}", cidadeEscolhida);
            return cidadeEscolhida;
        }

        public async Task<GetTempo> RequestDadosTempo(string city)
        {
            // Usa uma cidade aleatória ao invés do parâmetro recebido
            var cidadeAleatoria = GetCidadeAleatoria();

            if (_cache.TryGetValue(cidadeAleatoria, out GetTempo? cachedTempo))
            {
                _logger.LogInformation("Cache hit para cidade: {cidade}", cidadeAleatoria);
                return cachedTempo;
            }

            _logger.LogInformation("Cache miss para cidade: {cidade}. Consultando API...", cidadeAleatoria);
            var temperatureDataDto = await _repository.RequestDadosTempo(cidadeAleatoria);

            if (temperatureDataDto == null)
            {
                _logger.LogWarning("No temperature data received for city: {cidade}", cidadeAleatoria);
                return null;
            }

            var temperatureModel = ConvertDtoToModel(temperatureDataDto);

            _cache.Set(cidadeAleatoria, temperatureModel, _cacheDuration);
            _logger.LogInformation("Dados do tempo armazenados em cache por {Minutos} minuto(s) para: {cidade}", _cacheDuration.TotalMinutes, cidadeAleatoria);

            return temperatureModel;
        }

        private GetTempo ConvertDtoToModel(AgroSolutions.Domain.Dto.GetTempoDto dto)
        {
            return new GetTempo
            {
                location = new Location
                {
                    name = dto.location?.name,
                    country = dto.location?.country,
                    lat = dto.location?.lat ?? 0,
                    lon = dto.location?.lon ?? 0,
                    localtime = dto.location?.localtime
                },
                current = new Current
                {
                    temp_c = dto.current?.temp_c ?? 0,
                    wind_kph = dto.current?.wind_kph ?? 0,
                    wind_dir = dto.current?.wind_dir,
                    humidity = dto.current?.humidity ?? 0,
                    cloud = dto.current?.cloud ?? 0
                },
                forecast = new Forecast
                {
                    forecastday = dto.forecast?.forecastday?.Select(f => new Forecastday
                    {
                        date = f.date,
                        date_epoch = f.date_epoch,
                        day = new Day
                        {
                            maxtemp_c = f.day?.maxtemp_c ?? 0,
                            mintemp_c = f.day?.mintemp_c ?? 0,
                            uv = f.day?.uv ?? 0
                        }
                    }).ToArray()
                }
            };
        }

        public SendDataSensor ConvertToSendDataSensor(GetTempo dadosTempo, Guid talhaoId)
        {
            if (dadosTempo == null)
            {
                _logger.LogWarning("Dados de tempo nulos ao converter para SendDataSensor");
                return null;
            }

            return new SendDataSensor
            {
                TalhaoId = talhaoId,
                Umidade = dadosTempo.current?.humidity ?? 0,
                DataAfericao = DateTime.Now,
                Temperatura = dadosTempo.current?.temp_c ?? 0,
                IndiceUv = (int)(dadosTempo.forecast?.forecastday?.FirstOrDefault()?.day?.uv ?? 0),
                VelocidadeVento = (decimal)(dadosTempo.current?.wind_kph ?? 0)
            };
        }
    }


}
