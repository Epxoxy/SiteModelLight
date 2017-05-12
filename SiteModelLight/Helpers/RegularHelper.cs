using System.Text.RegularExpressions;

namespace SiteModelLight.Helpers
{
    public static class RegularHelper
    {
        public static string clearHTMLHeadBody(this string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            string result = string.Empty;
            result = Regex.Replace(input, @"<![\s\S]*?-->", string.Empty);
            result = Regex.Replace(result, @"<script[\s\S]*?script>", string.Empty);
            result = Regex.Replace(result, @"<html[\s\S]*?<body>", string.Empty);
            result = Regex.Replace(result, @">\s+<", "><");
            return result;
        }

        public static string clearHTMLHead(this string input)
        {
            string result = string.Empty;
            result = Regex.Replace(input, @"<![\s\S]*?-->", string.Empty);
            result = Regex.Replace(result, @"<script[\s\S]*?</script>", string.Empty);
            result = Regex.Replace(result, @">\s+<", "><");
            return result;
        }

        public static string clearEmptyLine(this string input)
        {
            return Regex.Replace(input, "\n+\n", " ");
        }

        public static string clearGtLt(this string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            string result = string.Empty;
            result = Regex.Replace(input, @"<.+?>", "\n");
            result = Regex.Replace(result, @"&nbsp;", string.Empty);
            return result;
        }

        public static MatchCollection Matches(this string input, string pattern)
        {
            Regex r = new Regex(pattern);
            return r.Matches(input);
        }
    }
}
