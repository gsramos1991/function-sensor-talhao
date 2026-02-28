using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroSolutions.Domain.Dto
{
    public class GetTempoDto
    {
        public Location location { get; set; }
        public Current current { get; set; }
        public Forecast forecast { get; set; }
    }

    public class Location
    {
        public string name { get; set; }
        public string country { get; set; }
        public float lat { get; set; }
        public float lon { get; set; }
        public string localtime { get; set; }
    }

    public class Current
    {
        public float temp_c { get; set; }
        public float wind_kph { get; set; }
        public string wind_dir { get; set; }
        public int humidity { get; set; }
        public int cloud { get; set; }
    }

    public class Forecast
    {
        public Forecastday[] forecastday { get; set; }
    }

    public class Forecastday
    {
        public string date { get; set; }
        public int date_epoch { get; set; }
        public Day day { get; set; }
    }

    public class Day
    {
        public float maxtemp_c { get; set; }
        public float mintemp_c { get; set; }
        public float uv { get; set; }
    }

}
