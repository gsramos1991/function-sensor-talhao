using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroSolutions.Domain.Interfaces
{
    public interface IAutenticateApi
    {
        Task<string> GetToken(string Username, string Passoword);
    }
}
