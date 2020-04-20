using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyCiteLib.Repository;

namespace EasyCiteLib.Models.Search.Export
{
    public static class BibFileHelpers
    {
        public static void Entry(this StringBuilder stringBuilder, string entryType, Document document, ref Dictionary<string, int> keys)
        {
            // Generate id
            var id = string.Join('+', document.Authors.Select(a => a.LastName));

            if(keys.ContainsKey(id) == false)
                keys[id] = 0;
            else
                id += keys[id].ToString();

            keys[id] += 1;

            // Generate entry
            stringBuilder.AppendLine($"@{entryType}{{{id},");

            // Generate all fields
            stringBuilder.Field(FieldTypes.Title, document.Title);

            var authors = string.Join(" and ", document.Authors.Select(a => a.Name.ToLowerInvariant())); // Get authors
            stringBuilder.Field(FieldTypes.Author, authors);
            stringBuilder.Field(FieldTypes.Month, document.PublishDate.ToString("MMMM")); // Full month
            stringBuilder.Field(FieldTypes.Year, document.PublishDate.ToString("yyyy")); // Full year

            // Add other information to citation
            var note = new StringBuilder();
            note.AppendLine($"Publication Title: {document.PublicationTitle}");
            note.Append($"IEEE Link: https://ieeexplore.ieee.org/document/{document.Id}");

            stringBuilder.Field(FieldTypes.Note, note.ToString());

            // Add closing tag
            stringBuilder.AppendLine("}");

            // Final newline for formatting's sake
            stringBuilder.AppendLine();
        }

        public static void Field(this StringBuilder stringBuilder, string fieldType, string value)
        {
            stringBuilder.AppendLine($"\t{fieldType} = \"{value}\",");
        }
    }
}