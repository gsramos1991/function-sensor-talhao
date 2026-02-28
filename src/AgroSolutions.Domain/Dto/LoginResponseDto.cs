using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AgroSolutions.Domain.Dto
{
    public class LoginResponseDto
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }
        
        [JsonPropertyName("expiresIn")]
        public int ExpiresIn { get; set; }
    }
}
