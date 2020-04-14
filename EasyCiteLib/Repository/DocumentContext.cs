using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyCiteLib.Models.Search;
using Microsoft.Extensions.Configuration;
using Neo4jClient;
using Neo4jClient.Cypher;
using Newtonsoft.Json;
using static EasyCiteLib.Implementation.Helpers.RunExtension;

namespace EasyCiteLib.Repository
{
    public class DocumentContext
    {
        private readonly IGraphClientFactory _graphClientFactory;
        private readonly int _resultsSearchDepth;

        public DocumentContext(IGraphClientFactory graphClientFactory, IConfiguration configuration)
        {
            _graphClientFactory = graphClientFactory;
            _resultsSearchDepth = int.Parse(configuration["ResultsSearchDepth"]);
        }

        public async Task<List<Document>> GetDocumentsAsync(IEnumerable<string> documentIds)
        {
            using var client = _graphClientFactory.Create();

            await client.ConnectAsync();

            Task<List<Document>> getDocAndAuthorsTask = Run(async () =>
            {
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
            });

            Task<Dictionary<string, List<string>>> getKeywordsTask = Run(async () =>
            {
                var query = client.Cypher
                    .Unwind(documentIds, "docId")
                    .Match("(d:Document)")
                    .Where("d.id = docId")
                    .Match("(d)-[:TAGGED_BY]->(k:Keyword)")
                    .Return(() => new
                    {
                        DocumentId = Return.As<string>("d.id"),
                        Keywords = Return.As<IEnumerable<string>>("collect(k.keyword)")
                    });

                return (await query.ResultsAsync).ToDictionary(r => r.DocumentId, r => r.Keywords.ToList());
            });

            await Task.WhenAll(getDocAndAuthorsTask, getKeywordsTask);

            List<Document> documents = getDocAndAuthorsTask.Result;
            Dictionary<string, List<string>> keywordMap = getKeywordsTask.Result;

            foreach (var doc in documents)
                if (keywordMap.TryGetValue(doc.Id, out List<string> keywords))
                    doc.Keywords = keywords;

            return documents;
        }

        public async Task<List<string>> SearchDocumentsAsync(IEnumerable<string> documentIds, SearchSortType sortType, int depth)
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

            IEnumerable<DocumentResult> results = (await query.ResultsAsync).Where(r => !documentIdList.Contains(r.Reference.Id));

            IEnumerable<string> idList = sortType switch
            {
                SearchSortType.PageRank => results
                    .GroupBy(r => r.Reference.Id)
                    .Select(g => new
                    {
                        Count = g.Count(),
                        Reference = g.First().Reference
                    })
                    .OrderByDescending(r => r.Count)
                    .ThenByDescending(r => r.Reference.PageRank)
                    .Select(r => r.Reference.Id)
                    .Except(documentIdList),
                SearchSortType.Recency => results.OrderByDescending(d => d.Reference.PublishDate).Select(d => d.DocumentId),
                SearchSortType.AuthorPopularity => results.OrderByDescending(d => d.AuthorPopularity).Select(d => d.DocumentId),
                _ => throw new ArgumentOutOfRangeException(nameof(sortType), sortType, null)
            };

            return idList.ToList();

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
                    .Return(@ref => new DocumentResult
                    {
                        DocumentId = Return.As<string>("doc.id"),
                        Reference = @ref.As<ReferenceResult>(),
                        AuthorPopularity = Return.As<double>("max(a.popularity)")
                    });
            }
        }

        class ReferenceResult
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("pageRank")]
            public double PageRank { get; set; }

            [JsonProperty("publishDate")]
            public DateTime? PublishDate { get; set; }
        }

        class DocumentResult
        {
            public string DocumentId { get; set; }

            public ReferenceResult Reference { get; set; }

            public double AuthorPopularity { get; set; }
        }
    }
}