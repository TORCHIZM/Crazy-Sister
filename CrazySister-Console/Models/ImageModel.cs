using Newtonsoft.Json;

namespace Server.Models
{
    [JsonObject]
    public class ImageModel
    {
        [JsonProperty("path")]
        public string Path { get; set; }
    }
}
