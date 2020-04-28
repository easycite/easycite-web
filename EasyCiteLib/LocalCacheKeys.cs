namespace EasyCiteLib
{
    public static class LocalCacheKeys
    {
        public static string ProjectSearchResults(int projectId) => $"project-search-results:{projectId}";

        public static string Document(string documentId) => $"document:{documentId}";
    }
}