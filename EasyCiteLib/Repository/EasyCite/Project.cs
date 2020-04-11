using System.Collections.Generic;

namespace EasyCiteLib.Repository.EasyCite
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }


        public int UserId { get; set; }
        public User User { get; set; }

        public ICollection<ProjectReference> ProjectReferences { get; set; } = new HashSet<ProjectReference>();
    }
}