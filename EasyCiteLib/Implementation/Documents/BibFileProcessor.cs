using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EasyCiteLib.Interface.Documents;

namespace EasyCiteLib.Implementation.Documents
{
    public class BibFileProcessor : IBibFileProcessor
    {
        static readonly Regex _titleRegex = new Regex(@"(?:@[a-z]+{[\s\S]*?\stitle\s*=\s*{([^}]+)})", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
        
        public Task<IEnumerable<string>> GetTitlesAsync(string bibFileContent)
        {
            MatchCollection matches = _titleRegex.Matches(bibFileContent);

            return Task.FromResult(matches.Select(m => m.Groups[1].Value));
        }
    }
}