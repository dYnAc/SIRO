
namespace SIRO.Core.Helpers
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The string helper.
    /// </summary>
    internal static class StringHelper
    {
        /// <summary>
        /// The without postfix.
        /// </summary>
        /// <param name="str">
        /// The str.
        /// </param>
        /// <param name="postFix">
        /// The post fix.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string WithoutPostfix(this string str, string postFix)
        {
            if (str.EndsWith(postFix))
            {
                var index = str.LastIndexOf(postFix);
                str = str.Remove(index);
            }

            return str;
        }

        /// <summary>
        /// The build list from string.
        /// </summary>
        /// <param name="str">
        /// The str.
        /// </param>
        /// <param name="separator">
        /// The separator.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<string> BuildListFromString(this string str, char separator)
        {
            var enabledTemplates = str.Split(separator);
            var selected = from dtp in enabledTemplates
                           where !string.IsNullOrEmpty(dtp)
                           select dtp;

            var result = selected.ToList();

            return result;
        }
    }
}
