using System.Threading.Tasks;
using EasyCiteLib.Interface.Documents;
using EasyCiteLib.Interface.Projects;
using EasyCiteLib.Interface.Search.Export;
using EasyCiteLib.Models;
using EasyCiteLib.Models.Search.Export;

namespace EasyCiteLib.Implementation.Search.Export
{
    public class GetSearchExportDataProcessor : IGetSearchExportDataProcessor
    {
        private readonly IGetProjectForUserProcessor _getProjectForUserProcessor;
        private readonly IDocumentSearchProcessor _documentSearchProcessor;

        public GetSearchExportDataProcessor(IGetProjectForUserProcessor getProjectForUserProcessor,
            IDocumentSearchProcessor documentSearchProcessor)
        {
            _getProjectForUserProcessor = getProjectForUserProcessor;
            _documentSearchProcessor = documentSearchProcessor;
        }
        public async Task<Results<ExportData>> GetAsync(int projectId)
        {
            var results = new Results<ExportData>();
            var projectResults = await _getProjectForUserProcessor.GetAsync(projectId);

            if(projectResults.HasProblem) {
                results.Merge(projectResults);
                return results;
            }

            foreach (var reference in projectResults.Data.ProjectReferences)
                results.Data.RichTextCitations.Add(await _documentSearchProcessor.GetPlainTextCitationAsync(reference.ReferenceId));
            
            return results;
        }
    }
}