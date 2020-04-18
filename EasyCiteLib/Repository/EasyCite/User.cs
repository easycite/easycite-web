using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EasyCiteLib.Repository.EasyCite
{
    public class User
    {
        public int Id { get; set; }
        
        [Required] public string GoogleIdentifier { get; set; }
        [Required] public string Firstname { get; set; }
        [Required] public string Lastname { get; set; }
        [Required] public string Email { get; set; }
        

        public string FirstnameLastname => $"{Firstname} {Lastname}";
        public string LastnameFirstname => $"{Lastname}, {Firstname}";

        public virtual List<Project> Projects { get; set; } = new List<Project>();
    }
}