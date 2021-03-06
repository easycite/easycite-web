using System.ComponentModel;

namespace EasyCiteLib.Repository.EasyCite
{
    public class ProjectReference
    {
        public int Id { get; set; }
        
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }

        public string ReferenceId { get; set; }

        public bool IsPending { get; set; }
    }
}