using System;
using System.Collections.Generic;
using AutoFixture;
using ImagineCommunications.GamePlan.CommonTestsHelpers;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Exceptions;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using NodaTime;
using NUnit.Framework;
using xggameplan.model.External.Campaign;
using xggameplan.Model;

namespace ImagineCommunications.GamePlan.Domain.Tests.ExternalModels.Tests
{
    [TestFixture(Category = "Domain (external) models :: " + nameof(CreateCampaign))]

    public class CreateCampaignModelTests
    {
        private const string ValidBreakType = "VA-ValidBreakType";
        private const string InvalidBreakType_TooShort = "X";

        private readonly Fixture _fixture = new SafeFixture();

        [Test(Description = "BreakType::Null break type field is invalid")]
        public void NullBreakTypeFieldIsInvalid()
        {
            // Assert
            _ = Assert.Catch<BreakTypeNullException>(() =>
            {
                // Arrange
                var sample = _fixture
                    .Build<CreateCampaign>()
                    .Without(p => p.BreakType)
                    .Create();

                // Act
                sample.Validate();
            });
        }

        [Test(Description = "BreakType::White space only break type item value is invalid")]
        public void WhiteSpaceOnlyBreakTypeItemValueIsInvalid()
        {
            // Assert
            _ = Assert.Catch<BreakTypeNullException>(() =>
            {
                // Arrange
                var sample = _fixture
                    .Build<CreateCampaign>()
                    .With(p => p.BreakType, new List<string> { "   " })
                    .Create();

                // Act
                sample.Validate();
            });
        }

        [Test(Description = "BreakType::A null break type item value is invalid")]
        public void NullBreakTypeItemValueIsInvalid()
        {
            // Assert
            _ = Assert.Catch<BreakTypeNullException>(() =>
            {
                // Arrange
                var sample = _fixture
                    .Build<CreateCampaign>()
                    .With(p => p.BreakType, new List<string> { null })
                    .Create();

                // Act
                sample.Validate();
            });
        }

        [Test(Description = "BreakType::An empty break type field is valid")]
        public void EmptyBreakTypeFieldIsValid()
        {
            // Assert
            Assert.DoesNotThrow(() =>
            {
                // Arrange
                var sample = CreateValidCampaignObject();
                sample.BreakType.Clear();

                // Act
                sample.Validate();
            });
        }

        [Test(Description = "BreakType::Break type value with a length less than two is invalid")]
        public void BreakTypeValueHasLengthLessThanTwoIsInvalid()
        {
            // Assert
            _ = Assert.Catch<BreakTypeTooShortException>(() =>
            {
                // Arrange
                var sampleToTest = _fixture
                    .Build<CreateCampaign>()
                    .With(p => p.BreakType, new List<string> { InvalidBreakType_TooShort })
                    .Create();

                // Act
                sampleToTest.Validate();
            });
        }

        [Test(Description = "BreakType::Break type value with a length greater than or equal to two is valid")]
        public void BreakTypeValueHasLengthGreaterThanOrEqualToTwoIsValid()
        {
            // Assert
            Assert.DoesNotThrow(() =>
            {
                // Arrange
                var sampleToTest = CreateValidCampaignObject();

                // Act
                sampleToTest.Validate();
            });
        }

        private CreateCampaign CreateValidCampaignObject()
        {
            var campaign = new CreateCampaign()
            {
                ExternalId = "External Id",
                Name = "Campaign Name",
                DemoGraphic = "demographic",
                StartDateTime = DateTime.Today,
                EndDateTime = DateTime.Today.AddDays(1),
                Product = "product",
                TargetRatings = 1,
                DeliveryType = CampaignDeliveryType.Rating.ToString(),
                BreakType = new List<string>() { "VA-ValidBreakType" },
                SalesAreaCampaignTarget = new List<SalesAreaCampaignTargetViewModel>()
                {
                    new SalesAreaCampaignTargetViewModel()
                    {
                        SalesAreaGroup = new SalesAreaGroup()
                        {
                            GroupName ="GroupName",
                            SalesAreas =new List<string>(){ "SalesAreas" }
                        },
                        Multiparts = new List<MultipartModel>()
                        {
                            new MultipartModel()
                            {
                                Lengths = new List<Duration>
                                {
                                     Duration.FromMinutes(1)
                                },
                                DesiredPercentageSplit = 1
                            }
                        },
                        CampaignTargets = new List<CampaignTarget>(){
                            new CampaignTarget() {
                                StrikeWeights = new List<StrikeWeight>() {
                                    new StrikeWeight() {
                                        StartDate = DateTime.Now,
                                        EndDate = DateTime.Now.AddDays(1),
                                        DesiredPercentageSplit = 1,
                                        Lengths = new List<Length>()
                                        {
                                            new Length()
                                            {
                                                length = Duration.FromMinutes(1),
                                                DesiredPercentageSplit = 1
                                            }
                                        },
                                        DayParts = new List<DayPart>() {
                                            new DayPart() {
                                                DesiredPercentageSplit = 1,
                                                Timeslices = new List<Timeslice>()
                                                {
                                                    new Timeslice()
                                                    {
                                                        FromTime = "01:00",
                                                        ToTime = "02:00" ,
                                                        DowPattern = new List<string>() { "Mon" }
                                                    }
                                                },
                                                Lengths = new List<DayPartLength>
                                                {
                                                   new DayPartLength
                                                   {
                                                       DesiredPercentageSplit = 1
                                                   }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                TimeRestrictions = new List<TimeRestriction>()
                {
                    new TimeRestriction()
                    {
                        StartDateTime = DateTime.Now,
                        EndDateTime = DateTime.Now.AddDays(1),
                        DowPattern =new List<string>(){"MON"},
                        IsIncludeOrExclude = "I"
                    }
                },
                ProgrammeRestrictions = new List<ProgrammeRestriction>()
                {
                    new ProgrammeRestriction()
                    {
                        IsCategoryOrProgramme ="C",
                        IsIncludeOrExclude ="I"
                    }
                },
                CampaignGroup = "CampaignGroup",
                IncludeRightSizer = "Campaign Level",
                IncludeOptimisation = true,
                CampaignPassPriority = 2
            };

            return campaign;
        }
    }
}
