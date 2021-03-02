using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.RunTypes;
using xggameplan.Model;

namespace xggameplan.tests.ValidationTests.RunTypes.Data
{
    public sealed class RunTypesValidationsTestData
    {
        private static readonly List<object[]> _kpiNames = RunTypeAnalysisGroupKPINames.ListOfKPINames.Select(o => new object[] { o, true, "" }).ToList();

        public static IEnumerable<object[]> ValidateKPINameTestCases =>
            new List<object[]>(_kpiNames)
            {
                new object[] { "", false, " KPI is not valid" },
                new object[] { null, false, " KPI is not valid" },
                new object[] { "NotReallyAKPI", false, "NotReallyAKPI KPI is not valid" },
            };

        public static IEnumerable<object[]> ValidateDefaultKPINameTestCases =>
            new List<object[]>(_kpiNames)
            {
                new object[] { "", true, "" },
                new object[] { null, true, "" },
                new object[] { "NotReallyAKPI", false, "Default KPI: NotReallyAKPI KPI is not valid" },
            };

        public static IEnumerable<object[]> validateEmptyOrNullListOfRunTypeAnalysisGroups =>
            new List<object[]>()
            {
                new object[] { null },
                new object[] { new List<CreateRunTypeAnalysisGroupModel>() }
            };
    }
}
