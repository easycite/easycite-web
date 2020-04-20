using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using EasyCiteLib.Interface.Documents;
using EasyCiteLib.Models.Search;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EasyCiteLib.Implementation.Documents
{
    public class DocumentSearchProcessor : IDocumentSearchProcessor
    {
        private const string _baseUri = "https://ieeexplore.ieee.org";
        private const string _searchUri = "/rest/search";
        private const string _searchUriReferrerFormat = "/search/searchresult.jsp?newsearch=true&queryText={0}";

        private const string _citationFormatUriFormat = "/rest/search/citation/format?recordIds={0}&download-format={1}&lite=true";
        private const string _citationFormatUriReferrerFormat = "/document/{0}";

        private const string _plainTextFormat = "download-ascii";
        private const string _bibTexFormat = "download-bibtex";

        private readonly HttpClient _httpClient;

        public DocumentSearchProcessor(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();

            _httpClient.BaseAddress = new Uri(_baseUri);
        }

        public async Task<DocumentSearchResults> SearchByNameAsync(string query, int itemsPerPage = 10, int page = 1)
        {
            if (page < 1) page = 1;
            if (itemsPerPage < 0) itemsPerPage = 1;

            string referrerUri = _baseUri + string.Format(_searchUriReferrerFormat, Uri.EscapeDataString(query));

            using var request = new HttpRequestMessage(HttpMethod.Post, _searchUri)
            {
                Headers =
                {
                    Referrer = new Uri(referrerUri)
                },
                Content = new StringContent(JsonConvert.SerializeObject(new SearchRequest
                    {
                        QueryText = query,
                        ReturnFacets = new[]
                        {
                            "ALL"
                        },
                        PageNumber = page,
                        ItemsPerPage = itemsPerPage
                    }),
                    Encoding.Default,
                    "application/json")
            };

            using HttpResponseMessage response = await _httpClient.SendAsync(request);
            var stream = await response.Content.ReadAsStreamAsync();

            using var streamReader = new StreamReader(stream);
            using var jsonReader = new JsonTextReader(streamReader);

            JObject obj = await JObject.LoadAsync(jsonReader);

            var searchResults = new DocumentSearchResults();

            if (obj["records"] is JArray records)
            {
                searchResults.Results = records.Select(a => new DocumentSearchResults.Article
                    {
                        Id = a["articleNumber"]?.Value<string>(),
                        Title = a["articleTitle"]?.Value<string>()
                    })
                    .Where(a => !string.IsNullOrEmpty(a.Id) && !string.IsNullOrEmpty(a.Title))
                    .ToList();
            }

            if (obj.TryGetValue("totalPages", out JToken totalPages) && totalPages.Type == JTokenType.Integer)
                searchResults.PageCount = totalPages.Value<int>();

            return searchResults;
        }

        public async Task<DocumentSearchResults.Article> GetByNameExactAsync(string name)
        {
            var results = await SearchByNameAsync($"\"{name}\"");

            return results.Results.FirstOrDefault(r => r.Title.Trim().Equals(name.Trim(), StringComparison.CurrentCultureIgnoreCase));
        }

        public Task<string> GetPlainTextCitationAsync(string documentId) => GetCitationAsync(documentId, _plainTextFormat);

        public Task<string> GetBibTexCitationAsync(string documentId) => GetCitationAsync(documentId, _bibTexFormat);

        async Task<string> GetCitationAsync(string documentId, string format)
        {
            string requestUri = _baseUri + string.Format(_citationFormatUriFormat, documentId, format);
            string referrerUri = _baseUri + string.Format(_citationFormatUriReferrerFormat, documentId);
            
            using var request = new HttpRequestMessage(HttpMethod.Get, requestUri)
            {
                Headers =
                {
                    Referrer = new Uri(referrerUri)
                }
            };
            
            using HttpResponseMessage response = await _httpClient.SendAsync(request);
            
            var stream = await response.Content.ReadAsStreamAsync();

            using var streamReader = new StreamReader(stream);
            using var jsonReader = new JsonTextReader(streamReader);

            JObject obj = await JObject.LoadAsync(jsonReader);

            return obj["data"].Value<string>();
        }

        class SearchRequest
        {
            [JsonProperty("queryText")]
            public string QueryText { get; set; }

            [JsonProperty("returnFacets")]
            public IEnumerable<string> ReturnFacets { get; set; }

            [JsonProperty("pageNumber")]
            public int PageNumber { get; set; }
            
            [JsonProperty("rowsPerPage")]
            public int ItemsPerPage { get; set; }
        }
    }
}