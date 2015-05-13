using System.Text.RegularExpressions;
using System.Web;

namespace ReactiveETL
{
    /// <summary>
    /// Extension methods for strings
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Remove Html Markup
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string RemoveHtml(this object text)
        {
            if (text == null) return null;
            return HttpUtility.HtmlDecode(
                        Regex.Replace((string)text, @"<(.|\n)*?>", string.Empty));
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
