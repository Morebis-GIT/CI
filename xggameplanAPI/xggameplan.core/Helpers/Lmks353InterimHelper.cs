using System.Text.RegularExpressions;

namespace xggameplan.core.Helpers
{
    /// <summary>
    /// Exposes helper methods to extract names of ext.refs from the specific programme/category name values
    /// according to the pattern "programme name + (ext.ref)" or "programme category + (ext.ref)"
    /// </summary>
    public static class Lmks353InterimHelper
    {
        private static readonly Regex _pattern = new Regex(@"^(?<name>.+)\((?<extref>\w+)\)\s*$");

        /// <summary>
        /// Gets the external reference value from the specified <see cref="programmeOrCategoryName"/> parameter if it exists,
        /// otherwise returns the original <see cref="programmeOrCategoryName"/> parameter value.
        /// </summary>
        public static string GetExternalRefIfExists(string programmeOrCategoryName)
        {
            if (programmeOrCategoryName is null)
            {
                return null;
            }

            var match = _pattern.Match(programmeOrCategoryName);

            if (match.Success)
            {
                return match.Groups["extref"].Value;
            }

            return programmeOrCategoryName;
        }
    }
}
