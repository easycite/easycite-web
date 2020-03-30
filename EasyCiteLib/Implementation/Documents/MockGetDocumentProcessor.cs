using System.Collections.Generic;
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
        public async IAsyncEnumerable<Document> GetDocumentsAsync(IEnumerable<string> documentIds)
        {
            foreach (string documentId in documentIds)
            {
                yield return new Document
                {
                    Id = documentId,
                    Title = _getRandomDataProcessor.GetText(maxWords: 9),
                    Author = new Author
                    {
                        Name = _getRandomDataProcessor.GetName()
                    },
                    Conference = new Conference
                    {
                        Name = _getRandomDataProcessor.GetText(maxWords: 4)
                    },
                    Abstract = _getRandomDataProcessor.GetParagraph()
                };
            }
        }
    }
}