using AgroSolutions.Domain.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgroSolutions.Busines.Interface
{
    public interface IGetPropriedadesService
    {
        Task<List<PropriedadeDto>> GetPropriedades(string token);
    }
}
