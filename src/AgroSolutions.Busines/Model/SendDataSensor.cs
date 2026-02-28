using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroSolutions.Busines.Model
{
    public class SendDataSensor
    {
        public Guid TalhaoId { get; set; }
        public int Umidade { get; set; }
        public DateTime DataAfericao { get; set; }
        public double Temperatura { get; set; }
        public int IndiceUv { get; set; }
        public decimal VelocidadeVento { get; set; }
    }
}
