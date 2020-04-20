using System.Collections.Generic;
using System.Linq;

namespace EasyCiteLib.Models.Search.Export.Citations
{
    public class ApaCitation : Citation
    {
        public override string ToString()
        {
            List<string> citationParts = new List<string>();


            citationParts.Add(string.Join(" & ", Authors.Select(a => a.LastNameFirstMiddleInitial)));
            citationParts.Add($"({PublicationDate.ToString("yyyy")}).");
            citationParts.Add(Title + '.');
            string publicationTitle = $"<em>{PublicationTitle}";
            if (string.IsNullOrWhiteSpace(VolumeNumber) == false && string.IsNullOrWhiteSpace(IssueNumber) == false)
                publicationTitle += $" {VolumeNumber}({IssueNumber})";
            publicationTitle += ",</em>";

            string pages = "";
            if (string.IsNullOrWhiteSpace(FirstPage) == false)
            {
                pages += FirstPage;
                if (string.IsNullOrWhiteSpace(LastPage) == false)
                    pages += $"-{LastPage}";
                pages += '.';
            }
            return "";
        }
    }
}