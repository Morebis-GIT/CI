using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.AnalysisGroups.Objects;
using ImagineCommunications.GamePlan.Domain.RunTypes;
using xggameplan.Model;

namespace xggameplan.Validations.RunTypes
{
    public static class RunTypesValidations
    {
        /// <summary>
        /// Validation to make sure RunType name is always provided
        /// </summary>
        /// <param name="runTypeName">RunType name value</param>
        public static (bool isValid, string errorMessage) ValidateRunTypeName(string runTypeName)
        {
            if (string.IsNullOrWhiteSpace(runTypeName))
            {
                return (false, "Name cannot be empty");
            }

            return (true, "");
        }

        /// <summary>
        /// Validation to make sure KPI value passed in is part of RunTypeAnalysisGroupKPINames list
        /// </summary>
        /// <param name="kpi">Name value of the KPI</param>
        public static (bool isValid, string errorMessage) ValidateKPIName(string kpi)
        {
            if (!RunTypeAnalysisGroupKPINames.ListOfKPINames.Contains(kpi))
            {
                return (false, kpi + " KPI is not valid");
            }

            return (true, "");
        }

        /// <summary>
        /// Validation to make sure KPI value passed in is either not mandatory or part of RunTypeAnalysisGroupKPINames list
        /// </summary>
        /// <param name="defaultKPI">Name value of the default KPI property</param>
        public static (bool isValid, string errorMessage) ValidateDefaultKPIName(string defaultKPI)
        {
            if (!string.IsNullOrWhiteSpace(defaultKPI))
            {
                (bool kpiIsValid, string kpiErrorMessage) = ValidateKPIName(defaultKPI);
                if (!kpiIsValid)
                {
                    return (false, "Default KPI: " + kpiErrorMessage);
                }
            }

            return (true, "");
        }

        /// <summary>
        /// Validation for the list of RunType - AnalysisGroup relation records
        /// </summary>
        /// <param name="runTypeAnalysisGroups">List of RunType - AnalysisGroup relation records</param>
        /// <param name="allAnalysisGroups">List of all analysis group records present in the database</param>
        public static (bool isValid, string errorMessage) ValidateRunTypeAnalysisGroupList(
            IEnumerable<CreateRunTypeAnalysisGroupModel> runTypeAnalysisGroups,
            IEnumerable<AnalysisGroup> allAnalysisGroups)
        {
            if (runTypeAnalysisGroups is null)
            {
                return (true, "");
            }

            (bool listIsValid, string listErrorMessage) = RunTypesValidations.ValidateDuplicateAnalysisGroupAndKPIPairs(runTypeAnalysisGroups);
            if (!listIsValid)
            {
                return (false, listErrorMessage);
            }

            foreach (var runTypeAnalysisGroup in runTypeAnalysisGroups)
            {
                if (!allAnalysisGroups.Any(o => o.Id == runTypeAnalysisGroup.AnalysisGroupId))
                {
                    return (false, $"Analysis group \"{runTypeAnalysisGroup.AnalysisGroupName}\" no longer exist");
                }

                if (string.IsNullOrWhiteSpace(runTypeAnalysisGroup.KPI))
                {
                    return (false, "RunType-AnalysisGroup: KPI must have a value");
                }

                (bool kpiIsValid, string kpiErrorMessage) = RunTypesValidations.ValidateKPIName(runTypeAnalysisGroup.KPI);
                if (!kpiIsValid)
                {
                    return (false, "RunType-AnalysisGroup: " + kpiErrorMessage);
                }
            }

            return (true, "");
        }

        /// <summary>
        /// Validation to make sure there are no duplicated KPI and AnalysisGroupId pairs for RunType
        /// </summary>
        /// <param name="runTypeAnalysisGroups">List of RunType - AnalysisGroup relation records</param>
        public static (bool isValid, string errorMessage) ValidateDuplicateAnalysisGroupAndKPIPairs
            (IEnumerable<CreateRunTypeAnalysisGroupModel> runTypeAnalysisGroups)
        {
            var duplicates = runTypeAnalysisGroups
                .GroupBy(o => new { o.AnalysisGroupId, o.KPI })
                .Where(o => o.Skip(1).Any());

            if (duplicates != null && duplicates.Any())
            {
                var outputMessage = string.Join(", ", duplicates.Select(o =>
                    $"{runTypeAnalysisGroups.FirstOrDefault(x => x.AnalysisGroupId == o.Key.AnalysisGroupId)?.AnalysisGroupName} - {o.Key.KPI}"));

                return (false, "There are duplicate Analysis Group and KPI combinations: " + outputMessage);
            }

            return (true, "");
        }
    }
}
