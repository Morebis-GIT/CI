using System.Linq;
using ImagineCommunications.GamePlan.Domain.PositionInBreaks.BookingPositionGroups;
using ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;
using TechTalk.SpecFlow;
using BookingPositionGroupDbObject = ImagineCommunications.GamePlan.Domain.PositionInBreaks.BookingPositionGroups.Objects.BookingPositionGroup;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices.BookingPositionGroup
{
    public class BookingPositionGroupResultChecker : ResultCheckerService<BookingPositionGroupDbObject>
    {
        private readonly IBookingPositionGroupRepository _bookingPositionGroupRepository;

        public BookingPositionGroupResultChecker(ITestDataImporter dataImporter, IBookingPositionGroupRepository bookingPositionGroupRepository) : base(dataImporter) =>
            _bookingPositionGroupRepository = bookingPositionGroupRepository;

        public override bool CompareTargetDataToDb(string key, string fileName = null, Table tableData = null, TestDataResultOperationType operationType = default)
        {
            var featureTestData = GenerateDataFromTable(tableData);
            var fileTestData = GenerateDataFromFile(fileName, key);
            var positionGroups = _bookingPositionGroupRepository.GetAll().ToList();

            switch (operationType)
            {
                case TestDataResultOperationType.Add:
                {
                    featureTestData.AddRange(fileTestData);
                    return positionGroups.Count == featureTestData.Count && featureTestData.All(entity => positionGroups.Count(c => AreSameBookingPositionGroups(c, entity)) == 1);
                }
                case TestDataResultOperationType.Remove:
                case TestDataResultOperationType.Replace:
                    return true;
                default:
                    return false;
            }
        }

        private static bool AreSameBookingPositionGroups(BookingPositionGroupDbObject source, BookingPositionGroupDbObject target) =>
            source.GroupId == target.GroupId &&
            source.Code == target.Code &&
            source.Description == target.Description &&
            source.DisplayOrder == target.DisplayOrder &&
            source.UserDefined == target.UserDefined &&
            source.PositionGroupAssociations.Count == target.PositionGroupAssociations.Count;
    }
}
