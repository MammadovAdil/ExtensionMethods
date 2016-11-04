using System;
using System.Text;

namespace Ma.ExtensionMethods.Utilities
{
    public static class ExceptionUtilities
    {
        /// <summary>
        /// Construct error message including inner exceptions.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// When ex is null.
        /// </exception>
        /// <param name="ex">Exception to get message from.</param>
        /// <returns>Constructed error message.</returns>
        public static string ConstructErrorMessage(this Exception ex)
        {
            if (ex == null)
                throw new ArgumentNullException("ex");

            StringBuilder errorMessage = new StringBuilder();
            errorMessage.Append(ex.Message);

            if (ex.InnerException != null)
            {
                errorMessage.AppendFormat("\nInnerException: {0}", 
                    ConstructErrorMessage(ex.InnerException));
            }

            return errorMessage.ToString();
        }
    }
}
