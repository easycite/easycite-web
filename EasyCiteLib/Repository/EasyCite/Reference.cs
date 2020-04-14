using System;
using System.Collections.Generic;

namespace EasyCiteLib.Repository.EasyCite
{
    public class Reference
    {
        public int Id { get; set; }
        public string Title { get; set; }
        
        public string PublicationTitle { get; set; }
        public DateTime PublicationDate { get; set; }

        public List<ProjectReference> ProjectReferences { get; set; } = new List<ProjectReference>();
        public List<ReferenceSource> Sources { get; set; } = new List<ReferenceSource>();
    }
}