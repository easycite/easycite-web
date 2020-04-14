using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyCiteLib.Interface.Documents;
using EasyCiteLib.Interface.Helpers;
using EasyCiteLib.Repository;

namespace EasyCiteLib.Implementation.Documents
{
    public class MockGetDocumentProcessor : IGetDocumentProcessor
    {
        private readonly IGetRandomDataProcessor _getRandomDataProcessor;

        public MockGetDocumentProcessor(IGetRandomDataProcessor getRandomDataProcessor)
        {
            _getRandomDataProcessor = getRandomDataProcessor;
        }
        public async Task<IReadOnlyList<Document>> GetDocumentsAsync(IEnumerable<string> documentIds)
        {
            return documentIds.Select(documentId => new Document
            {
                Id = documentId,
                Title = _getRandomDataProcessor.GetText(maxWords: 9),
                Authors =
                {
                    new Author
                    {
                        Name = _getRandomDataProcessor.GetName()
                    }
                },
                PublicationTitle = _getRandomDataProcessor.GetText(maxWords: 4),
                Abstract = _getRandomDataProcessor.GetParagraph()
            }).ToList();
        }
    }
}