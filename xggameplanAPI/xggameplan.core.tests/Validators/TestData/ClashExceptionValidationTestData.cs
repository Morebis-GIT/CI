using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Validation;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions;

namespace xggameplan.core.tests.Validators.TestData
{
    public sealed class ClashExceptionValidationTestData
    {
        private const int OFFSET_HOURS = 6;

        private static DateTime UtcTomorrow => DateTime.UtcNow.Date.AddDays(1);

        public static readonly object[] ClashExceptionsOverlapValidationData =
        {
            new object[]
            {
                new[]
                {
                    new ClashException
                    {
                        Id = 1,
                        FromType = ClashExceptionType.Clash, ToType = ClashExceptionType.Clash,
                        FromValue = "TEST_FROM_1", ToValue = "TEST_TO_1",
                        StartDate = UtcTomorrow, EndDate = UtcTomorrow.AddDays(10),
                        TimeAndDows = new List<TimeAndDow> { new TimeAndDow {
                            StartTime = null,
                            EndTime = null,
                            DaysOfWeek = "1111111"
                        }}
                    }
                },
                OFFSET_HOURS,
                BunchOfExistingClashExceptions,
                CustomValidationResult.Failed("Overlapping Dates Exist for given exceptions")
            },
            new object[]
            {
                new[]
                {
                    new ClashException
                    {
                        Id = 2,
                        FromType = ClashExceptionType.Clash, ToType = ClashExceptionType.Clash,
                        FromValue = "TEST_FROM_2", ToValue = "TEST_TO_2",
                        StartDate = UtcTomorrow.AddDays(15), EndDate = UtcTomorrow.AddDays(25),
                        TimeAndDows = new List<TimeAndDow> { new TimeAndDow {
                            StartTime = null,
                            EndTime = null,
                            DaysOfWeek = "0101010"
                        }}
                    }
                },
                OFFSET_HOURS,
                BunchOfExistingClashExceptions,
                CustomValidationResult.Success()
            },
            new object[]
            {
                new[]
                {
                    new ClashException
                    {
                        Id = 3,
                        FromType = ClashExceptionType.Advertiser, ToType = ClashExceptionType.Advertiser,
                        FromValue = "TEST_FROM_3", ToValue = "TEST_TO_3",
                        StartDate = UtcTomorrow.AddDays(5), EndDate = UtcTomorrow.AddDays(20),
                        TimeAndDows = new List<TimeAndDow> { new TimeAndDow {
                            StartTime = null,
                            EndTime = null,
                            DaysOfWeek = "1111111"
                        }}
                    }
                },
                OFFSET_HOURS,
                BunchOfExistingClashExceptions,
                CustomValidationResult.Failed("Overlapping Dates Exist for given exceptions")
            }
        };

        public static readonly object[] ClashExceptionSameStructureValidation =
        {
            new object[]
            {
                new ClashException
                {
                    FromType = ClashExceptionType.Advertiser, ToType = ClashExceptionType.Advertiser
                },
                CustomValidationResult.Success()
            },
            new object[]
            {
                new ClashException
                {
                    FromType = ClashExceptionType.Product, ToType = ClashExceptionType.Product
                },
                CustomValidationResult.Success()
            },
            new object[]
            {
                new ClashException
                {
                    FromType = ClashExceptionType.Clash, ToType = ClashExceptionType.Clash,
                    FromValue = "NON_EXISTING"
                },
                CustomValidationResult.Failed("Could not find Clash with external reference NON_EXISTING")
            },
            new object[]
            {
                new ClashException
                {
                    FromType = ClashExceptionType.Clash, ToType = ClashExceptionType.Clash,
                    FromValue = "EXTERNAL_REF_1", ToValue = "NON_EXISTING"
                },
                CustomValidationResult.Failed("Could not find Clash with external reference NON_EXISTING")
            },
            new object[]
            {
                new ClashException
                {
                    FromType = ClashExceptionType.Clash, ToType = ClashExceptionType.Clash,
                    FromValue = "EXTERNAL_REF_1", ToValue = "EXTERNAL_REF_2",
                    IncludeOrExclude = IncludeOrExclude.E
                },
                CustomValidationResult.Success()
            },
            new object[]
            {
                new ClashException
                {
                    FromType = ClashExceptionType.Clash, ToType = ClashExceptionType.Clash,
                    FromValue = "EXTERNAL_REF_PARENT", ToValue = "EXTERNAL_REF_3",
                    IncludeOrExclude = IncludeOrExclude.I
                },
                CustomValidationResult.Success()
            }
        };

        private static IEnumerable<ClashException> BunchOfExistingClashExceptions => new[]
        {
            new ClashException
            {
                Id = 3,
                FromType = ClashExceptionType.Clash, ToType = ClashExceptionType.Clash,
                FromValue = "TEST_FROM_1", ToValue = "TEST_TO_1",
                StartDate = UtcTomorrow, EndDate = UtcTomorrow.AddDays(10),
                TimeAndDows = new List<TimeAndDow> { new TimeAndDow {
                    StartTime = null,
                    EndTime = null,
                    DaysOfWeek = "1111111"
                }}
            },
            new ClashException
            {
                Id = 4,
                FromType = ClashExceptionType.Product, ToType = ClashExceptionType.Product,
                FromValue = "TEST_FROM_2", ToValue = "TEST_TO_2",
                StartDate = UtcTomorrow.AddDays(10), EndDate = UtcTomorrow.AddDays(20),
                TimeAndDows = new List<TimeAndDow> { new TimeAndDow
                {
                    StartTime = null,
                    EndTime = null,
                    DaysOfWeek = "1010101"
                }}
            },
            new ClashException
            {
                Id = 5,
                FromType = ClashExceptionType.Advertiser, ToType = ClashExceptionType.Advertiser,
                FromValue = "TEST_FROM_3", ToValue = "TEST_TO_3",
                StartDate = UtcTomorrow, EndDate = UtcTomorrow.AddDays(5),
                TimeAndDows = new List<TimeAndDow> { new TimeAndDow
                {
                    StartTime = TimeSpan.Parse("07:15:00"),
                    EndTime = TimeSpan.Parse("08:30:00"),
                    DaysOfWeek = "1010101"
                }}
            }
        };

        public static IEnumerable<Clash> BunchOfClashes = new[]
        {
            new Clash {Externalref = "EXTERNAL_REF_PARENT"}, new Clash {Externalref = "EXTERNAL_REF_1", ParentExternalidentifier = "EXTERNAL_REF_PARENT"},
            new Clash {Externalref = "EXTERNAL_REF_2", ParentExternalidentifier = "EXTERNAL_REF_1"}, new Clash {Externalref = "EXTERNAL_REF_3"}
        };
    }
}
