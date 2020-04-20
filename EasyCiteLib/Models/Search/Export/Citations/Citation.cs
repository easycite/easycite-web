using System;
using System.Collections.Generic;

namespace EasyCiteLib.Models.Search.Export.Citations
{
    public abstract class Citation
    {
        public string Title { get; set; }
        public string PublicationTitle { get; set; }
        public DateTime PublicationDate { get; set; }
        public List<CitationAuthor> Authors { get; set; } = new List<CitationAuthor>();
        public string DOI { get; set; }

        public string VolumeNumber { get; set; }
        public string IssueNumber { get; set; }

        public string FirstPage { get; set; }
        public string LastPage { get; set; }

        public abstract override string ToString();
    }
}