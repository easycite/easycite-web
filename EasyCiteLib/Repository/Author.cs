using Newtonsoft.Json;

namespace EasyCiteLib.Repository
{
    public class Author
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}