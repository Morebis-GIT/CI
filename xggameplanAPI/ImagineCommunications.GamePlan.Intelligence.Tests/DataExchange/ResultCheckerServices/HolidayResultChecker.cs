using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;
using TechTalk.SpecFlow;
using SalesAreaDomainObject = ImagineCommunications.GamePlan.Domain.Shared.SalesAreas.SalesArea;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices
{
    public class HolidayResultChecker : ResultCheckerService<SalesAreaDomainObject>
    {
        private readonly ISalesAreaRepository _clashRepository;
        
        public HolidayResultChecker(ITestDataImporter dataImporter, ISalesAreaRepository clashRepository) : base(dataImporter) =>
            _clashRepository = clashRepository;
        
        public override bool CompareTargetDataToDb(string key, string fileName = null, Table tableData = null, TestDataResultOperationType operationType = 0)
        {
            var fileTestData = GenerateDataFromFile(fileName, "SalesAreas");
            var allSalesAreas = _clashRepository.GetAll().ToList();
            
            switch (operationType)
            {
                case TestDataResultOperationType.Add:
                    return fileTestData.All(testSalesArea =>
                    {
                        var existingSalesArea = allSalesAreas.FirstOrDefault(t => t.Id == testSalesArea.Id);
                        if (existingSalesArea is null)
                        {
                            throw new InvalidOperationException($"SalesArea with id {testSalesArea.Id} was not found");
                        }

                        return AreDateRangesMatched(testSalesArea.PublicHolidays, existingSalesArea.PublicHolidays) &&
                               AreDateRangesMatched(testSalesArea.SchoolHolidays, existingSalesArea.SchoolHolidays);
                    });
                default:
                    return false;
            }
        }
        
        private static bool AreDateRangesMatched(IReadOnlyCollection<DateRange> first, IReadOnlyCollection<DateRange> second)
        {
            if (first == null && second == null)
            {
                return true;
            }

            if (first is null || second is null || first.Count != second.Count)
            {
                return false;
            }

            if (first.Count == 0 && second.Count == 0)
            {
                return true;
            }

            var firstOrdered = first
                .OrderBy(c => c.Start.Date)
                .ThenBy(d => d.End.Date)
                .ToList();

            var secondOrdered = second
                .OrderBy(c => c.Start.Date)
                .ThenBy(d => d.End.Date)
                .ToList();
            
            for (int i = 0; i < firstOrdered.Count; i++)
            {
                if (firstOrdered[i].Start.Date != secondOrdered[i].Start.Date || firstOrdered[i].End.Date != secondOrdered[i].End.Date)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
