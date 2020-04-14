using System.ComponentModel;

namespace EasyCiteLib.Models.Search
{
    public enum SearchSortType
    {
        [Description("Most Relevant")]
        PageRank = 0,
        Recency,
        [Description("Author Popularity")]
        AuthorPopularity
    }
}