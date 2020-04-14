using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace EasyCiteLib.Interface.Documents
{
    public interface IBibFileProcessor
    {
        Task<IEnumerable<string>> GetTitlesAsync(string bibFileContent);
    }
}