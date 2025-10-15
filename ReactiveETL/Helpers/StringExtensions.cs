namespace ReactiveETL;

using System.Text.RegularExpressions;
using System.Web;

using System;

/// <summary>
/// Extension methods for strings
/// </summary>
public static partial class StringExtensions
{
#pragma warning disable SYSLIB1045
#if NETSTANDARD2_0
    private static readonly Regex RemoveHtmlRegex =
        new(pattern: @"<(.|\n)*?>", RegexOptions.Compiled | RegexOptions.IgnoreCase, matchTimeout: TimeSpan.FromMilliseconds(1000));
#endif
#pragma warning restore SYSLIB1045

#if NET9_0_OR_GREATER

    [GeneratedRegex(@"<(.|\\n)*?>", RegexOptions.IgnoreCase, matchTimeoutMilliseconds:  1000)]
    private static partial Regex RemoveHtmlRegex();    
#endif
    
    /// <summary>
    /// Remove Html Markup
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string RemoveHtml(this object text)
    {
        if (text == null)
        {
            return null;
        }
#if NETSTANDARD2_0
        return HttpUtility.HtmlDecode(RemoveHtmlRegex.Replace((string)text, string.Empty));
#endif
#if NET9_0_OR_GREATER
        return HttpUtility.HtmlDecode(RemoveHtmlRegex().Replace((string)text, string.Empty));
#endif  
    }

    /// <summary>
    /// Limit size of string to the given size
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    public static string LimitSizeTo(this object obj, int size)
    {
        var txt = obj as string;
        if (txt != null && txt.Length > size) txt = txt.Substring(0, size - 1);

        return txt;
    }
}