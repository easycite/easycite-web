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
        [JsonProperty("publishDateStr")]
        public string PublishDate { get; set; }
        [JsonProperty("publishYear")]
        public int? PublishYear { get; set; }

        [JsonProperty("abstract")]
        public string Abstract { get; set; }
        [JsonProperty("publicationTitle")]
        public string PublicationTitle { get; set; }
        
        [JsonProperty("pageRank")]
        public float PageRank { get; set; }

        [JsonIgnore]
        public List<Author> Authors { get; set; }
        
        [JsonIgnore]
        public List<string> Keywords { get; set; }
    }
}