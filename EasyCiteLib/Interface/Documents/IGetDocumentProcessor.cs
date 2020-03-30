using System.Collections.Generic;
using EasyCiteLib.Repository;

namespace EasyCiteLib.Interface.Documents
{
    public interface IGetDocumentProcessor
    {
        IAsyncEnumerable<Document> GetDocumentsAsync(IEnumerable<string> documentIds);
    }
}