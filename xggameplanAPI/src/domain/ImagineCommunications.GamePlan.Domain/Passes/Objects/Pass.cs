using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;

namespace ImagineCommunications.GamePlan.Domain.Passes.Objects
{
    public class Pass : ICloneable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public bool? IsLibraried { get; set; } = false; // TODO: Can be set to bool after DB update
        public List<General> General = new List<General>();
        public List<Weighting> Weightings = new List<Weighting>();
        public List<Tolerance> Tolerances = new List<Tolerance>();
        public List<PassRule> Rules = new List<PassRule>();
        public List<RatingPoint> RatingPoints = new List<RatingPoint>();
        public List<ProgrammeRepetition> ProgrammeRepetitions = new List<ProgrammeRepetition>();
        public List<BreakExclusion> BreakExclusions = new List<BreakExclusion>();
        public List<SlottingLimit> SlottingLimits = new List<SlottingLimit>();
        public PassSalesAreaPriority PassSalesAreaPriorities = new PassSalesAreaPriority();

        public object Clone() => MemberwiseClone();

        public static void ValidateForSave(Pass pass)
        {
            ValidateForSave(pass, null);
        }

        public static void ValidateForSave(Pass pass, IEnumerable<SalesArea> validSalesAreas)
        {
            if (string.IsNullOrEmpty(pass.Name))
            {
                throw new Exception("Pass name must be set");
            }

            if (pass.General == null || !pass.General.Any())
            {
                throw new Exception("Pass must have general settings");
            }

            foreach (var item in pass.General)
            {
                if (Int32.TryParse(item.Value, out int itemValue))
                {
                    switch (item.RuleId)
                    {
                        case (int)RuleID.MinimumEfficiency:
                        case (int)RuleID.DemographBandingTolerance:
                            {
                                if (itemValue < 0)
                                {
                                    throw new Exception(
                                        "Minimum Efficiency OR Demograph Banding Tolerance has to be greater than or equal to 0");
                                }

                                break;
                            }
                        case (int)RuleID.MaximumRank:
                            {
                                if (itemValue < 1)
                                {
                                    throw new Exception("Maximum Rank has to be greater than 0");
                                }

                                break;
                            }
                        case (int)RuleID.DefaultCentreBreakRatio:
                            {
                                if (0 > itemValue || itemValue > 100)
                                {
                                    throw new Exception("Default Centre Break Ratio has to be between 0 and 100");
                                }

                                break;
                            }
                        case (int)RuleID.UseCampaignMaxSpotRatings:
                            {
                                if (0 > itemValue || itemValue > 1)
                                {
                                    throw new Exception("Use Max Spot Ratings Set By Campaigns has to be only 0 or 1");
                                }

                                break;
                            }
                        case (int)RuleID.UseSponsorExclusivity:
                            {
                                if (0 > itemValue || itemValue > 1)
                                {
                                    throw new Exception("Sponsorship Exclusivity Set By Campaigns has to be only 0 or 1");
                                }

                                break;
                            }
                        case (int)RuleID.EvenDistributionZeroRatingSpots:
                            {
                                if (0 > itemValue || itemValue > 1)
                                {
                                    throw new Exception($"{nameof(RuleID.EvenDistributionZeroRatingSpots)} has to be between 0 and 1");
                                }

                                break;
                            }
                        case (int)RuleID.ZeroRatedBreaks:
                            {
                                if (0 > itemValue || itemValue > 2)
                                {
                                    throw new Exception($"{nameof(RuleID.ZeroRatedBreaks)} has to be between 0 and 2");
                                }

                                break;
                            }
                        case (int)RuleID.DayPartGroup:
                        {
                            if (1 > itemValue || itemValue > 3)
                            {
                                throw new Exception($"{nameof(RuleID.DayPartGroup)} has to be between 1 and 3");
                            }

                            break;
                        }
                        default:
                            break;
                    }
                }
                else
                {
                    throw new Exception("Non integer value detected for one of the General settings values");
                }
            }

            if (pass.Weightings == null || !pass.Weightings.Any())
            {
                throw new Exception("Pass must have weightings");
            }

            if (pass.Tolerances == null || !pass.Tolerances.Any())
            {
                throw new Exception("Pass must have tolerances");
            }

            if (pass.Rules == null || !pass.Rules.Any())
            {
                throw new Exception("Pass must have rules");
            }

            pass.PassSalesAreaPriorities?.Validate(validSalesAreas);

            ValidatePassRatingPoints(pass.RatingPoints);
        }

        public static void ValidateScenarioNamingUniqueness(Pass pass, IEnumerable<Scenario> parentScenarios)
        {
            if (parentScenarios.Any(x => x.Name.ToLower().Trim() == pass.Name.ToLower().Trim()))
            {
                throw new Exception("Pass inside the scenario has same name as the scenario itself");
            }
        }

        private static void ValidatePassRatingPoints(List<RatingPoint> ratingPoints)
        {
            if (ratingPoints == null)
            {
                throw new Exception("Pass ratingPoints can not be null");
            }

            var containsNotOnlyRatingPointForAllSalesAreas = ratingPoints.Any(t => t.SalesAreas != null && !t.SalesAreas.Any()) && ratingPoints.Count > 1;

            if(containsNotOnlyRatingPointForAllSalesAreas)
            {
                throw new Exception("Pass rating point for all sales areas requires to remove all other rating points");
            }

            foreach (var ratingPoint in ratingPoints)
            {
                if (ratingPoint.SalesAreas == null)
                {
                    throw new Exception("Pass rating points sales areas can not be null");
                }

                var isAnyRatingPointDaypartHasValue = ratingPoint.OffPeakValue.HasValue
                    || ratingPoint.PeakValue.HasValue || ratingPoint.MidnightToDawnValue.HasValue;

                if (!isAnyRatingPointDaypartHasValue)
                {
                    throw new Exception("Pass rating point should have at least one daypart value");
                }

                ValidateRatingPointValueForDaypart(ratingPoint.OffPeakValue, nameof(ratingPoint.OffPeakValue));

                ValidateRatingPointValueForDaypart(ratingPoint.PeakValue, nameof(ratingPoint.PeakValue));

                ValidateRatingPointValueForDaypart(ratingPoint.MidnightToDawnValue, nameof(ratingPoint.MidnightToDawnValue));

                foreach (var salesArea in ratingPoint.SalesAreas)
                {
                    var sameSalesAreaRatingPoint = ratingPoints.FirstOrDefault(t => t.SalesAreas.Contains(salesArea) && t != ratingPoint);

                    if (sameSalesAreaRatingPoint == null)
                    {
                        continue;
                    }

                    var isOverlappedDaypartValues = ratingPoint.OffPeakValue.HasValue && sameSalesAreaRatingPoint.OffPeakValue.HasValue
                        || ratingPoint.PeakValue.HasValue && sameSalesAreaRatingPoint.PeakValue.HasValue
                        || ratingPoint.MidnightToDawnValue.HasValue && sameSalesAreaRatingPoint.MidnightToDawnValue.HasValue;

                    if (isOverlappedDaypartValues)
                    {
                        throw new Exception($"Pass rating point value is overlapped by another rating point for {salesArea} sales area");
                    }
                }
            }
        }

        private static void ValidateRatingPointValueForDaypart(double? value, string daypartName)
        {
            if (value != null)
            {
                if (value <= 0)
                {
                    throw new Exception($"Pass rating point {daypartName} has to be greater than 0");
                }
            }
        }
    }
}
