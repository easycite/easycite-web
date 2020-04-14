using System.Collections.Generic;
using System.Threading.Tasks;
using EasyCiteLib.Interface.Documents;
using EasyCiteLib.Repository;

namespace EasyCiteLib.Implementation.Documents
{
    public class GetDocumentProcessor : IGetDocumentProcessor
    {
        private readonly DocumentContext _context;

        public GetDocumentProcessor(DocumentContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Document>> GetDocumentsAsync(IEnumerable<string> documentIds)
        {
            return await _context.GetDocumentsAsync(documentIds);
        }
    }
}