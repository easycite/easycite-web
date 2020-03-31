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

        public Task<IList<Document>> GetDocumentsAsync(IEnumerable<string> documentIds)
        {
            return _context.GetDocumentsAsync(documentIds);
        }
    }
}