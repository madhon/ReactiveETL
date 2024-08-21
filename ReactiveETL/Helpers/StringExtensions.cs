using System.Text.RegularExpressions;
using System.Web;

namespace ReactiveETL
{
    using System;

    /// <summary>
    /// Extension methods for strings
    /// </summary>
    public static partial class StringExtensions
    {
#pragma warning disable SYSLIB1045
        private static readonly Regex RemoveHtmlRegex =
            new(pattern: @"<(.|\n)*?>", RegexOptions.Compiled | RegexOptions.IgnoreCase, matchTimeout: TimeSpan.FromMilliseconds(1000));
#pragma warning restore SYSLIB1045
        
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
            
            return HttpUtility.HtmlDecode(RemoveHtmlRegex.Replace((string)text, string.Empty));
                //Regex.Replace((string)text, @"<(.|\n)*?>", string.Empty));
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
}
