using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyCiteLib.Interface.Documents;
using EasyCiteLib.Repository;
using Microsoft.Extensions.Caching.Memory;

namespace EasyCiteLib.Implementation.Documents
{
    public class LocalCachedGetDocumentProcessor : IGetDocumentProcessor
    {
        private readonly DocumentContext _context;
        private readonly IMemoryCache _cache;

        public LocalCachedGetDocumentProcessor(DocumentContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<IReadOnlyList<Document>> GetDocumentsAsync(IEnumerable<string> documentIds)
        {
            var documents = new List<KeyValuePair<string, Document>>();
            var missingIds = new List<string>();

            foreach (string docId in documentIds)
            {
                string key = LocalCacheKeys.Document(docId);
                bool cached = _cache.TryGetValue(key, out Document cachedDoc);
                documents.Add(new KeyValuePair<string, Document>(docId, cachedDoc));
                if (!cached)
                    missingIds.Add(docId);
            }

            List<Document> missing = await _context.GetDocumentsAsync(missingIds);

            List<Document> results = documents.GroupJoin(missing,
                    d => d.Key,
                    m => m.Id,
                    (d, m) => d.Value ?? m.FirstOrDefault())
                .Where(d => d != null)
                .ToList();

            foreach (var missingDoc in missing)
            {
                string key = LocalCacheKeys.Document(missingDoc.Id);
                _cache.Set(key, missingDoc, TimeSpan.FromMinutes(5));
            }

            return results;
        }
    }
}