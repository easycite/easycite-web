using System.Collections.Generic;

namespace EasyCiteLib.Models.Search
{
    public class SearchData
    {
        public int PageNumber { get; set; }
        public int ItemsPerPage { get; set; }
        public List<string> SearchByIds { get; set; } = new List<string>();
        public List<string> SearchTags { get; set; } = new List<string>();
        public SearchSortType SearchSortType { get; set; }
        public bool ForceNoCache { get; set; }
        public SearchDepth SearchDepth { get; set; }
    }
    public enum SearchDepth
    {
        Low = 0,
        Medium = 1,
        High = 2
    }
}