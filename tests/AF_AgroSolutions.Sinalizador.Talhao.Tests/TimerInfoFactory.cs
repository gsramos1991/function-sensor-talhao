using Microsoft.Azure.Functions.Worker;
using System.Reflection;

namespace AF_AgroSolutions.Sinalizador.Talhao.Tests;

internal static class TimerInfoFactory
{
    public static TimerInfo CreateMinimal()
    {
        // O TimerInfo do isolated worker não é trivial de instanciar manualmente.
        // Para manter os testes resilientes a mudanças de assinatura, criamos via reflexão.
        var timerInfoType = typeof(TimerInfo);

        var ctor = timerInfoType
            .GetConstructors(BindingFlags.Public | BindingFlags.Instance)
            .OrderBy(c => c.GetParameters().Length)
            .FirstOrDefault();

        if (ctor is null)
        {
            throw new Xunit.Sdk.XunitException("Nao foi possivel encontrar um construtor publico para TimerInfo.");
        }

        var args = ctor.GetParameters()
            .Select(p => p.HasDefaultValue ? p.DefaultValue : GetDefaultValue(p.ParameterType))
            .ToArray();

        return (TimerInfo)ctor.Invoke(args);
    }

    private static object? GetDefaultValue(Type t)
    {
        if (t == typeof(string))
        {
            return null;
        }

        return t.IsValueType ? Activator.CreateInstance(t) : null;
    }
}
