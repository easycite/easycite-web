namespace EasyCiteLib.Models
{
    public class DropdownOption<T>
    {
        public T Value { get; set; }
        public string Text { get; set; }
        public string HelpText { get; set; }
    }
}