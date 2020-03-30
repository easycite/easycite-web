using System.Collections.Generic;

namespace EasyCiteLib.Models.Search
{
    public class SearchResultsVm
    {
        public List<ResultVm> Results { get; set; } = new List<ResultVm>();
        public int NumberOfPages { get; set; }
    }
}