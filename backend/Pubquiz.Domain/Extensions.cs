using System;

namespace Pubquiz.Domain;

public static class Extensions
{
    public static string ToShortGuidString(this Guid guid)
    {
        string enc = Convert.ToBase64String(guid.ToByteArray());
        enc = enc.Replace("/", "_");
        enc = enc.Replace("+", "-");
        return enc.Substring(0, 22);
    }
}