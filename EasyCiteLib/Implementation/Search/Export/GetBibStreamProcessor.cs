using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyCiteLib.Interface.Documents;
using EasyCiteLib.Interface.Projects;
using EasyCiteLib.Interface.Search.Export;
using EasyCiteLib.Models;
using EasyCiteLib.Models.Search.Export;

namespace EasyCiteLib.Implementation.Search.Export
{
    public class GetBibStreamProcessor : IGetBibStreamProcessor
    {
        private readonly IGetDocumentProcessor _getDocumentProcessor;
        private readonly IGetProjectForUserProcessor _getProjectForUserProcessor;

        private Dictionary<string, int> Keys = new Dictionary<string, int>();

        public GetBibStreamProcessor(
            IGetDocumentProcessor getDocumentProcessor,
            IGetProjectForUserProcessor getProjectForUserProcessor)
        {
            _getDocumentProcessor = getDocumentProcessor;
            _getProjectForUserProcessor = getProjectForUserProcessor;
        }
        public async Task<Results<FileData>> GetAsync(int projectId, string filename = null)
        {
            var results = new Results<FileData>();
            var projectResults = await _getProjectForUserProcessor.GetAsync(projectId);

            if (projectResults.HasProblem)
            {
                results.Merge(projectResults);
                return results;
            }

            var documentIds = projectResults.Data.ProjectReferences
                .Select(pr => pr.ReferenceId);

            // Actually get documents
            var bibFile = new StringBuilder();
            foreach (var doc in await _getDocumentProcessor.GetDocumentsAsync(documentIds))
                bibFile.Entry(EntryTypes.Misc, doc, ref Keys);
            
            results.Data.Content = Encoding.UTF8.GetBytes(bibFile.ToString());
            results.Data.Filename = string.IsNullOrWhiteSpace(filename) ? $"{projectResults.Data.Name}-references" : filename;
            results.Data.Filename += ".bib";
            return results;
        }
    }
}