using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Validation;

namespace xggameplan.Controllers
{
    public partial class ClashExceptionController
    {
        private CustomValidationResult ValidateForSave(IEnumerable<ClashException> incomingClashExceptions)
        {
            foreach (var clashException in incomingClashExceptions)
            {
                var structureRulesViolationValidationResult =
                    _clashExceptionValidations.ValidateClashExceptionForSameStructureRulesViolation(clashException);

                if (!structureRulesViolationValidationResult.Successful)
                {
                    return structureRulesViolationValidationResult;
                }
            }

            // MUST BE REWORKED!
            const int offsetHours = 6;
            var existingClashExceptions = _clashExceptionRepository.GetAll().ToList();

            return _clashExceptionValidations.ValidateTimeRanges(incomingClashExceptions, offsetHours,
                existingClashExceptions);
        }
    }
}
