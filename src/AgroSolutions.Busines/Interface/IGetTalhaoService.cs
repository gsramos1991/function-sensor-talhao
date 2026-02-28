using AgroSolutions.Busines.Model;
using AgroSolutions.Domain.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroSolutions.Busines.Interface
{
    public interface IGetTalhaoService
    {
        Task<List<TalhaoDto>> GetTalhoesByPropriedade(Guid propriedadeId, string token);
    }
}
