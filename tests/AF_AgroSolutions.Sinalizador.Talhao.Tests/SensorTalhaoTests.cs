using AF_AgroSolutions.Sinalizador.Talhao;
using AgroSolutions.Busines.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;

namespace AF_AgroSolutions.Sinalizador.Talhao.Tests;

public class SensorTalhaoTests
{
    [Fact]
    public void Ctor_WhenLoggerNull_Throws()
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection().Build();
        var service = new SpyProcessarService();

        Assert.Throws<ArgumentNullException>(() => new SensorTalhao(null!, config, service));
    }

    [Fact]
    public void Ctor_WhenConfigNull_Throws()
    {
        var service = new SpyProcessarService();

        Assert.Throws<ArgumentNullException>(() => new SensorTalhao(NullLogger<SensorTalhao>.Instance, null!, service));
    }

    [Fact]
    public void Ctor_WhenProcessarServiceNull_Throws()
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection().Build();

        Assert.Throws<ArgumentNullException>(() => new SensorTalhao(NullLogger<SensorTalhao>.Instance, config, null!));
    }

    [Fact]
    public async Task Run_WhenCredentialsMissing_DoesNotCallProcessarService()
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            // Sem EmailService e PasswordService
        }).Build();

        var spy = new SpyProcessarService();
        var fn = new SensorTalhao(NullLogger<SensorTalhao>.Instance, config, spy);
        var timerInfo = TimerInfoFactory.CreateMinimal();

        await fn.Run(timerInfo);

        Assert.False(spy.WasCalled);
    }

    [Fact]
    public async Task Run_WhenCredentialsProvided_CallsProcessarServiceWithExpectedValues()
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["EmailService"] = "user@local.test",
            ["PasswordService"] = "pwd",
        }).Build();

        var spy = new SpyProcessarService();
        var fn = new SensorTalhao(NullLogger<SensorTalhao>.Instance, config, spy);
        var timerInfo = TimerInfoFactory.CreateMinimal();

        await fn.Run(timerInfo);

        Assert.True(spy.WasCalled);
        Assert.Equal("user@local.test", spy.Username);
        Assert.Equal("pwd", spy.Password);
    }

    [Fact]
    public async Task Run_WhenProcessarServiceThrows_Rethrows()
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["EmailService"] = "user@local.test",
            ["PasswordService"] = "pwd",
        }).Build();

        var expected = new InvalidOperationException("boom");
        var spy = new SpyProcessarService { ThrowOnCall = expected };
        var fn = new SensorTalhao(NullLogger<SensorTalhao>.Instance, config, spy);
        var timerInfo = TimerInfoFactory.CreateMinimal();

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => fn.Run(timerInfo));
        Assert.Same(expected, ex);
    }

    private sealed class SpyProcessarService : IProcessarService
    {
        public bool WasCalled { get; private set; }
        public string? Username { get; private set; }
        public string? Password { get; private set; }
        public Exception? ThrowOnCall { get; set; }

        public Task ProcessarDadosTalhoes(string username, string password)
        {
            WasCalled = true;
            Username = username;
            Password = password;

            if (ThrowOnCall is not null)
            {
                throw ThrowOnCall;
            }

            return Task.CompletedTask;
        }
    }
}
