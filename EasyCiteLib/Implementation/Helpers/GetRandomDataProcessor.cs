using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using EasyCiteLib.Interface.Helpers;

namespace EasyCiteLib.Implementation.Helpers
{
    public class GetRandomDataProcessor : IGetRandomDataProcessor
    {
        private List<string> _loremIpsum = new List<string>
        {
            "lorem", "ipsum", "dolor", "sit", "amet", "consectetuer",
            "adipiscing", "elit", "sed", "diam", "nonummy", "nibh", "euismod",
            "tincidunt", "ut", "laoreet", "dolore", "magna", "aliquam", "erat"
        };

        private List<string> _firstNames = new List<string> { "Lewis", "Jack", "Ryan", "James", "Callum", "Cameron", "Daniel", "Liam", "Jamie", "Kyle", "Matthew", "Logan", "Finlay", "Adam", "Alexander", "Dylan", "Aiden", "Andrew", "Ben", "Aaron", "Connor", "Thomas", "Joshua", "David", "Ross", "Luke", "Nathan", "Charlie", "Ethan", "Aidan", "Michael", "John", "Calum", "Scott", "Josh", "Samuel", "Kieran", "Fraser", "William", "Oliver", "Rhys", "Sean", "Harry", "Owen", "Sam", "Christopher", "Euan", "Robert", "Kai", "Jay", "Jake", "Lucas", "Jayden", "Tyler", "Rory", "Reece", "Robbie", "Joseph", "Max", "Benjamin", "Ewan", "Archie", "Evan", "Leo", "Taylor", "Alfie", "Blair", "Arran", "Leon", "Angus", "Craig", "Murray", "Declan", "Zak", "Brandon", "Harris", "Finn", "Lee", "Lennon", "Cole", "George", "Jacob", "Mark", "Hayden", "Kenzie", "Alex", "Shaun", "Louis", "Caleb", "Mason", "Gregor", "Mohammed", "Luca", "Harrison", "Kian", "Noah", "Paul", "Riley", "Stuart", "Joe", "Jonathan", "Stephen", "Brodie", "Marcus", "Mackenzie", "Bailey", "Corey", "Hamish", "Dean", "Muhammad", "Alistair", "Elliot", "Jason", "Steven", "Charles", "Oscar", "Peter", "Calvin", "Cody", "Findlay", "Isaac", "Harvey", "Patrick", "Ashton", "Ciaran", "Duncan", "Fergus", "Conor", "Darren", "Alasdair", "Anthony", "Kayden", "Kerr", "Sonny", "Jordan", "Blake", "Gabriel", "Lachlan", "Bradley", "Gary", "Jude", "Kevin", "Kaiden", "Tom", "Zach", "Ellis", "Keiran", "Mitchell", "Alan", "Billy", "Edward", "Kyran", "Mohammad", "Nicholas", "Dominic", "Iain", "Toby", "Campbell", "Joel", "Keir", "Shay", "Struan", "Danny", "Ronan", "Zack", "Innes", "Reuben", "Ruaridh", "Cayden", "Jakub", "Marc", "Martin", "Zac", "Cian", "Flynn", "Gavin", "Ian", "Mikey", "Theo", "Caiden", "McKenzie", "Cooper", "Douglas", "Harley", "Morgan", "Zander", "Alastair", "Bruce", "Grant", "Jackson", "Ruairidh", "Ali", "Allan", "Brian", "Henry", "Rocco", "Colin", "Levi", "Nathaniel", "Neil", "Rohan", "Callan", "Greg", "Kacper", "Rowan", "Shane", "Tony", "Barry", "Maxwell", "Rio", "Brendan", "Layton", "Niall", "Warren", "Bobby", "Drew", "Felix", "Freddie", "Justin", "Kris", "Magnus" };
        private List<string> _lastNames = new List<string> { "Smith", "Johnson", "Williams", "Brown", "Jones", "Miller", "Davis", "Garcia", "Rodriguez", "Wilson", "Martinez", "Anderson", "Taylor", "Thomas", "Hernandez", "Moore", "Martin", "Jackson", "Thompson", "White", "Lopez", "Lee", "Gonzalez", "Harris", "Clark", "Lewis", "Robinson", "Walker", "Perez", "Hall", "Young", "Allen", "Sanchez", "Wright", "King", "Scott", "Green", "Baker", "Adams", "Nelson", "Hill", "Ramirez", "Campbell", "Mitchell", "Roberts", "Carter", "Phillips", "Evans", "Turner", "Torres", "Parker", "Collins", "Edwards", "Stewart", "Flores", "Morris", "Nguyen", "Murphy", "Rivera", "Cook", "Rogers", "Morgan", "Peterson", "Cooper", "Reed", "Bailey", "Bell", "Gomez", "Kelly", "Howard", "Ward", "Cox", "Diaz", "Richardson", "Wood", "Watson", "Brooks", "Bennett", "Gray", "James", "Reyes", "Cruz", "Hughes", "Price", "Myers", "Long", "Foster", "Sanders", "Ross", "Morales", "Powell", "Sullivan", "Russell", "Ortiz", "Jenkins", "Gutierrez", "Perry", "Butler", "Barnes", "Fisher", "Henderson", "Coleman", "Simmons", "Patterson", "Jordan", "Reynolds", "Hamilton", "Graham", "Kim", "Gonzales", "Alexander", "Ramos", "Wallace", "Griffin", "West", "Cole", "Hayes", "Chavez", "Gibson", "Bryant", "Ellis", "Stevens", "Murray", "Ford", "Marshall", "Owens", "Mcdonald", "Harrison", "Ruiz", "Kennedy", "Wells", "Alvarez", "Woods", "Mendoza", "Castillo", "Olson", "Webb", "Washington", "Tucker", "Freeman", "Burns", "Henry", "Vasquez", "Snyder", "Simpson", "Crawford", "Jimenez", "Porter", "Mason", "Shaw", "Gordon", "Wagner", "Hunter", "Romero", "Hicks", "Dixon", "Hunt", "Palmer", "Robertson", "Black", "Holmes", "Stone", "Meyer", "Boyd", "Mills", "Warren", "Fox", "Rose", "Rice", "Moreno", "Schmidt", "Patel", "Ferguson", "Nichols", "Herrera", "Medina", "Ryan", "Fernandez", "Weaver", "Daniels", "Stephens", "Gardner", "Payne", "Kelley", "Dunn", "Pierce", "Arnold", "Tran", "Spencer", "Peters", "Hawkins", "Grant", "Hansen", "Castro", "Hoffman", "Hart", "Elliott", "Cunningham", "Knight", "Bradley", "Carroll", "Hudson", "Duncan", "Armstrong", "Berry", "Andrews", "Johnston", "Ray", "Lane", "Riley", "Carpenter", "Perkins", "Aguilar", "Silva", "Richards", "Willis", "Matthews", "Chapman", "Lawrence", "Garza", "Vargas", "Watkins", "Wheeler", "Larson", "Carlson", "Harper", "George", "Greene", "Burke", "Guzman", "Morrison", "Munoz", "Jacobs", "Obrien", "Lawson", "Franklin", "Lynch", "Bishop", "Carr", "Salazar", "Austin", "Mendez", "Gilbert", "Jensen", "Williamson", "Montgomery", "Harvey", "Oliver", "Howell", "Dean", "Hanson", "Weber", "Garrett", "Sims", "Burton", "Fuller", "Soto", "Mccoy", "Welch", "Chen", "Schultz", "Walters", "Reid", "Fields", "Walsh", "Little", "Fowler", "Bowman", "Davidson", "May", "Day", "Schneider", "Newman", "Brewer", "Lucas", "Holland", "Wong", "Banks", "Santos", "Curtis", "Pearson", "Delgado", "Valdez", "Pena", "Rios", "Douglas", "Sandoval", "Barrett", "Hopkins", "Keller", "Guerrero", "Stanley", "Bates", "Alvarado", "Beck", "Ortega", "Wade", "Estrada", "Contreras", "Barnett", "Caldwell", "Santiago", "Lambert", "Powers", "Chambers", "Nunez", "Craig", "Leonard", "Lowe", "Rhodes", "Byrd", "Gregory", "Shelton", "Frazier", "Becker", "Maldonado", "Fleming", "Vega", "Sutton", "Cohen", "Jennings", "Parks", "Mcdaniel", "Watts", "Barker", "Norris", "Vaughn", "Vazquez", "Holt", "Schwartz", "Steele", "Benson", "Neal", "Dominguez", "Horton", "Terry", "Wolfe", "Hale", "Lyons", "Graves", "Haynes", "Miles", "Park", "Warner", "Padilla", "Bush", "Thornton", "Mccarthy", "Mann", "Zimmerman", "Erickson", "Fletcher", "Mckinney", "Page", "Dawson", "Joseph", "Marquez", "Reeves", "Klein", "Espinoza", "Baldwin", "Moran", "Love", "Robbins", "Higgins", "Ball", "Cortez", "Le", "Griffith", "Bowen", "Sharp", "Cummings", "Ramsey", "Hardy", "Swanson", "Barber", "Acosta", "Luna", "Chandler", "Blair", "Daniel", "Cross", "Simon", "Dennis", "Oconnor", "Quinn", "Gross", "Navarro", "Moss", "Fitzgerald", "Doyle", "Mclaughlin", "Rojas", "Rodgers", "Stevenson", "Singh", "Yang", "Figueroa", "Harmon", "Newton", "Paul", "Manning", "Garner", "Mcgee", "Reese", "Francis", "Burgess", "Adkins", "Goodman", "Curry", "Brady", "Christensen", "Potter", "Walton", "Goodwin", "Mullins", "Molina", "Webster", "Fischer", "Campos", "Avila", "Sherman", "Todd", "Chang", "Blake", "Malone", "Wolf", "Hodges", "Juarez", "Gill", "Farmer", "Hines", "Gallagher", "Duran", "Hubbard", "Cannon", "Miranda", "Wang", "Saunders", "Tate", "Mack", "Hammond", "Carrillo", "Townsend", "Wise", "Ingram", "Barton", "Mejia", "Ayala", "Schroeder", "Hampton", "Rowe", "Parsons", "Frank", "Waters", "Strickland", "Osborne", "Maxwell", "Chan", "Deleon", "Norman", "Harrington", "Casey", "Patton", "Logan", "Bowers", "Mueller", "Glover", "Floyd", "Hartman", "Buchanan", "Cobb", "French", "Kramer", "Mccormick", "Clarke", "Tyler", "Gibbs", "Moody", "Conner", "Sparks", "Mcguire", "Leon", "Bauer", "Norton", "Pope", "Flynn", "Hogan", "Robles", "Salinas", "Yates", "Lindsey", "Lloyd", "Marsh", "Mcbride", "Owen", "Solis", "Pham", "Lang", "Pratt", "Lara", "Brock", "Ballard", "Trujillo", "Shaffer", "Drake", "Roman", "Aguirre", "Morton", "Stokes", "Lamb", "Pacheco", "Patrick", "Cochran", "Shepherd", "Cain", "Burnett", "Hess", "Li", "Cervantes", "Olsen", "Briggs", "Ochoa", "Cabrera", "Velasquez", "Montoya", "Roth", "Meyers", "Cardenas", "Fuentes", "Weiss", "Hoover", "Wilkins", "Nicholson", "Underwood", "Short", "Carson", "Morrow", "Colon", "Holloway", "Summers", "Bryan", "Petersen", "Mckenzie", "Serrano", "Wilcox", "Carey", "Clayton", "Poole", "Calderon", "Gallegos", "Greer", "Rivas", "Guerra", "Decker", "Collier", "Wall", "Whitaker", "Bass", "Flowers", "Davenport", "Conley", "Houston", "Huff", "Copeland", "Hood", "Monroe", "Massey", "Roberson", "Combs", "Franco", "Larsen", "Pittman", "Randall", "Skinner", "Wilkinson", "Kirby", "Cameron", "Bridges", "Anthony", "Richard", "Kirk", "Bruce", "Singleton", "Mathis", "Bradford", "Boone", "Abbott", "Charles", "Allison", "Sweeney", "Atkinson", "Horn", "Jefferson", "Rosales", "York", "Christian", "Phelps", "Farrell", "Castaneda", "Nash", "Dickerson", "Bond", "Wyatt", "Foley", "Chase", "Gates", "Vincent", "Mathews", "Hodge", "Garrison", "Trevino", "Villarreal", "Heath", "Dalton", "Valencia", "Callahan", "Hensley", "Atkins", "Huffman", "Roy", "Boyer", "Shields", "Lin", "Hancock", "Grimes", "Glenn", "Cline", "Delacruz", "Camacho", "Dillon", "Parrish", "Oneill", "Melton", "Booth", "Kane", "Berg", "Harrell", "Pitts", "Savage", "Wiggins", "Brennan", "Salas", "Marks", "Russo", "Sawyer", "Baxter", "Golden", "Hutchinson", "Liu", "Walter", "Mcdowell", "Wiley", "Rich", "Humphrey", "Johns", "Koch", "Suarez", "Hobbs", "Beard", "Gilmore", "Ibarra", "Keith", "Macias", "Khan", "Andrade", "Ware", "Stephenson", "Henson", "Wilkerson", "Dyer", "Mcclure", "Blackwell", "Mercado", "Tanner", "Eaton", "Clay", "Barron", "Beasley", "Oneal", "Preston", "Small", "Wu", "Zamora", "Macdonald", "Vance", "Snow", "Mcclain", "Stafford", "Orozco", "Barry", "English", "Shannon", "Kline", "Jacobson", "Woodard", "Huang", "Kemp", "Mosley", "Prince", "Merritt", "Hurst", "Villanueva", "Roach", "Nolan", "Lam", "Yoder", "Mccullough", "Lester", "Santana", "Valenzuela", "Winters", "Barrera", "Leach", "Orr", "Berger", "Mckee", "Strong", "Conway", "Stein", "Whitehead", "Bullock", "Escobar", "Knox", "Meadows", "Solomon", "Velez", "Odonnell", "Kerr", "Stout", "Blankenship", "Browning", "Kent", "Lozano", "Bartlett", "Pruitt", "Buck", "Barr", "Gaines", "Durham", "Gentry", "Mcintyre", "Sloan", "Rocha", "Melendez", "Herman", "Sexton", "Moon", "Hendricks", "Rangel", "Stark", "Lowery", "Hardin", "Hull", "Sellers", "Ellison", "Calhoun", "Gillespie", "Mora", "Knapp", "Mccall", "Morse", "Dorsey", "Weeks", "Nielsen", "Livingston", "Leblanc", "Mclean", "Bradshaw", "Glass", "Middleton", "Buckley", "Schaefer", "Frost", "Howe", "House", "Mcintosh", "Ho", "Pennington", "Reilly", "Hebert", "Mcfarland", "Hickman", "Noble", "Spears", "Conrad", "Arias", "Galvan", "Velazquez", "Huynh", "Frederick", "Randolph", "Cantu", "Fitzpatrick", "Mahoney", "Peck", "Villa", "Michael", "Donovan", "Mcconnell", "Walls", "Boyle", "Mayer", "Zuniga", "Giles", "Pineda", "Pace", "Hurley", "Mays" };

        private Random _random = new Random();

        public DateTime GetDate(DateTime? minDate = null, DateTime? maxDate = null)
        {
            var start = minDate ?? new DateTime(1995, 1, 1);
            var end = maxDate ?? DateTime.Today;

            return start.AddDays(_random.Next((end - start).Days));
        }

        public int GetInt(int min = 1, int max = 1000) => _random.Next(min, max);

        public double GetFraction() => _random.NextDouble();

        
        public string GetText(int minWords = 1, int maxWords = 1, int minSentences = 0, int maxSentences = 0, int numParagraphs = 0)
        {
            if (maxWords <= 0 || minWords < 0) return "";

            int numSentences = _random.Next(minSentences, maxSentences) + minSentences;
            int numWords = _random.Next(minWords, maxWords) + minWords + 1;

            var result = new StringBuilder();
            if (numParagraphs > 0)
                for (int i = 0; i < numParagraphs; i++)
                {
                    for (int j = 0; j < numSentences; j++)
                    {
                        for (int k = 0; k < numWords; k++)
                        {
                            if (k > 0) result.Append(" ");

                            var word = _loremIpsum[_random.Next(_loremIpsum.Count)];

                            if (k == 0) word = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(word);

                            result.Append(word);
                        }

                        result.Append(". ");
                    }

                    result.AppendLine();
                }
            else if(numSentences > 0)
                for (int j = 0; j < numSentences; j++)
                {
                    for (int k = 0; k < numWords; k++)
                    {
                        if (k > 0) result.Append(" ");

                        var word = _loremIpsum[_random.Next(_loremIpsum.Count)];

                        if (k == 0) word = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(word);

                        result.Append(word);
                    }

                    result.Append(". ");
                }
            else if (numWords > 1)
                for (int k = 0; k < numWords; k++)
                {
                    if (k > 0) result.Append(" ");

                    var word = _loremIpsum[_random.Next(_loremIpsum.Count)];

                    if (k == 0) word = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(word);

                    result.Append(word);
                }
            else
                result.Append(_loremIpsum[_random.Next(_loremIpsum.Count)]);

            return result.ToString();
        }
        public string GetParagraph() => GetText(5, 12, 3, 5, 1);
        public string GetName() => GetFirstName() + " " + GetLastName();

        public string GetFirstName() => _firstNames[_random.Next(_firstNames.Count)];

        public string GetLastName() => _lastNames[_random.Next(_lastNames.Count)];
    }
}
