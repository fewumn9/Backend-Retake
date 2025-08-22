using System.Text.Json.Serialization;

namespace RevueCrafters.Models
{
    internal class ApiResponseDTO
    {
        [JsonPropertyName("msg")]
        public string? Msg { get; set; }

        [JsonPropertyName("id")]
        public string? RevueId { get; set; }
    }
}
