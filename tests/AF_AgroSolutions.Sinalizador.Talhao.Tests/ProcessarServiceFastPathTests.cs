using AgroSolutions.Busines.Interface;
using AgroSolutions.Busines.Services;
using AgroSolutions.Domain.Dto;
using Microsoft.Extensions.Logging.Abstractions;

namespace AF_AgroSolutions.Sinalizador.Talhao.Tests;

public class ProcessarServiceFastPathTests
{
    [Fact]
    public async Task ProcessarDadosTalhoes_WhenAuthReturnsEmptyToken_DoesNotFetchPropriedades()
    {
        var auth = new FakeAutenticateService(tokenToReturn: "");
        var propriedades = new SpyGetPropriedadesService(resultToReturn: new List<PropriedadeDto> { new() });

        var sut = new ProcessarService(
            NullLogger<ProcessarService>.Instance,
            auth,
            propriedades,
            new ThrowIfCalledGetTalhaoService(),
            new ThrowIfCalledGetTempoService(),
            new ThrowIfCalledSendDataSensorService());

        await sut.ProcessarDadosTalhoes("u", "p");

        Assert.Equal(0, propriedades.CallCount);
    }

    [Fact]
    public async Task ProcessarDadosTalhoes_WhenNoPropriedades_DoesNotFetchTalhoes()
    {
        var auth = new FakeAutenticateService(tokenToReturn: "token");
        var propriedades = new SpyGetPropriedadesService(resultToReturn: new List<PropriedadeDto>());
        var talhoes = new SpyGetTalhaoService(resultToReturn: new List<TalhaoDto> { new() });

        var sut = new ProcessarService(
            NullLogger<ProcessarService>.Instance,
            auth,
            propriedades,
            talhoes,
            new ThrowIfCalledGetTempoService(),
            new ThrowIfCalledSendDataSensorService());

        await sut.ProcessarDadosTalhoes("u", "p");

        Assert.Equal(1, propriedades.CallCount);
        Assert.Equal(0, talhoes.CallCount);
    }

    [Fact]
    public async Task ProcessarDadosTalhoes_WhenNoTalhoes_DoesNotFetchTempoOrSendSensorData()
    {
        var auth = new FakeAutenticateService(tokenToReturn: "token");
        var propriedadeId = Guid.NewGuid();
        var propriedades = new SpyGetPropriedadesService(resultToReturn: new List<PropriedadeDto>
        {
            new() { Id = propriedadeId, Nome = "Prop" }
        });
        var talhoes = new SpyGetTalhaoService(resultToReturn: new List<TalhaoDto>());
        var tempo = new SpyGetTempoService();
        var send = new SpySendDataSensorService();

        var sut = new ProcessarService(
            NullLogger<ProcessarService>.Instance,
            auth,
            propriedades,
            talhoes,
            tempo,
            send);

        await sut.ProcessarDadosTalhoes("u", "p");

        Assert.Equal(1, propriedades.CallCount);
        Assert.Equal(1, talhoes.CallCount);
        Assert.Equal(0, tempo.CallCount);
        Assert.Equal(0, send.CallCount);
    }

    private sealed class FakeAutenticateService : IAutenticateService
    {
        private readonly string _tokenToReturn;

        public FakeAutenticateService(string tokenToReturn) => _tokenToReturn = tokenToReturn;

        public Task<string> Autenticar(string username, string password) => Task.FromResult(_tokenToReturn);
    }

    private sealed class SpyGetPropriedadesService : IGetPropriedadesService
    {
        private readonly List<PropriedadeDto> _resultToReturn;

        public SpyGetPropriedadesService(List<PropriedadeDto> resultToReturn) => _resultToReturn = resultToReturn;

        public int CallCount { get; private set; }

        public Task<List<PropriedadeDto>> GetPropriedades(string token)
        {
            CallCount++;
            return Task.FromResult(_resultToReturn);
        }
    }

    private sealed class SpyGetTalhaoService : IGetTalhaoService
    {
        private readonly List<TalhaoDto> _resultToReturn;

        public SpyGetTalhaoService(List<TalhaoDto> resultToReturn) => _resultToReturn = resultToReturn;

        public int CallCount { get; private set; }

        public Task<List<TalhaoDto>> GetTalhoesByPropriedade(Guid propriedadeId, string token)
        {
            CallCount++;
            return Task.FromResult(_resultToReturn);
        }
    }

    private sealed class SpyGetTempoService : IGetTempoService
    {
        public int CallCount { get; private set; }

        public Task<AgroSolutions.Busines.Model.GetTempo> RequestDadosTempo(string city)
        {
            CallCount++;
            throw new Xunit.Sdk.XunitException("Nao deveria chamar RequestDadosTempo neste teste.");
        }

        public AgroSolutions.Busines.Model.SendDataSensor ConvertToSendDataSensor(AgroSolutions.Busines.Model.GetTempo dadosTempo, Guid talhaoId)
        {
            CallCount++;
            throw new Xunit.Sdk.XunitException("Nao deveria chamar ConvertToSendDataSensor neste teste.");
        }
    }

    private sealed class SpySendDataSensorService : ISendDataSensorService
    {
        public int CallCount { get; private set; }

        public Task<bool> SendDadosTalhao(AgroSolutions.Busines.Model.SendDataSensor sendDataSensor)
        {
            CallCount++;
            throw new Xunit.Sdk.XunitException("Nao deveria chamar SendDadosTalhao neste teste.");
        }
    }

    private sealed class ThrowIfCalledGetTalhaoService : IGetTalhaoService
    {
        public Task<List<TalhaoDto>> GetTalhoesByPropriedade(Guid propriedadeId, string token)
            => throw new Xunit.Sdk.XunitException("Nao deveria chamar GetTalhoesByPropriedade neste teste.");
    }

    private sealed class ThrowIfCalledGetTempoService : IGetTempoService
    {
        public Task<AgroSolutions.Busines.Model.GetTempo> RequestDadosTempo(string city)
            => throw new Xunit.Sdk.XunitException("Nao deveria chamar RequestDadosTempo neste teste.");

        public AgroSolutions.Busines.Model.SendDataSensor ConvertToSendDataSensor(AgroSolutions.Busines.Model.GetTempo dadosTempo, Guid talhaoId)
            => throw new Xunit.Sdk.XunitException("Nao deveria chamar ConvertToSendDataSensor neste teste.");
    }

    private sealed class ThrowIfCalledSendDataSensorService : ISendDataSensorService
    {
        public Task<bool> SendDadosTalhao(AgroSolutions.Busines.Model.SendDataSensor sendDataSensor)
            => throw new Xunit.Sdk.XunitException("Nao deveria chamar SendDadosTalhao neste teste.");
    }
}
