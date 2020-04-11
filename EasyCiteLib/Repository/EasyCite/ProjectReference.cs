namespace EasyCiteLib.Repository.EasyCite
{
    public class ProjectReference
    {
        public int Id { get; set; }
        
        public int ProjectId { get; set; }
        public Project Project { get; set; }

        public int ReferenceId { get; set; }
        public Reference Reference { get; set; }
    }
}