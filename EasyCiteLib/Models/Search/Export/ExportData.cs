using System.Collections.Generic;
using EasyCiteLib.Models.Search.Export.Citations;

namespace EasyCiteLib.Models.Search.Export
{
    public class ExportData
    {
        public List<ApaCitation> ApaCitations { get; set; } = new List<ApaCitation>();
    }
}