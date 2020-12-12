using Newtonsoft.Json;

namespace Client.Models
{
    [JsonObject]
    public class MessageBoxModel
    {
        [JsonProperty("Title")]
        public string Title { get; set; }
        [JsonProperty("Description")]
        public string Description { get; set; }
        [JsonProperty("BoxIcon")]
        public int BoxIcon { get; set; }
        [JsonProperty("BoxButtons")]
        public int BoxButtons { get; set; }
    }
}
