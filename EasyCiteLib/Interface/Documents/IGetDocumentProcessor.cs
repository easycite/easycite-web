using System.Collections.Generic;
using System.Threading.Tasks;
using EasyCiteLib.Repository;

namespace EasyCiteLib.Interface.Documents
{
    public interface IGetDocumentProcessor
    {
        Task<IList<Document>> GetDocumentsAsync(IEnumerable<string> documentIds);
    }
}