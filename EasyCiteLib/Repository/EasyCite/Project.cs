using System.Collections.Generic;

namespace EasyCiteLib.Repository.EasyCite
{
    public class Project
    {

        public int Id { get; set; }
        public string Name { get; set; }


        public int UserId { get; set; }
        public virtual User User { get; set; }

        public virtual List<ProjectReference> ProjectReferences { get; set; } = new List<ProjectReference>();
        public virtual List<ProjectHiddenResult> ProjectHiddenResults { get; set; } = new List<ProjectHiddenResult>();
    }
}