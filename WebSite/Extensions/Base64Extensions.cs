using System.Text;

namespace WebSite.Extensions
{
    public static class Base64Extensions
    {
        public static string ToUrlSafeBase64(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            var bytes = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(bytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('=');
        }

        public static string FromUrlSafeBase64(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            // Restore padding
            var padded = input.PadRight((input.Length + 3) & ~3, '=');

            // Restore standard Base64 characters
            var standard = padded.Replace('-', '+').Replace('_', '/');

            var bytes = Convert.FromBase64String(standard);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}