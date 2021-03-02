using System.Globalization;
using ImagineCommunications.GamePlan.ReportSystem.Formaters;

namespace xggameplan.core.ReportGenerators.ReportFormatters
{
    /// <summary>
    /// Currency formatter for GamePlan reports
    /// </summary>
    /// <seealso cref="ImagineCommunications.GamePlan.ReportSystem.Formaters.IFormatter" />
    public class CurrencyReportFormatter : IFormatter
    {
        public const string DefaultCurrencySymbol = "£";

        private readonly NumberFormatInfo _numberFormat;

        public CurrencyReportFormatter(string currencySymbol = DefaultCurrencySymbol)
        {
            _numberFormat = new NumberFormatInfo
            {
                CurrencySymbol = currencySymbol,
                CurrencyNegativePattern = 1
            };
        }

        /// <summary>
        /// Applies currency formatting to the specified value.
        /// </summary>
        /// <param name="value">Initial value</param>
        /// <returns></returns>
        public string Format(object value) =>
            value is null
                ? string.Empty
                : string.Format(_numberFormat, "{0:C}", value);
    }
}
