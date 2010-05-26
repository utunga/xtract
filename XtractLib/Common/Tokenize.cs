using System.Text.RegularExpressions;

namespace XtractLib.Common
{
    public static class Tokenize
    {
        // Methods
        public static string[] Words(string source)
        {
            return Regex.Split(source, @"\W+");
        }
    }
}