using System.Collections.Generic;

namespace EasyCiteLib.Models.Search
{
    public class DocumentSearchResults
    {
        public List<Article> Results { get; set; } = new List<Article>();
        
        public int PageCount { get; set; }
        
        public class Article
        {
            public string Id { get; set; }
            public string Title { get; set; }
        }
    }
}