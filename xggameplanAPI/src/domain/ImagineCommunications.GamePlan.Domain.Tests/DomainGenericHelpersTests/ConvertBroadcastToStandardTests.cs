using System;
using ImagineCommunications.GamePlan.Domain.Generic.Helpers;
using NUnit.Framework;

namespace ImagineCommunications.GamePlan.Domain.Tests.Helpers.Tests
{
    [TestFixture(Category = "Domain Generic :: Helpers :: DateHelper")]
    public static class ConvertBroadcastToStandardTests
    {
        [Test]
        [Description("Given a datetime object and timespan object then the datetime object should be incremented by the value of the timespan if the timespan is >= 0600.")]
        public static void Given_datetime_and_timespan_then_datetime_should_increment_by_value_of_timespan_if_timespan_equal_or_later_than_0600()
        {
            //Arrange
            TimeSpan timeSpan = new TimeSpan(06, 59, 59);
            DateTime dateTime = new DateTime(2018, 07, 18);

            //Act
            DateTime newdateobject = DateHelper.ConvertBroadcastToStandard(dateTime, timeSpan);

            //Asert
            Assert.That(newdateobject.Equals(dateTime.Add(timeSpan)));
        }

        [Test]
        [Description("Given a datetime object and timespan object then the datetime object should be incremented by the value of the timespan object and 1 day should be added if the timespan is before 0600.")]
        public static void Given_datetime_and_timespan_then_datetime_should_increment_by_value_of_timespan_and_1_day_added_if_timespan_before_0600()
        {
            //Arrange
            TimeSpan timeSpan = new TimeSpan(05, 59, 59);
            DateTime dateTime = new DateTime(2018, 07, 18);

            //Act
            DateTime newdateobject = DateHelper.ConvertBroadcastToStandard(dateTime, timeSpan);

            //Asert
            Assert.That(newdateobject.Equals(dateTime.AddDays(1).Add(timeSpan)));
        }
    }
}
