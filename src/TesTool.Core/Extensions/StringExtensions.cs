using System;
using System.Linq;
using System.Text;

namespace TesTool.Core.Extensions
{
    public static class StringExtensions
    {
        public static string ToSnakeCase(this string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return default;
            if (text.Length < 2) return text;
            
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(char.ToLowerInvariant(text[0]));
            for (int index = 1; index < text.Length; ++index)
            {
                char character = text[index];
                if (char.IsUpper(character))
                {
                    stringBuilder.Append('_');
                    stringBuilder.Append(char.ToLowerInvariant(character));
                    continue;
                }
                stringBuilder.Append(character);
            }
            return stringBuilder.ToString();
        }

        public static string ToLowerCaseFirst(this string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;

            char[] chars = text.ToCharArray();
            chars[0] = char.ToLower(chars[0]);

            return new string(chars);
        }
    }
}
