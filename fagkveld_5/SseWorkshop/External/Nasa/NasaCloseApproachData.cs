using System.Text.Json.Serialization;

namespace SseWorkshop.External.Nasa
{
    public partial class NasaCloseApproachData
    {
        [JsonPropertyName("signature")]
        public Signature Signature { get; set; }

        [JsonPropertyName("count")]
        public long Count { get; set; }

        [JsonPropertyName("fields")]
        public string[] Fields { get; set; }

        [JsonPropertyName("data")]
        public string[][] Data { get; set; }
    }

    public partial class Signature
    {
        [JsonPropertyName("source")]
        public string Source { get; set; }

        [JsonPropertyName("version")]
        public string Version { get; set; }
    }
}


