using System.Collections.Generic;
using System.Threading.Tasks;
using EasyCiteLib.Models;

namespace EasyCiteLib.Interface.Search
{
    public interface IKeywordAutoCompleteProcessor
    {
        Task<Results<List<string>>> AutoCompleteKeywordsAsync(string term, int resultsCount);
    }
}