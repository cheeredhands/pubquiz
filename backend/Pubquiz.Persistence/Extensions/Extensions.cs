using System;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

namespace Pubquiz.Persistence.Extensions
{
    public static class Extensions
    {
        /// <summary>
        ///     Deep clone using JSON serialization.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toClone"></param>
        /// <returns></returns>
        public static T Clone<T>(this T toClone) where T : class =>
            JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(toClone));

        public static string ToShortGuidString(this Guid guid)
        {
            string enc = Convert.ToBase64String(guid.ToByteArray());
            enc = enc.Replace("/", "_");
            enc = enc.Replace("+", "-");
            return enc.Substring(0, 22);
        }

        public static bool TryDecodeToGuid(this string input, out Guid decoded)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(input))
                {
                    decoded = Guid.Empty;
                    return true;
                }
                input = input.Replace("_", "/");
                input = input.Replace("-", "+");
                byte[] buffer = Convert.FromBase64String(input + "==");
                decoded = new Guid(buffer);
                return true;
            }
            catch (Exception)
            {
                decoded = Guid.Empty;
                return false;
            }
        }
    }
}