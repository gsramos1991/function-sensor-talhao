using AgroSolutions.Domain.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroSolutions.Domain.Interfaces
{
    public interface ISendDataSensorRepository
    {
        Task<bool> SendDadosTalhao(SendDataSensorDto sendDataSensor);
    }
}
