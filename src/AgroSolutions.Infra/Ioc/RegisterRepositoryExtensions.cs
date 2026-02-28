using AgroSolutions.Domain.Interfaces;
using AgroSolutions.Infra.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroSolutions.Infra.Ioc
{
    public static class RegisterRepositoryExtensions
    {
        public static IServiceCollection AddRegisterRepositories(this IServiceCollection services)
        {
            // Register your repositories here
            services.AddTransient<IGetTempoRepository, GetTempoRepository>();
            services.AddTransient<ISendDataSensorRepository, SendDataSensorServiceBusRepository>();
            services.AddTransient<IGetTalhaoRepository, GetTalhaoRepository>();
            services.AddTransient<IGetPropriedadesRepository, GetPropriedadesRepository>();
            services.AddTransient<IAutenticateApi, AutenticateApiRepository>();
            return services;
        }
    }
}
