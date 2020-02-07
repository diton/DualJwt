using System.Text.RegularExpressions;

namespace DualJwt.Security
{
    public static class WildCardToRegexHelper
    {
        public static string WildCardToRegex(this string value)
        {
            return "^" + Regex.Escape(value.ToLower()).Replace("\\?", ".").Replace("\\*", ".*") + "$";
        }
    }
}
