using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroSolutions.Busines.Interface
{
    public interface IProcessarService
    {
        Task ProcessarDadosTalhoes(string username, string password);
    }
}
