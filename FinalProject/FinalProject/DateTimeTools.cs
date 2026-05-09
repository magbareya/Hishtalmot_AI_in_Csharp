using System.Globalization;

namespace AIChat
{
    public class DateTimeTools
    {
        public string GetDate()
        {
            return DateTime.Now.ToString("dddd, MMMM dd, yyyy", CultureInfo.GetCultureInfo("en-US"));
        }

        public string GetTime()
        {
            return DateTime.Now.ToString("HH:mm:ss");
        }
    }
}
