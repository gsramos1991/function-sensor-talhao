using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AgroSolutions.Domain.Dto
{
    public class PropriedadeDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        
        [JsonPropertyName("produtorId")]
        public Guid ProdutorId { get; set; }
        
        [JsonPropertyName("nome")]
        public string Nome { get; set; } = string.Empty;
        
        [JsonPropertyName("dataCriacao")]
        public DateTime DataCriacao { get; set; }
        
        [JsonPropertyName("dataAtualizacao")]
        public DateTime? DataAtualizacao { get; set; }
        
        [JsonPropertyName("excluido")]
        public bool Excluido { get; set; }
        
        [JsonPropertyName("talhoes")]
        public List<object> Talhoes { get; set; } = new List<object>();
    }
}
