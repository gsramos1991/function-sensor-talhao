using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroSolutions.Busines.Model
{
    public class GetTalhaoModel
    {
        public Guid IdTalhao { get; set; }
        public int IdFazenda { get; set; }
        public string LocalTalhao { get; set; } = string.Empty;
    }
}
