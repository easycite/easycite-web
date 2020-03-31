using System;

namespace EasyCiteLib.Interface.Helpers
{
    public interface IGetRandomDataProcessor
    {
        DateTime GetDate(DateTime? minDate = null, DateTime? maxDate = null);

        int GetInt(int min = 1, int max = 1000);

        double GetFraction();

        string GetText(int minWords = 1, int maxWords = 1, int minSentences = 0, int maxSentences = 0, int numParagraphs = 0);
        string GetParagraph();


        string GetName();
        string GetFirstName();
        string GetLastName();
    }
}