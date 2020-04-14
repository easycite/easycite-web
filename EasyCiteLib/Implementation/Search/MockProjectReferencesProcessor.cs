using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyCiteLib.Interface.Documents;
using EasyCiteLib.Interface.Search;
using EasyCiteLib.Models;
using EasyCiteLib.Models.Search;
using EasyCiteLib.Repository;

namespace EasyCiteLib.Implementation.Search
{
    public class MockProjectReferencesProcessor : IProjectReferencesProcessor
    {
        private readonly IGetDocumentProcessor _getDocumentProcessor;
        private readonly ProjectReferencesContext _context;

        public MockProjectReferencesProcessor(
            IGetDocumentProcessor getDocumentProcessor,
            ProjectReferencesContext context)
        {
            _getDocumentProcessor = getDocumentProcessor;
            _context = context;
        }

        public Task<Results<List<string>>> AddAsync(int projectId, IEnumerable<string> documentIds)
        {
            var results = new Results<List<string>>();

            List<string> documentsToAdd = documentIds.Except(_context.DocumentIds).ToList();
            _context.DocumentIds.AddRange(documentsToAdd);
            results.Data = documentsToAdd;

            return Task.FromResult(results);
        }

        public async Task<Results<bool>> RemoveAsync(int projectId, string documentId)
        {
            var results = new Results<bool>();
            
            _context.DocumentIds.Remove(documentId);

            return results;
        }

        public async Task<Results<List<ReferenceVm>>> GetAllAsync(int projectId)
        {
            var documents = await _getDocumentProcessor.GetDocumentsAsync(_context.DocumentIds);

            return new Results<List<ReferenceVm>>
            {
                Data = documents.Select(document => new ReferenceVm
                {
                    Id = document.Id,
                    Title = document.Title,
                    Abstract = document.Abstract,
                    IsPending = false
                }).ToList()
            };
        }

        public async Task<Results<List<ReferenceVm>>> GetCompletedScrapesAsync(int projectId, IEnumerable<string> documentIds)
        {
            return new Results<List<ReferenceVm>>
            {
                Data = new List<ReferenceVm>()
            };
        }
    }
}