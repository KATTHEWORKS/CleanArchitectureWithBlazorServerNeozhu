using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace PublicCommon;
public static class StringExtensions
    {
    public static bool Equals(this string left, string right)
        {
        return string.Equals(left, right, StringComparison.OrdinalIgnoreCase);
        }
    public static string ToMD5(this string input)
        {
        using (var md5 = MD5.Create())
            {
            var encoding = Encoding.ASCII;
            var data = encoding.GetBytes(input);

            Span<byte> hashBytes = stackalloc byte[16];
            md5.TryComputeHash(data, hashBytes, out var written);
            if (written != hashBytes.Length)
                throw new OverflowException();


            Span<char> stringBuffer = stackalloc char[32];
            for (var i = 0; i < hashBytes.Length; i++) hashBytes[i].TryFormat(stringBuffer.Slice(2 * i), out _, "x2");
            return new string(stringBuffer);
            }
        }

    private static readonly char[] TrimChars = [' ', '\t', '\n', '\r'];
    public static string? TrimSelf(this string? input)
        {
        if (string.IsNullOrEmpty(input))
            return input;
        input = new StringBuilder(input).TrimSelf();
        return input;
        }
    public static string? TrimSelf(this StringBuilder input)
        {
        if (input == null)
            return null;
        for (int i = input.Length - 1; i >= 0; i--)
            {
            if (TrimChars.Contains(input[i]))
                {
                input.Remove(i, 1);
                }
            }
        return input.ToString();
        }
    public static string? TrimResult(this string? input)
        {
        if (string.IsNullOrEmpty(input))
            return input;
        char[] trimChars = { ' ', '\t', '\n', '\r' };
        // Create a StringBuilder to build the trimmed string
        var trimmed = new StringBuilder(input.Length);

        foreach (char c in input)
            {
            if (!trimChars.Contains(c))
                {
                trimmed.Append(c);
                }
            }

        return trimmed.ToString();
        }
    public static bool IsNullOrEmpty(this string? input)
        {
        return input != null && string.IsNullOrEmpty(input.TrimResult());
        }
    public static bool IsNullOrEmptyAndTrimSelf(this string? input)
        {
        return string.IsNullOrEmpty(input.TrimSelf());
        }

    public static string? SubStringTruncate(this string? str, int length = 25, string? truncationSymbol = null)
        {
        //ex: madhusudhan  
        //if length expected is 10, then if more than 10 then add 10
        //                                          if 10-dots length logic
        if (str == null)
            {
            return string.Empty;
            }

        if (string.IsNullOrEmpty(truncationSymbol))
            return str.Length <= length ? str : str[..length];
        else
            {
            if (str.Length <= length) return str;
            else
                {

                }
            return str![..length] + truncationSymbol;
            }
        }
    public static string SubstringSafe(this string? text, int maxLength)
        {
        if (string.IsNullOrEmpty(text))
            {
            return string.Empty; // Return empty string for null or empty input
            }

        return text.Length <= maxLength ? text : text.Substring(0, maxLength);
        }

    public static string UrlGetDomain(this string? text, string defaultValue = "")
        {
        if (string.IsNullOrEmpty(text))
            {
            if (!string.IsNullOrEmpty(defaultValue)) return defaultValue;
            return string.Empty; // Return empty string for null or empty input
            }
        var t1 = text.Replace("https://www.", string.Empty, StringComparison.InvariantCultureIgnoreCase);
        if (t1.IsNullOrEmpty() && !defaultValue.IsNullOrEmpty()) return defaultValue;
        return t1.Trim();
        }
    }
