namespace EasyCiteLib.Repository.EasyCite
{
    public class ProjectHiddenResult
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }
        public string ReferenceId { get; set; }
    }
}