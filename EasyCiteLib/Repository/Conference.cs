using Newtonsoft.Json;

namespace EasyCiteLib.Repository
{
    public class Conference
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}