using System.Collections.Generic;
using System.ComponentModel;

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
        [HelpText("Fastest search, might not be as thorough")]
        Low = 0,
        
        [HelpText("Good balance between speed and thoroughness")]
        Medium = 1,
        
        [HelpText("Slowest search, most thorough")]
        High = 2
    }
}