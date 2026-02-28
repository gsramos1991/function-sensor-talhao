using System;
using System.Text.Json.Serialization;

namespace AgroSolutions.Domain.Dto
{
    public class TalhaoDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        
        [JsonPropertyName("propriedadeId")]
        public Guid PropriedadeId { get; set; }
        
        [JsonPropertyName("nome")]
        public string Nome { get; set; } = string.Empty;
        
        [JsonPropertyName("areaHectares")]
        public decimal AreaHectares { get; set; }
        
        [JsonPropertyName("cultura")]
        public int Cultura { get; set; }
        
        [JsonPropertyName("dataCriacao")]
        public DateTime DataCriacao { get; set; }
        
        [JsonPropertyName("dataAtualizacao")]
        public DateTime? DataAtualizacao { get; set; }
        
        [JsonPropertyName("excluido")]
        public bool Excluido { get; set; }
    }
}
