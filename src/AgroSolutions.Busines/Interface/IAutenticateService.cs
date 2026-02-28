using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroSolutions.Busines.Interface
{
    public interface IAutenticateService
    {
        Task<string> Autenticar(string username, string password);
    }
}
