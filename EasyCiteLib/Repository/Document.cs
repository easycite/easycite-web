using Newtonsoft.Json;
using System;
using System.Collections.Generic;

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
        [JsonProperty("journal")]
        public string Journal { get; set; }
        
        [JsonProperty("pageRank")]
        public float PageRank { get; set; }

        [JsonIgnore]
        public List<Author> Authors { get; set; }
        [JsonIgnore]
        public Conference Conference { get; set; }
    }
}