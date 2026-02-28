using AgroSolutions.Busines.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroSolutions.Busines.Interface
{
    public interface IGetTempoService
    {
        Task<GetTempo> RequestDadosTempo(string city);
        SendDataSensor ConvertToSendDataSensor(GetTempo dadosTempo, Guid talhaoId);
    }
}
