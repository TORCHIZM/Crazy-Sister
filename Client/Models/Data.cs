using Newtonsoft.Json;

namespace Client.Models
{
    [JsonObject]
    public class Data
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("Contents")]
        public string Contents { get; set; }
    }
}
