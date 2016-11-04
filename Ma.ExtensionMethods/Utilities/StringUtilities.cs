using System;
using System.Globalization;

namespace Ma.ExtensionMethods.Utilities
{
    public static class StringUtilities
    {
        private static readonly CultureInfo azerbaijaniCulture = CultureInfo.GetCultureInfo("az-Latn-AZ");

        /// <summary>
        /// Convert propvided string to upper string using Azerbaijani culture
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// When source is null
        /// </exception>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToAzeriUpper(this string source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            return source.ToUpper(azerbaijaniCulture);
        }

        /// <summary>
        /// Convert string to Date.
        /// </summary>
        /// <param name="dateString">Date in string format.</param>
        /// <returns>Appropriate date if can convert/null otherwise.</returns>
        public static Nullable<DateTime> ToDate(this string dateString)
        {
            if (string.IsNullOrEmpty(dateString))
                return null;

            // Replace 'XX's with '01's
            dateString = dateString.Replace("XX", "01");

            string[] formats = { "yyyy-MM-dd" };
            DateTime convertedDate;
            bool isSuccessful = DateTime.TryParseExact(
                dateString,
                formats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out convertedDate);

            if (isSuccessful)
                return convertedDate;
            else
                return null;
        }
    }
}
