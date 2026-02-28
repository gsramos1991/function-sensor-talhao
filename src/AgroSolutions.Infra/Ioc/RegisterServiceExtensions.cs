using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgroSolutions.Busines.Interface;
using AgroSolutions.Busines.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AgroSolutions.Infra.Ioc
{
    public static class RegisterServiceExtensions
    {
        public static IServiceCollection AddRegisterServices(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddTransient<IGetTempoService, GetTempoService>();
            services.AddTransient<ISendDataSensorService, SendDataSensorService>();
            services.AddTransient<IGetTalhaoService, GetTalhaoService>();
            services.AddTransient<IGetPropriedadesService, GetPropriedadesService>();
            services.AddTransient<IAutenticateService, AutenticateService>();
            services.AddTransient<IProcessarService, ProcessarService>();
            return services;
        }
    }
}
