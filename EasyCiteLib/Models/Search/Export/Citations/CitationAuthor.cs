namespace EasyCiteLib.Models.Search.Export.Citations
{
    public class CitationAuthor
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string FirstLastName => $"{FirstName} {LastName}";
        public string LastFirstName => $"{LastName}, {FirstName}";
        public string LastNameFirstMiddleInitial
        {
            get
            {
                var ret = LastName;

                if (string.IsNullOrWhiteSpace(FirstName) == false)
                    ret += $", {FirstName[0]}.";

                if (string.IsNullOrWhiteSpace(MiddleName) == false)
                    ret += $" {MiddleName.ToUpper()[0]}.";

                return ret;
            }
        }
    }
}