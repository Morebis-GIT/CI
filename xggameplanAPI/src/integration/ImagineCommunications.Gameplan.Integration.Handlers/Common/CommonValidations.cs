using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.DayParts.Repositories;
using ImagineCommunications.GamePlan.Domain.ProgrammeCategory;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.Metadatas;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using xggameplan.Extensions;

namespace ImagineCommunications.Gameplan.Integration.Handlers.Common
{
    public static class CommonValidations
    {
        public static void ValidateBreakType(this IMetadataRepository repository, IReadOnlyCollection<string> breaks)
        {
            if (repository is null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            if (breaks == null || breaks.Count == 0 || breaks.Any(string.IsNullOrWhiteSpace))
            {
                throw new DataSyncException(DataSyncErrorCode.InvalidBreakType);
            }

            var metadataList =
                repository.GetByKey(MetaDataKeys.BreakTypes)
                .Select(s => s.Value.ToString())
                .ToArray();

            ValidateBreakType(metadataList, breaks);
        }

        public static void ValidateBreakType(IReadOnlyCollection<string> existingBreakTypes, IReadOnlyCollection<string> breaks)
        {
            if (breaks == null || breaks.Count == 0 || breaks.Any(string.IsNullOrWhiteSpace))
            {
                throw new DataSyncException(DataSyncErrorCode.InvalidBreakType);
            }

            var invalidBreaks = breaks.Distinct(StringComparer.CurrentCultureIgnoreCase)
                .Except(existingBreakTypes, StringComparer.CurrentCultureIgnoreCase)
                .ToArray();

            if (invalidBreaks.Any())
            {
                throw new DataSyncException(DataSyncErrorCode.InvalidBreakType, "Invalid Break Type: " + string.Join(",", invalidBreaks));
            }
        }

        public static void ValidateSalesArea(this ISalesAreaRepository repository, IReadOnlyCollection<string> salesAreaNames)
        {
            if (repository is null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            if (!salesAreaNames.Any())
            {
                throw new DataSyncException(DataSyncErrorCode.SalesAreaNotFound, "Empty Sales area names list");
            }

            var sanitizedNames = salesAreaNames.Distinct().Trim();
            IReadOnlyCollection<string> existingSalesAreas =
                repository.FindByNames(sanitizedNames)
                .Select(r => r.Name)
                .ToArray();

            ValidateSalesArea(existingSalesAreas, salesAreaNames);
        }

        public static void ValidateSalesArea(IReadOnlyCollection<string> existingSalesAreas, IReadOnlyCollection<string> salesAreaNames)
        {
            if (!salesAreaNames.Any())
            {
                throw new DataSyncException(DataSyncErrorCode.SalesAreaNotFound, "Empty Sales area names list");
            }

            var sanitizedNames = salesAreaNames.Distinct().Trim();
            var invalidNames = sanitizedNames.Except(existingSalesAreas).ToList();

            if (invalidNames.Any())
            {
                throw new DataSyncException(DataSyncErrorCode.SalesAreaNotFound, "Invalid Sales Area: " + string.Join(",", invalidNames.ToList()));
            }
        }

        public static void ValidateProgrammeCategory(this IProgrammeCategoryHierarchyRepository repository, IReadOnlyCollection<string> categories)
        {
            if (repository is null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            if (categories == null || categories.Count == 0 || categories.Any(string.IsNullOrWhiteSpace))
            {
                return;
            }

            var categoryNamesList = repository.GetAll()
                .Select(s => s.Name)
                .ToArray();

            ValidateProgrammeCategory(categoryNamesList, categories);
        }

        public static void ValidateProgrammeCategory(IReadOnlyCollection<string> categoryNamesList, IReadOnlyCollection<string> categories)
        {
            if (categories == null || categories.Count == 0 || categories.Any(string.IsNullOrWhiteSpace))
            {
                return;
            }

            var filteredCategories = categories.Where(s => !string.IsNullOrEmpty(s)).ToList();
            var invalidCategories = filteredCategories.Distinct(StringComparer.CurrentCultureIgnoreCase).Except(categoryNamesList, StringComparer.CurrentCultureIgnoreCase).ToList();
            if (invalidCategories.Any())
            {
                throw new DataSyncException(DataSyncErrorCode.ProgrammeCategoryNotFound, "Invalid programme categories: " + string.Join(",", invalidCategories.ToList()));
            }
        }

        public static void ValidateDemographics(this IDemographicRepository repository, IReadOnlyCollection<string> demographics)
        {
            if (repository is null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            if (repository.GetByExternalRef(demographics?.Where(d => !string.IsNullOrEmpty(d)).ToList()).Distinct().Count() != demographics?.Where(d => !string.IsNullOrEmpty(d)).Distinct().Count())
            {
                throw new DataSyncException(DataSyncErrorCode.DemographicNotFound, "Demographic doesn't exist");
            }
        }

        public static Demographic CheckDemographicByExternalRef(this IDemographicRepository repository, string externalRef)
        {
            if (repository is null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            var demographic = repository.GetByExternalRef(externalRef);

            if (demographic == null)
            {
                throw new DataSyncException(DataSyncErrorCode.ExternalReferenceNotFound,
                    $"Demographic can't be found for ExternalRef: {externalRef}");
            }

            return demographic;
        }

        public static Clash CheckClashByExternalRef(this IClashRepository repository, string externalRef)
        {
            if (repository is null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            var clash = repository.FindByExternal(externalRef)
                .SingleOrDefault();

            if (clash is null)
            {
                throw new DataSyncException(DataSyncErrorCode.ExternalReferenceNotFound,
                    "Clash can't be found with specified external reference.");
            }

            return clash;
        }

        public static void CheckClashParents(this IClashRepository repository, IReadOnlyCollection<Clash> newClashes)
        {
            if (repository is null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            var parentDoesNotExistMessages = new List<string>();
            var exposureCountGreaterThanParentsMessages = new List<string>();
            var existingClashes = repository.GetAll();
            newClashes = newClashes.OrderBy(c => c.ParentExternalidentifier).ThenBy(c => c.Externalref).ToList();

            newClashes.Where(c => c.Externalref.Equals(c.ParentExternalidentifier, StringComparison.OrdinalIgnoreCase))
                .ToList().ForEach(
                    c => parentDoesNotExistMessages.Add($"Clash code {c.Externalref} has invalid parent clash code {c.ParentExternalidentifier}"));

            var allClashes = newClashes.Union(existingClashes ?? Enumerable.Empty<Clash>());

            newClashes.Where(c => !string.IsNullOrWhiteSpace(c.ParentExternalidentifier)).ToList().ForEach(c =>
            {
                var parentClash = allClashes.FirstOrDefault(e => e.Externalref.Equals(c.ParentExternalidentifier,
                    StringComparison.OrdinalIgnoreCase));
                if (parentClash != null)
                {
                    if (parentClash.DefaultPeakExposureCount < c.DefaultPeakExposureCount)
                    {
                        exposureCountGreaterThanParentsMessages.Add("Clash code " + c.Externalref +
                                            " default peak exposure count should be less than or equal to " +
                                            parentClash.DefaultPeakExposureCount);
                    }

                    if (parentClash.DefaultOffPeakExposureCount < c.DefaultOffPeakExposureCount)
                    {
                        exposureCountGreaterThanParentsMessages.Add("Clash code " + c.Externalref +
                                            " default non-peak exposure count should be less than or equal to " +
                                            parentClash.DefaultOffPeakExposureCount);
                    }
                }
                else
                {
                    parentDoesNotExistMessages.Add("Clash code " + c.Externalref + " has invalid parent clash code " +
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
                        exposureCountGreaterThanParentsMessages.Add("Clash code " + c.Externalref +
                            " default peak exposure count should be higher than or equal to " +
                            childClash.DefaultPeakExposureCount);
                    }

                    if (c.DefaultOffPeakExposureCount < childClash.DefaultOffPeakExposureCount)
                    {
                        exposureCountGreaterThanParentsMessages.Add("Clash code " + c.Externalref +
                            " default non-peak exposure count should be higher than or equal to " +
                            childClash.DefaultOffPeakExposureCount);
                    }
                }
            });

            if (parentDoesNotExistMessages.Any())
            {
                throw new DataSyncException(DataSyncErrorCode.Clash_ParentDoesNotExist,
                    string.Join(", ", parentDoesNotExistMessages));
            }

            if (exposureCountGreaterThanParentsMessages.Any())
            {
                throw new DataSyncException(DataSyncErrorCode.Clash_ExposureCountGreaterThenParents,
                    string.Join(", ", exposureCountGreaterThanParentsMessages));
            }
        }

        public static void ValidateProgrammes(this IProgrammeRepository repository, IReadOnlyCollection<string> externalRefs)
        {
            if (repository is null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            var sanitizedExternalRefs = externalRefs.ToList().Distinct(StringComparer.CurrentCultureIgnoreCase).Trim().ToList();
            var existingProgrammes = repository.FindByExternal(sanitizedExternalRefs);
            var invalidExternalRefs = sanitizedExternalRefs.Except(existingProgrammes.Select(s => s.ExternalReference), StringComparer.CurrentCultureIgnoreCase).ToList();

            if (invalidExternalRefs.Any())
            {
                throw new DataSyncException(DataSyncErrorCode.ExternalReferenceNotFound, "Invalid Programme: " + string.Join(",", invalidExternalRefs.ToList()));
            }
        }

        public static void ValidateDayParts(this IStandardDayPartRepository repository, IReadOnlyCollection<int> externalIds)
        {
            if (repository is null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            var existingDayParts = repository.FindByExternal(externalIds.ToList());
            var invalidExternalRefs = externalIds.Except(existingDayParts.Select(s => s.DayPartId)).ToList();

            if (invalidExternalRefs.Any())
            {
                throw new DataSyncException(DataSyncErrorCode.DayPartNotFound, "Invalid DayParts: " + string.Join(",", invalidExternalRefs));
            }
        }
    }
}
