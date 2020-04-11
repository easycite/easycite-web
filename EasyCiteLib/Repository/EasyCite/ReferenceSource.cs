using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EasyCiteLib.Repository.EasyCite
{
    public class ReferenceSource
    {
        public int Id { get; set; }
        public string SourceDocumentId { get; set; }
        public int ReferenceId { get; set; }
        public Reference Reference { get; set; }

        public int SourceId { get; set; }
        public Source Source { get; set; }
    }
}