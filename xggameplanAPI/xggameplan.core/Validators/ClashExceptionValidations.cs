using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Domain.Generic.Validation;
using xggameplan.Extensions;

namespace xggameplan.core.Validations
{
    public class ClashExceptionValidations : IClashExceptionValidations
    {
        private readonly IClashRepository _clashRepository;

        public ClashExceptionValidations(IClashRepository clashRepository)
        {
            _clashRepository = clashRepository;
        }

        #region Overlap Validations

        /// <summary>
        /// Validate ClashExceptions overlay with existing ones, skipping those which has end date less than today
        /// </summary>
        /// <returns></returns>
        public CustomValidationResult ValidateTimeRanges(
            IEnumerable<ClashException> givenExceptions,
            int offsetHours,
            IEnumerable<ClashException> existingExceptions = null)
        {
            if (existingExceptions == null || !existingExceptions.Any())
            {
                return CustomValidationResult.Success();
            }

            var filteredExistingExceptions = existingExceptions.Where(e => e.EndDate == null || e.EndDate >= DateTime.Today.Date).ToList();

            var overlapExists = givenExceptions.Any(ne => DoesOverlapExistWithExistingClashExceptions(ne, filteredExistingExceptions, offsetHours));

            return overlapExists
                ? CustomValidationResult.Failed("Overlapping Dates Exist for given exceptions")
                : CustomValidationResult.Success();
        }

        private bool DoesOverlapExistWithExistingClashExceptions(
            ClashException givenClashException,
            IEnumerable<ClashException> existingExceptions,
            int offsetHours)
        {
            return existingExceptions
                .Where(e => e.Id != givenClashException.Id)
                .Any(existingClashException =>
                {
                    var range = (existingClashException.StartDate, existingClashException.EndDate);

                    if ((range.EndDate == null || givenClashException.StartDate <= range.EndDate) &&
                    (givenClashException.EndDate == null || givenClashException.EndDate >= range.StartDate))
                    {
                        if (DoClashExceptionsHaveSameFromAndToProperties(givenClashException, existingClashException))
                        {
                            return givenClashException.TimeAndDows.Any(givenClashExceptionTimeAndRow =>
                                DoesOverlapExistOnDaysOfWeek(givenClashExceptionTimeAndRow, existingClashException, offsetHours));
                        }
                    }

                    return false;
                });
        }

        private static bool DoClashExceptionsHaveSameFromAndToProperties(
            ClashException givenClashException,
            ClashException existingClashException)
        {
            return givenClashException.FromType == existingClashException.FromType &&
                   givenClashException.ToType == existingClashException.ToType &&
                   givenClashException.FromValue == existingClashException.FromValue &&
                   givenClashException.ToValue == existingClashException.ToValue;
        }

        private static bool DoesOverlapExistOnDaysOfWeek(
            TimeAndDow givenClashExceptionTimeAndDow,
            ClashException existingClashException,
            int offsetHours)
        {
            var daysOfWeekOfGivenClashException = givenClashExceptionTimeAndDow.DaysOfWeek.ParseDayOfWeekDayCode();
            return existingClashException.TimeAndDows.Any(existingClashExceptionTimeAndDow =>
            {
                var daysOfWeekOfExistingClashException = existingClashExceptionTimeAndDow.DaysOfWeek.ParseDayOfWeekDayCode();

                var intersection = daysOfWeekOfExistingClashException.Intersect(daysOfWeekOfGivenClashException);
                if (intersection.Any())
                {
                    return DoesOverlapExistOnTimeRanges(givenClashExceptionTimeAndDow, existingClashExceptionTimeAndDow, offsetHours);
                }

                return false;
            });
        }

        private static bool DoesOverlapExistOnTimeRanges(
            TimeAndDow givenClashExceptionTimeAndDow,
            TimeAndDow existingClashExceptionTimeAndDow,
            int offsetHours)
        {
            if (!existingClashExceptionTimeAndDow.StartTime.HasValue && !existingClashExceptionTimeAndDow.EndTime.HasValue)
            {
                return true;
            }

            if (!givenClashExceptionTimeAndDow.StartTime.HasValue && !givenClashExceptionTimeAndDow.EndTime.HasValue)
            {
                return true;
            }

            var existingClashExceptionRange = new DateTimeRange(
                RevertOffsetAndConvertToDateTime(existingClashExceptionTimeAndDow.StartTime.Value, offsetHours),
                RevertOffsetAndConvertToDateTime(existingClashExceptionTimeAndDow.EndTime.Value, offsetHours)
            );

            var givenClashExceptionRange = new DateTimeRange(
                RevertOffsetAndConvertToDateTime(givenClashExceptionTimeAndDow.StartTime.Value, offsetHours),
                RevertOffsetAndConvertToDateTime(givenClashExceptionTimeAndDow.EndTime.Value, offsetHours)
            );

            return existingClashExceptionRange.Overlays(givenClashExceptionRange, DateTimeRange.CompareStrategy.IgnoreEdges);
        }

        private static DateTime RevertOffsetAndConvertToDateTime(TimeSpan time, int offsetHours)
        {
            var startPoint = DateTime.UtcNow.Date;
            var ticks = time.Ticks - TimeSpan.FromHours(offsetHours).Ticks;

            if (ticks < 0)
            {
                startPoint = startPoint.AddDays(1);
            }

            return startPoint.AddTicks(ticks);
        }

        #endregion Overlap Validations

        #region Structure Validations

        public CustomValidationResult ValidateClashExceptionForSameStructureRulesViolation(ClashException clashException)
        {
            if (clashException.FromType != ClashExceptionType.Clash ||
                clashException.ToType != ClashExceptionType.Clash)
            {
                return CustomValidationResult.Success();
            }

            var fromClash = _clashRepository.FindByExternal(clashException.FromValue).FirstOrDefault();
            if (fromClash is null)
            {
                return CustomValidationResult.Failed(
                    $"Could not find Clash with external reference {clashException.FromValue}");
            }

            var toClash = _clashRepository.FindByExternal(clashException.ToValue).FirstOrDefault();
            if (toClash is null)
            {
                return CustomValidationResult.Failed(
                    $"Could not find Clash with external reference {clashException.ToValue}");
            }

            bool clashesAreFromTheSameStructure;
            try
            {
                clashesAreFromTheSameStructure = AreClashesFromTheSameStructure(fromClash, toClash);
            }
            catch (ArgumentException exception)
            {
                return CustomValidationResult.Failed(exception.Message);
            }

            if (clashesAreFromTheSameStructure)
            {
                if (clashException.IncludeOrExclude == IncludeOrExclude.I)
                {
                    return CustomValidationResult.Failed(
                        $"Clash exception with Include rule and values from: {clashException.FromValue}, " +
                        $"to: {clashException.ToValue} is not allowed as clashes are from the same structure");
                }
            }

            return CustomValidationResult.Success();
        }

        private bool AreClashesFromTheSameStructure(Clash first, Clash second)
        {
            while (true)
            {
                if (first == null || second == null)
                {
                    return false;
                }

                if (string.IsNullOrWhiteSpace(first.ParentExternalidentifier) &&
                    string.IsNullOrWhiteSpace(second.ParentExternalidentifier))
                {
                    return false;
                }

                if (first.Externalref.EqualInvariantCultureIgnoreCase(second.ParentExternalidentifier))
                {
                    return true;
                }

                if (second.Externalref.EqualInvariantCultureIgnoreCase(first.ParentExternalidentifier))
                {
                    return true;
                }

                if (first.ParentExternalidentifier.EqualInvariantCultureIgnoreCase(second.ParentExternalidentifier))
                {
                    return true;
                }

                var parentClashFromFirst =
                    _clashRepository.FindByExternal(first.ParentExternalidentifier).FirstOrDefault();

                if (parentClashFromFirst != null &&
                    !string.IsNullOrWhiteSpace(parentClashFromFirst.ParentExternalidentifier) &&
                        parentClashFromFirst.ParentExternalidentifier.EqualInvariantCultureIgnoreCase(second.ParentExternalidentifier))
                {
                    return true;
                }

                var parentClashFromSecond =
                    _clashRepository.FindByExternal(second.ParentExternalidentifier).FirstOrDefault();

                if (parentClashFromSecond != null &&
                    !string.IsNullOrWhiteSpace(parentClashFromSecond.ParentExternalidentifier) &&
                    parentClashFromSecond.ParentExternalidentifier.EqualInvariantCultureIgnoreCase(first.ParentExternalidentifier))
                {
                    return true;
                }

                first = parentClashFromFirst;
                second = parentClashFromSecond;
            }
        }

        #endregion Structure Validations
    }
}
