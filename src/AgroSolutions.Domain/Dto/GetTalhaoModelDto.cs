using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroSolutions.Domain.Dto
{
    public class GetTalhaoModelDto
    {
        public Guid IdTalhao { get; set; }
        public int IdFazenda { get; set; }
        public string LocalTalhao { get; set; } = string.Empty;
    }
}
