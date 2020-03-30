using System.Collections.Generic;
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

        public async Task<Results<bool>> AddAsync(int projectId, string documentId)
        {
            var results = new Results<bool>();
            
            _context.DocumentIds.Add(documentId);

            return results;
        }

        public async Task<Results<bool>> RemoveAsync(int projectId, string documentId)
        {
            var results = new Results<bool>();
            
            _context.DocumentIds.Remove(documentId);

            return results;
        }

        public async Task<Results<List<ReferenceVm>>> GetAllAsync(int projectId)
        {
            var results = new Results<List<ReferenceVm>>
            {
                Data = new List<ReferenceVm>()
            };

            var documentsEnumerable = _getDocumentProcessor.GetDocumentsAsync(_context.DocumentIds);

            var list = new List<ReferenceVm>();

            await foreach (var document in documentsEnumerable)
            {
                results.Data.Add(new ReferenceVm
                {
                    Id = document.Id,
                    Title = document.Title,
                    Abstract = document.Abstract
                });
            }

            return results;
        }
    }
}