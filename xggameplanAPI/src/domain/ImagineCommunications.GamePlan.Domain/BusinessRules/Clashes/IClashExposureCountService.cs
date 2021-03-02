using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;

namespace ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes
{
    public interface IClashExposureCountService
    {
        /// <summary>
        /// Calculate the clash exposure count for a date and sales area.
        /// </summary>
        /// <param name="clashDifferences">
        /// Difference in clash exposure count based on given filters.
        /// </param>
        /// <param name="clashExposureCountDefaults">
        /// The default peak and off-peak exposure counts.
        /// </param>
        /// <param name="dateAndSalesArea">
        /// Summary details of the date and sales area to assess.
        /// </param>
        /// <returns>
        /// Returns a value indicating the allowed clash exposure count for a
        /// date and sales area.
        /// </returns>
        int Calculate(
            IReadOnlyList<ClashDifference> clashDifferences,
            (int peak, int offPeak) clashExposureCountDefaults,
            (DateTime scheduledDate, string salesAreaName) dateAndSalesArea
            );
    }
}
