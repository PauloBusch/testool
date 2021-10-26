using System.Text.RegularExpressions;

namespace TesTool.Core.Extensions
{
    public static class RegexExtensions
    {
        public static string ReplaceValue(this Group group, string text, string value)
        {
            var left = text.Substring(0, group.Index);
            var right = text.Substring(group.Index + group.Length);
            return $"{left}{value}{right}";
        }
    }
}
