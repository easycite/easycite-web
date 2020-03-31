using Newtonsoft.Json;
using System;

namespace EasyCiteLib.Repository
{
    public class Document
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("publishDate")]
        public DateTime PublishDate { get; set; }
        [JsonProperty("abstract")]
        public string Abstract { get; set; }

        [JsonIgnore]
        public Author Author { get; set; }
        [JsonIgnore]
        public Conference Conference { get; set; }
    }
}