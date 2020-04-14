using System.Collections.Generic;

namespace EasyCiteLib.Repository.EasyCite
{
    public enum SourceEnum
    {
        IEEE
    }
    public class Source
    {
        public SourceEnum Id { get; set; }
        public string Name { get; set; }
        public List<ReferenceSource> ReferenceSources { get; set; } = new List<ReferenceSource>();
    }
}