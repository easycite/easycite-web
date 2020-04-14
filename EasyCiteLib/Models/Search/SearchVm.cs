using System.Collections.Generic;
using static EasyCiteLib.Models.Search.SearchData;

namespace EasyCiteLib.Models.Search
{
    public class SearchVm
    {
        public int ProjectId { get; set; }
        public SearchSortType DefaultSortOption { get; set; }
        public List<DropdownOption<SearchSortType>> SortOptions { get; set; } = new List<DropdownOption<SearchSortType>>();
        
        public SearchDepth DefaultSearchDepth { get; set; }
        public List<DropdownOption<SearchDepth>> SearchDepths { get; set; } = new List<DropdownOption<SearchDepth>>();
        
        public List<ReferenceVm> References { get; set; } = new List<ReferenceVm>();
    }
}