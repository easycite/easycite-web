namespace EasyCiteLib.Models.Search
{
    public class ResultVm
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string PublishDate { get; set; }
        public string AuthorName { get; set; }
        public string Conference { get; set; }
        public string Abstract { get; set; }
        public int CiteCount { get; set; }
    }
}
