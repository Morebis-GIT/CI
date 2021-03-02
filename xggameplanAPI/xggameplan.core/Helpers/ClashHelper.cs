using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;

namespace xggameplan.core.Helpers
{
    public static class ClashHelper
    {
        /// <summary>
        /// Validate
        /// </summary>
        /// <param name="newClashes">clash which need to validate</param>
        /// <param name="exisitingClashes">Existing class in DB to compare</param>
        public static void ValidateClashes(List<Clash> newClashes, List<Clash> exisitingClashes = null)
        {
            newClashes = newClashes.OrderBy(c => c.ParentExternalidentifier).ThenBy(c => c.Externalref).ToList();

            newClashes.Where(c => c.Externalref.Equals(c.ParentExternalidentifier, StringComparison.OrdinalIgnoreCase))
                .ToList().ForEach(
                    c => throw new Exception("Invalid parent clash code at Clash code " + c.Externalref));

            // find the duplicate entry with existing clash
            if (exisitingClashes != null && exisitingClashes.Any())
            {
                newClashes.Where(y => exisitingClashes.Any(
                    z => z.Externalref.Equals(y.Externalref, StringComparison.OrdinalIgnoreCase)))?.ToList().ForEach(
                    s => throw new Exception("Clash code " + s.Externalref + " already exist"));
            }

            //find duplicate entry within new clash
            newClashes.Select(s => s.Externalref).GroupBy(c => c, StringComparer.OrdinalIgnoreCase)
                .Where(g => g.Count() > 1)?.ToList()
                .ForEach(s => throw new Exception("Clash code " + s.Key + " already exist"));

            var allClashes = newClashes.Union(exisitingClashes ?? Enumerable.Empty<Clash>());

            newClashes.Where(c => !string.IsNullOrWhiteSpace(c.ParentExternalidentifier)).ToList().ForEach(c =>
            {
                var parentClash = allClashes.FirstOrDefault(e => e.Externalref.Equals(c.ParentExternalidentifier,
                    StringComparison.OrdinalIgnoreCase));
                if (parentClash != null)
                {
                    if (parentClash.DefaultPeakExposureCount < c.DefaultPeakExposureCount)
                    {
                        throw new Exception("Clash code " + c.Externalref +
                                            " default peak exposure count should be less than or equal to " +
                                            parentClash.DefaultPeakExposureCount);
                    }

                    if (parentClash.DefaultOffPeakExposureCount < c.DefaultOffPeakExposureCount)
                    {
                        throw new Exception("Clash code " + c.Externalref +
                                            " default non-peak exposure count should be less than or equal to " +
                                            parentClash.DefaultOffPeakExposureCount);
                    }
                }
                else
                {
                    throw new Exception("Clash code " + c.Externalref + " has invalid parent clash code " +
                                        c.ParentExternalidentifier);
                }
            });

            newClashes.Where(c => allClashes.Any(ch => ch.ParentExternalidentifier == c.Externalref)).ToList().ForEach(c =>
            {
                var childClashes = allClashes.Where(ch => ch.ParentExternalidentifier == c.Externalref);

                foreach (var childClash in childClashes)
                {
                    if (c.DefaultPeakExposureCount < childClash.DefaultPeakExposureCount)
                    {
                        throw new Exception("Clash code " + c.Externalref +
                            " default peak exposure count should be higher than or equal to " +
                            childClash.DefaultPeakExposureCount);
                    }

                    if (c.DefaultOffPeakExposureCount < childClash.DefaultOffPeakExposureCount)
                    {
                        throw new Exception("Clash code " + c.Externalref +
                            " default non-peak exposure count should be higher than or equal to " +
                            childClash.DefaultOffPeakExposureCount);
                    }
                }
            });
        }

        /// <summary>
        /// Calculates top parents for each item in dictionary
        /// </summary>
        /// <param name="clashByExternalRef"></param>
        /// <returns>Dictionary with Clash External Reference as Key and Root Clash Item as value</returns>
        public static IReadOnlyDictionary<string, string> CalculateClashTopParents(IReadOnlyDictionary<string, Clash> clashByExternalRef)
        {
            var result = new Dictionary<string, string>();

            foreach (var item in clashByExternalRef)
            {
                result[item.Key] = CalculateClashTopParent(item.Key);
            }

            return result;

            string CalculateClashTopParent(string clashCode)
            {
                if (!clashByExternalRef.TryGetValue(clashCode, out var item) || string.IsNullOrWhiteSpace(item.ParentExternalidentifier))
                {
                    return clashCode;
                }

                return CalculateClashTopParent(item.ParentExternalidentifier);
            }
        }
    }
}
