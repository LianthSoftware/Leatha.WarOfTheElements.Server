using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Leatha.WarOfTheElements.Common.Communication.Transfer
{
    public sealed class AuraTemplateObject
    {
        [Required]
        [JsonPropertyName("auraId")]
        public int AuraId { get; set; }

        [Required]
        [JsonPropertyName("auraName")]
        public string AuraName { get; set; } = null!;

        //[Required]
        //[JsonPropertyName("auraType")]
        //[JsonConverter(typeof(JsonStringEnumConverter))]
        //public AuraType AuraType { get; set; }

        //[Required]
        //[JsonPropertyName("auraFlags")]
        //[JsonConverter(typeof(JsonStringEnumConverter))]
        //public AuraFlags AuraFlags { get; set; }

        [Required]
        [JsonPropertyName("parameters")]
        public Dictionary<string, string> Parameters { get; set; } = [];
    }
}
