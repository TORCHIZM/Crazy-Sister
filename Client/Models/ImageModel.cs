using Newtonsoft.Json;

namespace Client.Models
{
    [JsonObject]
    public class ImageModel
    {
        [JsonProperty("path")]
        public string Path { get; set; }
    }
}
