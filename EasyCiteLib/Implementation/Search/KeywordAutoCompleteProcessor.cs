using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EasyCiteLib.Interface.Search;
using EasyCiteLib.Models;
using EasyCiteLib.Repository;

namespace EasyCiteLib.Implementation.Search
{
    public class KeywordAutoCompleteProcessor : IKeywordAutoCompleteProcessor
    {
        private readonly DocumentContext _documentContext;

        public KeywordAutoCompleteProcessor(DocumentContext documentContext)
        {
            _documentContext = documentContext;
        }

        public async Task<Results<List<string>>> AutoCompleteKeywordsAsync(string term, int resultsCount)
        {
            var results = new Results<List<string>>();

            try
            {
                results.Data = await _documentContext.AutoCompleteKeywordsAsync(term, resultsCount);
            }
            catch (Exception e)
            {
                results.AddException(new Exception("Failed to autocomplete keywords.", e));
            }

            return results;
        }
    }
}