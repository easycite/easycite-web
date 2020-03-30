namespace EasyCiteLib.Repository
{
    public class Document
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Abstract { get; set; }
        public Author Author { get; set; }
        public Conference Conference { get; set; }
    }
}