using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using NUnit.Framework;
using xggameplan.KPIProcessing.KPICalculation.Infrastructure;

namespace xggameplan.core.tests.KPIProcessing
{
    [TestFixture]
    public class KPICalculationHelpersTests
    {
        private List<DayPart> _dayParts;

        [SetUp]
        public void Init()
        {
            _dayParts = new List<DayPart>
            {
                new DayPart
                {
                    DayPartName = "2100-2359(Mon-Fri)",
                    Timeslices = new List<Timeslice>
                    {
                        new Timeslice
                        {
                            DowPattern = new List<string>
                            {
                                "Mon",
                                "Tue",
                                "Wed",
                                "Thu",
                                "Fri"
                            },
                            FromTime = "21:00",
                            ToTime = "23:59"
                        }
                    }
                },
                new DayPart
                {
                    DayPartName = "2100-2659(Mon-Fri)",
                    Timeslices = new List<Timeslice>
                    {
                        new Timeslice
                        {
                            DowPattern = new List<string>
                            {
                                "Mon",
                                "Tue",
                                "Wed",
                                "Thu",
                                "Fri"
                            },
                            FromTime = "21:00",
                            ToTime = "02:59"
                        }
                    }
                },
                new DayPart
                {
                    DayPartName = "1000-1259(Mon-Mon)",
                    Timeslices = new List<Timeslice>
                    {
                        new Timeslice
                        {
                            DowPattern = new List<string>
                            {
                                "Mon",
                            },
                            FromTime = "10:00",
                            ToTime = "12:59"
                        }
                    }
                }
            };
        }

        [Test]
        [Description("Get DayPart By DOW and time string should return DayPart")]
        public void GetDayPartByDowAndTimeString_ShouldReturnDayPart()
        {
            //Arrange
            var expected = new DayPart
            {
                DayPartName = "2100-2359(Mon-Fri)",
                Timeslices = new List<Timeslice>
                    {
                        new Timeslice
                        {
                            DowPattern = new List<string>
                            {
                                "Mon",
                                "Tue",
                                "Wed",
                                "Thu",
                                "Fri"
                            },
                            FromTime = "21:00",
                            ToTime = "23:59"
                        }
                    }
            };

            //Act
            var result = KPICalculationHelpers.GetDayPartByDowTimeString(_dayParts,
                expected.DayPartName);

            //Assert
            Assert.IsTrue(result.DayPartName == expected.DayPartName);
        }

        [Test]
        [Description("Get DayPart By DOW and time string and time is next day should return DayPart")]
        public void GetDayPartByDowAndTimeStringAndTimeIsNextDay_ShouldReturnDayPart()
        {
            //Arrange
            var expected = new DayPart
            {
                DayPartName = "2100-2659(Mon-Fri)",
                Timeslices = new List<Timeslice>
                    {
                        new Timeslice
                        {
                            DowPattern = new List<string>
                            {
                                "Mon",
                                "Tue",
                                "Wed",
                                "Thu",
                                "Fri"
                            },
                            FromTime = "21:00",
                            ToTime = "02:59"
                        }
                    }
            };

            //Act
            var result = KPICalculationHelpers.GetDayPartByDowTimeString(_dayParts,
                expected.DayPartName);

            //Assert
            Assert.IsTrue(result.DayPartName == expected.DayPartName);
        }

        [Test]
        [Description("Get DayPart By DOW and time string and only one DOW should return DayPart")]
        public void GetDayPartByDowAndTimeStringAndOnlyOneDow_ShouldReturnDayPart()
        {
            //Arrange
            var expected = new DayPart
            {
                DayPartName = "1000-1259(Mon-Mon)",
                Timeslices = new List<Timeslice>
                    {
                        new Timeslice
                        {
                            DowPattern = new List<string>
                            {
                                "Mon"
                            },
                            FromTime = "10:00",
                            ToTime = "12:59"
                        }
                    }
            };

            //Act
            var result = KPICalculationHelpers.GetDayPartByDowTimeString(_dayParts,
                expected.DayPartName);

            //Assert
            Assert.IsTrue(result.DayPartName == expected.DayPartName);
        }

        [Test]
        [Description("Get DayPart By DOW and time string and DayPart does not exist should return null")]
        public void GetDayPartByDowAndTimeStringAndDayPartDoesNotExist_ShouldReturnNull()
        {
            //Arrange
            var dowTimeString = "2000-2559(Mon-Tue)";

            //Act
            var result = KPICalculationHelpers.GetDayPartByDowTimeString(_dayParts,
                dowTimeString);

            //Assert
            Assert.IsNull(result);
        }
    }
}
