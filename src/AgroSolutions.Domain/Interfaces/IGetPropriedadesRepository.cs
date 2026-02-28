using AgroSolutions.Domain.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgroSolutions.Domain.Interfaces
{
    public interface IGetPropriedadesRepository
    {
        Task<List<PropriedadeDto>> GetPropriedades(string token);
    }
}
