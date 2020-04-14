namespace EasyCiteLib.Repository.EasyCite
{
    public class ProjectReference
    {
        public int Id { get; set; }
        
        public int ProjectId { get; set; }
        public Project Project { get; set; }

        public string ReferenceId { get; set; }
    }
}