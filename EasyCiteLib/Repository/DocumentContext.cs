using Neo4jClient;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyCiteLib.Repository
{
    public class DocumentContext
    {
        readonly IGraphClientFactory _graphClientFactory;

        public DocumentContext(IGraphClientFactory graphClientFactory)
        {
            _graphClientFactory = graphClientFactory;
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
                .OptionalMatch("(d)-[:PUBLISHED_IN]->(c:Conference)")
                .Return((d, a, c) => new
                {
                    Document = d.As<Document>(),
                    Author = a.As<Author>(),
                    Conference = c.As<Conference>()
                });

            return (await query.ResultsAsync).Select(r =>
            {
                r.Document.Author = r.Author;
                r.Document.Conference = r.Conference;
                return r.Document;
            }).ToList();
        }
    }
}
