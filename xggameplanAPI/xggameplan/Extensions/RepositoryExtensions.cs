using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.AnalysisGroups;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.ProgrammeCategory;
using ImagineCommunications.GamePlan.Domain.Shared.ClearanceCodes;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.Schedules;
using xggameplan.Model;

namespace xggameplan.Extensions
{
    public static class RepositoryExtensions
    {
        /// <summary>
        /// Validate the demographics
        /// </summary>
        /// <param name="values"></param>
        /// <param name="invalidList"></param>
        /// <returns></returns>
        public static bool ValidateDemographics(this IDemographicRepository repository, List<string> values, out List<string> invalidList)
        {
            invalidList = null;
            if (values is null || values.Count == 0 || values.Any(String.IsNullOrWhiteSpace))
            {
                return false;
            }

            var demographics = (repository ?? throw new ArgumentNullException(nameof(repository))).GetAll().ToList();
            invalidList = values.Except(demographics.Select(d => d.ExternalRef)).ToList();
            return invalidList != null && !invalidList.Any();
        }

        /// <summary>
        /// Get sales ares id by names
        /// </summary>
        /// <param name="salesAreaNames"></param>
        /// <returns></returns>
        public static void ValidateSaleArea(
            this ISalesAreaRepository repository,
            List<string> salesAreaNames)
        {
            if (repository is null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            if (salesAreaNames?.Count == 0)
            {
                throw new InvalidDataException("No sales area names to validate");
            }

            var names = salesAreaNames.Distinct().Trim();
            List<SalesArea> salesAreas = repository.FindByNames(names);

            if (salesAreas is null)
            {
                throw new InvalidDataException(
                    "Invalid Sales Area Names: " + string.Join(",", names)
                    );
            }

            var invalidNames = names
                .Except(salesAreas.Select(s => s.Name))
                .ToList();

            if (invalidNames.Count == 0)
            {
                return;
            }

            throw new InvalidDataException(
                "Invalid Sales Area Names: " + string.Join(",", invalidNames)
                );
        }

        /// <summary>
        /// Validate Product external Id
        /// </summary>
        /// <param name="productExternalIds"></param>
        public static void ValidateProductExternalIds(this IProductRepository repository, List<string> productExternalIds)
        {
            if (productExternalIds == null || !productExternalIds.Any())
            {
                throw new Exception("Empty product external id");
            }

            var ids = productExternalIds.Distinct(StringComparer.CurrentCultureIgnoreCase).Trim().ToList();
            var products = (repository ?? throw new ArgumentNullException(nameof(repository))).FindByExternal(ids)
                ?.ToList();

            var invalidProductExternalIds = ids.Except(
                products?.Select(p => p.Externalidentifier) ?? Enumerable.Empty<string>(),
                StringComparer.CurrentCultureIgnoreCase).ToList();

            if (!invalidProductExternalIds.Any())
            {
                return;
            }

            var msg = "Invalid Product: " + string.Join(",", invalidProductExternalIds.ToList());
            throw new InvalidDataException(msg);
        }

        /// <summary>
        /// Validate Clash codes
        /// </summary>
        /// <param name="clashCodes"></param>
        public static void ValidateClashCodes(this IClashRepository repository, List<string> clashCodes)
        {
            if (clashCodes == null || !clashCodes.Any())
            {
                throw new Exception("Empty clash code");
            }
            var ids = clashCodes.Distinct(StringComparer.CurrentCultureIgnoreCase).Trim().ToList();
            var clashes = (repository ?? throw new ArgumentNullException(nameof(repository))).FindByExternal(clashCodes)
                ?.ToList();
            var invalidClashCodes = ids.Except(
                clashes?.Select(p => p.Externalref)?.Distinct() ?? Enumerable.Empty<string>(),
                StringComparer.CurrentCultureIgnoreCase).ToList();
            if (!invalidClashCodes.Any())
            {
                return;
            }

            var msg = "Invalid clash code: " + string.Join(",", invalidClashCodes.ToList());
            throw new InvalidDataException(msg);
        }

        public static void ValidateClearanceCode(this IClearanceRepository repository, List<string> clearanceCodes)
        {
            if (clearanceCodes == null || !clearanceCodes.Any())
            {
                throw new Exception("Empty clearance code");
            }
            var ids = clearanceCodes.Distinct(StringComparer.CurrentCultureIgnoreCase).Trim().ToList();
            var clearances = (repository ?? throw new ArgumentNullException(nameof(repository)))
                .FindByExternal(clearanceCodes)?.ToList();
            var invalidClearances = ids.Except(
                clearances?.Select(p => p.Code)?.Distinct() ?? Enumerable.Empty<string>(),
                StringComparer.CurrentCultureIgnoreCase).ToList();
            if (!invalidClearances.Any())
            {
                return;
            }

            var msg = "Invalid clearances code: " + string.Join(",", invalidClearances.ToList());
            throw new InvalidDataException(msg);
        }

        /// <summary>
        /// Get schedules based on parameter
        /// </summary>
        /// <param name="repository">schedule repository</param>
        /// <param name="salesAreaName">sales area id</param>
        /// <param name="date">schedule date</param>
        /// <returns></returns>
        public static Schedule GetOrCreateSchedule(this IScheduleRepository repository, string salesAreaName, DateTime date)
        {
            return (repository ?? throw new ArgumentNullException(nameof(repository))).GetSchedule(salesAreaName, date) ??
                   new Schedule()
                   {
                       SalesArea = salesAreaName,
                       Date = date
                   };
        }

        public static bool IsValid(this IProgrammeCategoryHierarchyRepository repository, IReadOnlyCollection<string> categories, out List<string> invalidList)
        {
            invalidList = null;
            if (categories is null || categories.Count == 0 || categories.Any(string.IsNullOrWhiteSpace))
            {
                return false;
            }

            var existedCategoriesList = (repository ?? throw new ArgumentNullException(nameof(repository))).GetAll();
            var categoryNamesList = existedCategoriesList?.Select(s => s.Name).ToList();
            invalidList = categoryNamesList.Any()
                ? categories.Distinct(StringComparer.CurrentCultureIgnoreCase).Except(categoryNamesList, StringComparer.CurrentCultureIgnoreCase).ToList()
                : null;
            return categoryNamesList.Any() && (invalidList is null || !invalidList.Any());
        }

        /// <summary>
        /// Validates <see cref="CreateAnalysisGroupModel.Id"/> collection
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="ids"></param>
        /// <param name="invalidList"></param>
        /// <exception cref="System.ArgumentNullException">Thrown when repository is null</exception>
        /// <returns></returns>
        public static bool IsValid(this IAnalysisGroupRepository repository, ICollection<int> ids, out ICollection<int> invalidList)
        {
            invalidList = null;
            if (ids is null || ids.Count == 0 || ids.Any(x => x <= 0))
            {
                return false;
            }

            var existingEntities = (repository ?? throw new ArgumentNullException(nameof(repository))).GetByIds(ids, true);
            invalidList = ids.Except(existingEntities.Select(s => s.Id)).ToList();

            return !invalidList.Any();
        }
    }
}
