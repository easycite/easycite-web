using System.Collections.Generic;

namespace EasyCiteLib.Repository
{
    public class ProjectReferencesContext
    {
        public ProjectReferencesContext()
        {
        }
        public List<string> DocumentIds { get; set; } = new List<string> {
            "asdf",
            "qwer",
            "zxcv",
        };
    }
}