using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Validation;

namespace xggameplan.core.Validators
{
    public interface IClashValidator
    {
        /// <summary>
        /// Validates whether there are no Clashes which has parent external identifier equal to provided Clash external identifier
        /// </summary>
        /// <param name="clashExternalReference"></param>
        /// <returns>
        /// Return validation result typeof <see cref="CustomValidationResult"/>
        /// </returns>
        CustomValidationResult ValidateClashHasNoChildClashes(string clashExternalReference);

        /// <summary>
        /// Validates whether provided Clash is not linked to any active Product
        /// </summary>
        /// <param name="clashExternalReference"></param>
        /// <returns>
        /// Return validation result typeof <see cref="CustomValidationResult"/>
        /// </returns>
        CustomValidationResult ValidateClashIsNotLinkedToActiveProduct(string clashExternalReference);

        /// <summary>
        /// Validates whether provided Clash Difference do not overlap with existing ones
        /// </summary>
        /// <param name="clashDifferences"></param>
        /// <returns>
        /// Return validation result typeof <see cref="CustomValidationResult"/>
        /// </returns>
        CustomValidationResult ValidateTimeRanges(IEnumerable<ClashDifference> clashDifferences);

        /// <summary>
        /// Validates whether provided Clash Difference Peak and Off-peak exposure count values
        /// are do not higher than parent's ones for each sales area
        /// </summary>
        /// <param name="clash"></param>
        /// <param name="allClashes"></param>
        /// <returns>
        /// Return validation result typeof <see cref="CustomValidationResult"/>
        /// </returns>
        CustomValidationResult ValidateExposureCountDifferences(Clash clash, IEnumerable<Clash> allClashes);
    }
}
