using Neo4jClient;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Neo4jClient.Cypher;
using Newtonsoft.Json;

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

        public async Task<IList<Document>> GetDocumentsAsync(IEnumerable<string> documentIds)
        {
            using var client = _graphClientFactory.Create();

            await client.ConnectAsync();

            var query = client.Cypher
                .Unwind(documentIds, "docId")
                .Match("(d:Document)")
                .Where("d.id = docId")
                .OptionalMatch("(a:Author)-[:AUTHORED]->(d)")
                .Return((d, c) => new
                {
                    Document = d.As<Document>(),
                    Authors = Return.As<IEnumerable<Author>>("collect(a)"),
                });

            return (await query.ResultsAsync).Select(r =>
            {
                r.Document.Authors = r.Authors.ToList();
                return r.Document;
            }).ToList();
        }

        public async Task<(IList<Document> Documents, int TotalCount)> SearchDocumentsAsync(ICollection<string> documentIds, ICollection<string> tags, int skip = 0, int limit = 0)
        {
            using var client = _graphClientFactory.Create();

            await client.ConnectAsync();

            var queryPart = client.Cypher
                .Unwind(documentIds, "docId")
                .Match("(doc:Document)")
                .Where("doc.id = docId");

            queryPart = GetDocumentMatchClause(queryPart, true)
                .Return<DocumentResult>("ref")
                .Union();
            var query = GetDocumentMatchClause(queryPart, false)
                .Return<DocumentResult>("ref");

            List<string> refDocIds = (await query.ResultsAsync)
                .OrderByDescending(d => d.PageRank)
                .Select(d => d.Id)
                .Except(documentIds)
                .ToList();

            IEnumerable<string> results = refDocIds;

            if (skip > 0)
                results = results.Skip(skip);
            if (limit > 0)
                results = results.Take(limit);

            return (await GetDocumentsAsync(results), refDocIds.Count);

            ICypherFluentQuery GetDocumentMatchClause(ICypherFluentQuery q, bool refTo)
            {
                string dirFront = refTo ? "-" : "<-";
                string dirBack = refTo ? "->" : "-";
                string tagParamName = refTo ? "tagsTo" : "tagsFrom";
                string matchQueryPart = $"(doc){dirFront}[:CITES*1..{_resultsSearchDepth}]{dirBack}(ref:Document)";
                
                if (tags.Count > 0)
                {
                    return q.Match($"{matchQueryPart}-[:TAGGED_BY]->(k:Keyword)")
                        .Where($"k.keyword in {{{tagParamName}}}")
                        .WithParam(tagParamName, tags);
                }

                return q.Match(matchQueryPart)
                    .Where("ref.visited = true");
            }
        }

        class DocumentResult
        {
            [JsonProperty("id")]
            public string Id { get; set; }
            [JsonProperty("pageRank")]
            public double PageRank { get; set; }
        }
    }
}
