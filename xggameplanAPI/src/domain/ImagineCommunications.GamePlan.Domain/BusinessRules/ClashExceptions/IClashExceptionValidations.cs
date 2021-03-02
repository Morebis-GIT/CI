using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Validation;

namespace ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions
{
    public interface IClashExceptionValidations
    {
        /// <summary>
        /// Validates whether give clash exceptions overlaps with existing clash exceptions
        /// </summary>
        /// <param name="givenExceptions" cref="IEnumerable{ClashException}"></param>
        /// <param name="offsetHours"></param>
        /// <param name="existingExceptions"></param>
        /// <returns>
        /// Return validation result typeof <see cref="CustomValidationResult"/>
        /// </returns>
        CustomValidationResult ValidateTimeRanges(IEnumerable<ClashException> givenExceptions, int offsetHours,
            IEnumerable<ClashException> existingExceptions = null);

        /// <summary>
        /// Validates whether given clash exception does not violate "same structure" rules
        /// </summary>
        /// <param name="clashException"></param>
        /// <returns></returns>
        CustomValidationResult ValidateClashExceptionForSameStructureRulesViolation(ClashException clashException);
    }
}
