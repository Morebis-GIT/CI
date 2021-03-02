using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Domain.Generic.Validation;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using xggameplan.Extensions;

namespace xggameplan.core.Validators
{
    public class ClashValidator : IClashValidator
    {
        private readonly IClashRepository _clashRepository;
        private readonly IProductRepository _productRepository;

        public ClashValidator(IClashRepository clashRepository, IProductRepository productRepository)
        {
            _clashRepository = clashRepository;
            _productRepository = productRepository;
        }

        public CustomValidationResult ValidateClashHasNoChildClashes(string clashExternalReference)
        {
            var lookupClashes = _clashRepository.GetAll()
                .Where(c => c.ParentExternalidentifier == clashExternalReference);

            return lookupClashes.Any()
                ? CustomValidationResult.Failed()
                : CustomValidationResult.Success();
        }

        public CustomValidationResult ValidateClashIsNotLinkedToActiveProduct(string clashExternalReference)
        {
            var lookupProducts = _productRepository.GetAll()
                .Where(p => p.ClashCode == clashExternalReference && p.EffectiveEndDate >= DateTime.UtcNow);

            return lookupProducts.Any()
                ? CustomValidationResult.Failed()
                : CustomValidationResult.Success();
        }

        public CustomValidationResult ValidateTimeRanges(
            IEnumerable<ClashDifference> clashDifferences)
        {
            var filteredExistingDifferences = clashDifferences.Where(d => d.EndDate == null || d.EndDate >= DateTime.Today.Date).ToList();

            foreach (var difference in clashDifferences)
            {
                if (DoesOverlapExistWithExistingDifferences(difference, filteredExistingDifferences))
                {
                    var salesAreaName = string.IsNullOrWhiteSpace(difference.SalesArea)
                        ? "all sales areas"
                        : $"{difference.SalesArea} sales area";
                    return CustomValidationResult.Failed($"Overlapping dates exist for {salesAreaName} difference");
                }
            }

            return CustomValidationResult.Success();
        }

        private static bool DoesOverlapExistWithExistingDifferences(
            ClashDifference givenDifference,
            IEnumerable<ClashDifference> existingDifferences)
        {
            var overlapingDifferences = existingDifferences
                    .Where(d => d.SalesArea == givenDifference.SalesArea);

            overlapingDifferences = overlapingDifferences.Where(d => d != givenDifference);

            return overlapingDifferences.Any(existingDifference =>
                {
                    var range = (existingDifference.StartDate, existingDifference.EndDate);

                    if ((givenDifference.StartDate is null || range.EndDate is null || givenDifference.StartDate <= range.EndDate) &&
                    (givenDifference.EndDate is null || range.StartDate is null || givenDifference.EndDate >= range.StartDate))
                    {
                        return DoesOverlapExistOnDaysOfWeek(givenDifference.TimeAndDow?.ConvertToTimeAndDow(), existingDifference);
                    }

                    return false;
                });
        }

        private static bool DoesOverlapExistOnDaysOfWeek(
            TimeAndDow givenDifferenceTimeAndDow,
            ClashDifference existingDifference)
        {
            const char one = '1';
            bool areDowsIntersect;

            var daysOfWeekOfGivenClashException = givenDifferenceTimeAndDow.DaysOfWeek.ParseDayOfWeekDayCode();
            var daysOfWeekOfExistingClashException = existingDifference.TimeAndDow.DaysOfWeekBinary.ParseDayOfWeekDayCode();

            var intersection = daysOfWeekOfExistingClashException.Intersect(daysOfWeekOfGivenClashException);

            areDowsIntersect = intersection.Any();

            return areDowsIntersect && DoesOverlapExistOnTimeRanges(givenDifferenceTimeAndDow, existingDifference.TimeAndDow?.ConvertToTimeAndDow());
        }

        private static bool DoesOverlapExistOnTimeRanges(
            TimeAndDow givenDifferenceTimeAndDow,
            TimeAndDow existingDifferenceTimeAndDow)
        {
            const int offsetHours = 6;
            var startTime = new TimeSpan(6, 0, 0);
            var endTime = new TimeSpan(5, 59, 59);

            if (!existingDifferenceTimeAndDow.StartTime.HasValue && !existingDifferenceTimeAndDow.EndTime.HasValue)
            {
                return true;
            }

            if (!givenDifferenceTimeAndDow.StartTime.HasValue && !givenDifferenceTimeAndDow.EndTime.HasValue)
            {
                return true;
            }

            var existingDifferenceTimeAndDowStartTime = existingDifferenceTimeAndDow.StartTime ?? startTime;
            var existingDifferenceTimeAndDowEndTime = existingDifferenceTimeAndDow.EndTime ?? endTime;

            var existingClashRange = new DateTimeRange(
                RevertOffsetAndConvertToDateTime(existingDifferenceTimeAndDowStartTime, offsetHours),
                RevertOffsetAndConvertToDateTime(existingDifferenceTimeAndDowEndTime, offsetHours)
            );

            var givenDifferenceTimeAndDowStartTime = givenDifferenceTimeAndDow.StartTime ?? startTime;
            var givenDifferenceTimeAndDowEndTime = givenDifferenceTimeAndDow.EndTime ?? endTime;

            var givenClashRange = new DateTimeRange(
                RevertOffsetAndConvertToDateTime(givenDifferenceTimeAndDowStartTime, offsetHours),
                RevertOffsetAndConvertToDateTime(givenDifferenceTimeAndDowEndTime, offsetHours)
            );

            return existingClashRange.Overlays(givenClashRange, DateTimeRange.CompareStrategy.IgnoreEdges);
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

        public CustomValidationResult ValidateExposureCountDifferences(Clash clash, IEnumerable<Clash> allClashes)
        {
            if (!string.IsNullOrWhiteSpace(clash.ParentExternalidentifier))
            {
                var parentClash = _clashRepository.FindByExternal(clash.ParentExternalidentifier).FirstOrDefault();

                if (parentClash != null)
                {
                    var validationResult = ValidateParentCountDifferences(clash.Differences, parentClash);

                    if (validationResult != CustomValidationResult.Success())
                    {
                        return validationResult;
                    }
                }
            }

            var childClashes = allClashes
                .Where(c => c.ParentExternalidentifier == clash.Externalref && c.Differences.Any()).ToList();

            if (childClashes.Any())
            {
                return ValidateChildCountDifferences(childClashes, clash);
            }

            return CustomValidationResult.Success();
        }

        public CustomValidationResult ValidateParentCountDifferences(List<ClashDifference> childClashDifferences, Clash parentClash)
        {
            var parentClashDifferences = parentClash.Differences;

            foreach (var difference in childClashDifferences)
            {
                var differencesForSalesArea = parentClashDifferences.Where(d => d.SalesArea == difference.SalesArea).ToList();

                if (!differencesForSalesArea.Any())
                {
                    if (parentClash.DefaultPeakExposureCount < difference.PeakExposureCount)
                    {
                        return CustomValidationResult.Failed(
                            $"Peak exposure count should be less than or equal to the parent default value for {difference.SalesArea} Sales Area");
                    }

                    if (parentClash.DefaultOffPeakExposureCount < difference.OffPeakExposureCount)
                    {
                        return CustomValidationResult.Failed(
                            $"Non-peak exposure count should be less than or equal to the parent default value for {difference.SalesArea} Sales Area");
                    }

                    continue;
                }

                var minParentPeakExposureCount = differencesForSalesArea.OrderBy(d => d.PeakExposureCount).FirstOrDefault().PeakExposureCount;

                if(minParentPeakExposureCount == null)
                {
                    minParentPeakExposureCount = parentClash.DefaultPeakExposureCount;
                }

                if (difference.PeakExposureCount > minParentPeakExposureCount)
                {
                    return CustomValidationResult.Failed(
                        $"Peak exposure count should be less than or equal to the parent difference value for {difference.SalesArea} Sales Area");
                }

                var minParentOffPeakExposureCount = differencesForSalesArea.OrderBy(d => d.OffPeakExposureCount).FirstOrDefault().OffPeakExposureCount;

                if (minParentOffPeakExposureCount == null)
                {
                    minParentOffPeakExposureCount = parentClash.DefaultOffPeakExposureCount;
                }

                if (difference.OffPeakExposureCount > minParentOffPeakExposureCount)
                {
                    return CustomValidationResult.Failed(
                        $"Non-peak exposure count should be less than or equal to the parent difference value for {difference.SalesArea} Sales Area");
                }
            }

            return CustomValidationResult.Success();
        }

        public CustomValidationResult ValidateChildCountDifferences(List<Clash> childClashes, Clash parentClash)
        {
            var parentClashDifferences = parentClash.Differences;

            if(parentClashDifferences != null && parentClashDifferences.Any())
            {
                var differencesForSalesAreas = parentClashDifferences.GroupBy(c => c.SalesArea);

                foreach (var differences in differencesForSalesAreas)
                {
                    var minParentPeakExposureCount = differences.OrderBy(d => d.PeakExposureCount).FirstOrDefault();
                    var minParentOffPeakExposureCount = differences.OrderBy(d => d.OffPeakExposureCount).FirstOrDefault();

                    var salesArea = string.IsNullOrWhiteSpace(differences.Key)
                        ? "all sales areas"
                        : $"{differences.Key} sales area";

                    if (childClashes.Any(c =>
                        c.Differences.Any(d => d.PeakExposureCount > minParentPeakExposureCount?.PeakExposureCount && d.SalesArea == differences.Key)))
                    {
                        return CustomValidationResult.Failed(
                            $"Parent difference peak exposure count should be higher than or equal to the child value for {salesArea} Sales Area");
                    }

                    if (childClashes.Any(c =>
                        c.Differences.Any(d => d.OffPeakExposureCount > minParentOffPeakExposureCount?.OffPeakExposureCount && d.SalesArea == differences.Key)))
                    {
                        return CustomValidationResult.Failed(
                            $"Parent difference non-peak exposure count should be higher than or equal to the child value for {salesArea} Sales Area");
                    }
                }

                var clashesWithOtherSalesAreaDifferences = childClashes.Where(c => 
                c.Differences.Any(d => 
                !differencesForSalesAreas.Any(pd => pd.Key == d.SalesArea))).ToList();

                if(clashesWithOtherSalesAreaDifferences != null)
                {
                    return ValidateChildExposureCount(clashesWithOtherSalesAreaDifferences, parentClash);
                }
            }
            else
            {
                return ValidateChildExposureCount(childClashes, parentClash);
            }

            return CustomValidationResult.Success();
        }

        public CustomValidationResult ValidateChildExposureCount(List<Clash> childClashes, Clash parentClash)
        {
            if (childClashes.Any(c =>
                        c.Differences.Any(d => d.PeakExposureCount > parentClash.DefaultPeakExposureCount)))
            {
                return CustomValidationResult.Failed(
                    $"Parent peak exposure count should be higher than or equal to the child difference value");
            }

            if (childClashes.Any(c =>
                c.Differences.Any(d => d.OffPeakExposureCount > parentClash.DefaultOffPeakExposureCount)))
            {
                return CustomValidationResult.Failed(
                    $"Parent non-peak exposure count should be higher than or equal to the child difference value");
            }

            return CustomValidationResult.Success();
        }
    }
}
