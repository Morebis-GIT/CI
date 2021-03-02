using System;
using System.Text;

namespace xggameplan.common.MSTeams
{
    /// <summary>
    /// MS Teams message formatting
    /// </summary>
    public static class MSTeamsMessageFormatter
    {
        /// <summary>
        /// Formatting types
        /// </summary>
        public enum FormattingTypes : byte
        {
            BoldText = 1,
            ItalicText = 2,
            StrikethroughText = 3
        }

        /// <summary>
        /// Formats hyperlink
        /// </summary>
        /// <param name="text"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string FormatHyperlink(string text, string url) =>
            "[" + text + "](" + url + ")";

        /// <summary>
        /// Formats text
        /// </summary>
        /// <param name="text"></param>
        /// <param name="formattingTypes"></param>
        /// <returns></returns>
        public static string FormatText(string text, params FormattingTypes[] formattingTypes)
        {
            var before = new StringBuilder();
            var after = new StringBuilder();

            foreach (FormattingTypes formattingType in formattingTypes)
            {
                switch (formattingType)
                {
                    case FormattingTypes.BoldText:
                        _ = before.Append("**");
                        _ = after.Append("**");
                        break;

                    case FormattingTypes.ItalicText:
                        _ = before.Append("*");
                        _ = after.Append("*");
                        break;

                    case FormattingTypes.StrikethroughText:
                        _ = before.Append("~~");
                        _ = after.Append("~~");
                        break;

                    default:
                        throw new ArgumentException(string.Format("Formatting type {0} is unsupported", formattingType.ToString()));
                }
            }

            return before.ToString() + text + after.ToString();
        }
    }
}
