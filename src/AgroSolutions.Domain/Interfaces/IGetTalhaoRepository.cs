using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgroSolutions.Domain.Dto;

namespace AgroSolutions.Domain.Interfaces
{
    public interface IGetTalhaoRepository
    {
        Task<List<TalhaoDto>> GetTalhoesByPropriedade(Guid propriedadeId, string token);
    }
}
