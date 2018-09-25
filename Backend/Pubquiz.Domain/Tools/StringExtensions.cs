namespace Pubquiz.Domain.Tools
{
    public static class StringExtensions
    {
        public static string ReplaceSpaces(this string input)
        {
            return input.Replace(" ", "%20");
        }
    }}