using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyCiteLib.Models.Search;
using Neo4jClient;
using Neo4jClient.Cypher;
using Newtonsoft.Json;

namespace EasyCiteLib.Repository
{
    public class DocumentContext
    {
        private readonly IGraphClientFactory _graphClientFactory;

        public DocumentContext(IGraphClientFactory graphClientFactory)
        {
            _graphClientFactory = graphClientFactory;
        }

        public async Task<List<Document>> GetDocumentsAsync(IEnumerable<string> documentIds)
        {
            using var client = _graphClientFactory.Create();

            await client.ConnectAsync();

            var query = client.Cypher
                .Unwind(documentIds, "docId")
                .Match("(d:Document)")
                .Where("d.id = docId")
                .OptionalMatch("(a:Author)-[:AUTHORED]->(d)")
                .Return((d, a) => new
                {
                    Document = d.As<Document>(),
                    Authors = a.CollectAs<Author>()
                });

            return (await query.ResultsAsync).Select(r =>
                {
                    r.Document.Authors = r.Authors.ToList();
                    return r.Document;
                })
                .ToList();
        }

        public async Task<List<string>> SearchDocumentsAsync(IEnumerable<string> documentIds, SearchSortType sortType, int depth, ICollection<string> anyTags, ICollection<string> allTags)
        {
            if (depth < 1)
                depth = 1;
            if (depth > 3)
                depth = 3;
            
            var documentIdList = documentIds.ToList();

            using var client = _graphClientFactory.Create();

            await client.ConnectAsync();

            var queryPart = ChainQueryPart(client.Cypher, true)
                .Union();

            ICypherFluentQuery<DocumentResult> query = ChainQueryPart(queryPart, false);

            var queryResults = (await query.ResultsAsync)
                .Where(r => !documentIdList.Contains(r.Reference.Id))
                .GroupBy(r => r.Reference.Id)
                .Select(g => new
                {
                    Count = g.Count(),
                    Result = g.First()
                });

            var results = (sortType switch
            {
                SearchSortType.PageRank => queryResults
                    .OrderByDescending(r => r.Count)
                    .ThenByDescending(r => r.Result.Reference.PageRank),
                SearchSortType.Recency => queryResults
                    .OrderByDescending(d => d.Result.Reference.PublishYear),
                SearchSortType.AuthorPopularity => queryResults.OrderByDescending(d => d.Result.AuthorPopularity),
                _ => throw new ArgumentOutOfRangeException(nameof(sortType), sortType, null)
            }).Select(r => r.Result);

            if (anyTags.Count > 0 || allTags.Count > 0)
            {
                results = results.Where(d => d.Keywords.Count > 0
                    && allTags.All(k => d.Keywords.Contains(k))
                    && (anyTags.Count == 0 || anyTags.Any(k => d.Keywords.Contains(k))));
            }

            return results.Select(r => r.Reference.Id).ToList();

            ICypherFluentQuery<DocumentResult> ChainQueryPart(ICypherFluentQuery q, bool refTo)
            {
                string dirFront = refTo ? "-" : "<-";
                string dirBack = refTo ? "->" : "-";

                return q.Unwind(documentIdList, "docId")
                    .Match("(doc:Document)")
                    .Where("doc.id = docId")
                    .Match($"(doc){dirFront}[:CITES*1..{depth}]{dirBack}(ref:Document)")
                    .Where("ref.visited = true")
                    .Match("(a:Author)-[:AUTHORED]->(ref)")
                    .OptionalMatch("(ref)-[:TAGGED_BY]->(k:Keyword)")
                    .Return(@ref => new DocumentResult
                    {
                        DocumentId = Return.As<string>("doc.id"),
                        Reference = @ref.As<ReferenceResult>(),
                        AuthorPopularity = Return.As<double>("max(a.popularity)"),
                        Keywords = Return.As<List<string>>("collect(k.keyword)")
                    });
            }
        }

        public async Task<List<string>> AutoCompleteKeywordsAsync(string term, int resultsCount)
        {
            using var client = _graphClientFactory.Create();

            await client.ConnectAsync();

            var query = client.Cypher
                .Match("(k:Keyword)")
                .Where("k.keyword STARTS WITH {term}")
                .WithParam("term", term)
                .Return(() => Return.As<string>("k.keyword"))
                .OrderBy("k.keyword")
                .Limit(resultsCount);

            return (await query.ResultsAsync).ToList();
        }

        class ReferenceResult
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("pageRank")]
            public double PageRank { get; set; }

            [JsonProperty("publishYear")]
            public int? PublishYear { get; set; }
        }

        class DocumentResult
        {
            public string DocumentId { get; set; }

            public ReferenceResult Reference { get; set; }

            public double AuthorPopularity { get; set; }
            
            public List<string> Keywords { get; set; }
        }
    }
}