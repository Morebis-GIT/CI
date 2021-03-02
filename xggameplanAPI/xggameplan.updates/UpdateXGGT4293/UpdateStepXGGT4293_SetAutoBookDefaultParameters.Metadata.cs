using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions;

namespace xggameplan.Updates
{
    internal partial class UpdateStepXGGT4293_SetAutoBookDefaultParameters
    {
        private AutoBookDefaultParameters GetAutoBookDefaultParameters()
        {
            return new AutoBookDefaultParameters()
            {
                AgBreak = GetAgBreak(),
                AgCampaign = GetAgCampaign(),
                AgCampaignSalesArea = GetAgCampaignSalesArea(),
                AgExposure = GetAgExposure(),
                AgHfssDemos = GetAgHfssDemos(),
                AgISRTimeBand = GetAgISRTimeBand(),
                AgPeakStartEndTime = GetAgPeakStartEndTime(),
                AgProgRestriction = GetAgProgRestriction(),
                AgProgTxDetail = GetAgProgTxDetail(),
                AgRestriction = GetAgRestriction(),
                AgSpot = GetAgSpot()
            };
        }

        private AgBreak GetAgBreak()
        {
            return new AgBreak()
            {
                LongForm = "N",
                EpisNo = 0,
                Solus = "N",
                AgSalesAreaPtrRef = new AgSalesAreaPtrRef()
                {
                    ClassId = 4
                },
            };
        }

        private AgCampaign GetAgCampaign()
        {
            return new AgCampaign()
            {
                CampaignNo = 1,
                DemographicNo = 1,
                DealNo = 1,
                ProductCode = 1,
                ClearanceCode = "",
                BusinesssAreaNo = 1,
                RevenueBudget = 0,
                DeliveryCurrency = 6,
                MultiPartFlag = "N",
                StartDate = "20170828",
                EndDate = "20170903",
                RootClashCode = " ",
                ClashCode = " ",
                AdvertiserIdentifier = " ",
                ClashNo = 1,
                AgCampaignRequirement = new AgRequirement()
                {
                    Required = 64039,
                    TgtRequired = 64039,
                    SareRequired = 64039
                },
                IncludeFunctions = 8,
                NbrAgCampagignSalesArea = 1,
                MaxAgCampagignSalesArea = 1,
                CampaignSpotMaxRatings = 0
            };
        }

        private AgCampaignSalesArea GetAgCampaignSalesArea()
        {
            return new AgCampaignSalesArea()
            {
                SalesAreaNo = 13,
                ChannelGroupNo = 13,
                CampaignNo = 1,
                RevenuePercentage = 0,
                MultiPartOnly = 0,
                AgSalesAreaCampaignRequirement = new AgRequirement()
                {
                    Required = 64039,
                    SareRequired = 64039,
                    TgtRequired = 64039,
                },
                NbrAgLengths = 1,
                MaxBreaks = 0,
                NbrAgStrikeWeights = 1,
                NbrParts = 1,
                NbrAgDayParts = 1,
                CentreBreakRatio = 50,
                EndBreakRatio = 50,
                AgCampaignSalesAreaPtrRef = new AgCampaignSalesAreaPtrRef()
                {
                    ClassId = 3,
                    SalesAreaNo = 1
                }
            };
        }

        private AgExposure GetAgExposure()
        {
            return new AgExposure()
            {
                BreakSalesAreaNo = 0,
                StartTime = "0",
                EndTime = "995959",
                StartDay = 1,
                EndDay = 7
            };
        }

        private List<AgHfssDemo> GetAgHfssDemos()
        {
            return new List<AgHfssDemo>
            {
                new AgHfssDemo()
                {
                    SalesAreaNo = 0,
                    IndexType = 1,
                    BaseDemoNo = 1,
                    IndexDemoNo = 47,
                    BreakScheduledDate = "0"
                }
            };
        }

        private AgISRTimeBand GetAgISRTimeBand()
        {
            return new AgISRTimeBand()
            {
                Exclude = nameof(IncludeOrExclude.I)
            };
        }

        private AgPeakStartEndTime GetAgPeakStartEndTime()
        {
            return new AgPeakStartEndTime()
            {
                DayPartNumber = 1,
                StartDayOfDayPart = 1,
                EndDayOfDayPart = 7,
                MidPoint = -1,
                Name = "Peak Airtime"
            };
        }

        private AgProgRestriction GetAgProgRestriction()
        {
            return new AgProgRestriction()
            {
                ProgNo = 0,
                PrgcNo = 0,
                EpisNo = 0
            };
        }

        private AgProgTxDetail GetAgProgTxDetail()
        {
            return new AgProgTxDetail()
            {
                EpisodeNo = 0
            };
        }

        private AgRestriction GetAgRestriction()
        {
            return new AgRestriction()
            {
                EpisodeNo = 0
            };
        }

        private AgSpot GetAgSpot()
        {
            return new AgSpot()
            {
                BreakSalesAreaNo = 286778,
                BreakDate = "286778",
                BreakTime = "286778",
                BreakNo = 286778,
                CampaignNo = 287778,
                SpotNo = 477008891,
                Status = "S",
                MultipartIndicator = "N",
                PreempteeStatus = 10,
                PreemptorStatus = 10,
                BookingPosition = -1,
                SpotLength = 30,
                SpotSalesAreaNo = 14001,
                PriceFactor = 1.000,
                BonusSpot = "N",
                ClientPicked = "N",
                ISRLocked = 0
            };
        }
    }
}
