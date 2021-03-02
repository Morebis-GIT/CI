using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Helpers;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.System.Models;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using xggameplan.core.Extensions;

namespace xggameplan.Validations.Runs
{
    public class RunsValidations
    {
        private static readonly TimeSpan _broadcastDayStartTime = TimeSpan.FromHours(6);
        private static readonly TimeSpan _solarDayStartTime = new TimeSpan(00, 00, 00);
        private static readonly TimeSpan _solarDayEndTime = new TimeSpan(23, 59, 59);

        /// <summary>
        /// Iterate list of scenarios for validation
        /// </summary>
        /// <param name="allScenarios"></param>
        public static void ValidateScenarios(List<Scenario> allScenarios)
        {
            if (allScenarios is null || !allScenarios.Any())
            {
                return;
            }

            var errorMsgBuilder = new StringBuilder();

            foreach (var scenario in allScenarios)
            {
                var scenarioError = ValidateScenario(scenario);
                if (!string.IsNullOrWhiteSpace(scenarioError))
                {
                    errorMsgBuilder.Append(scenarioError);
                }
            }

            if (errorMsgBuilder.Length > 0)
            {
                throw new Exception(errorMsgBuilder.ToString());
            }
        }

        /// <summary>
        /// Validates scenario for passes and PassSalesAreaPriorities
        /// </summary>
        /// <param name="scenario"></param>
        /// <returns></returns>
        public static string ValidateScenario(Scenario scenario)
        {
            var errorMsgBuilder = new StringBuilder();

            if (scenario is null)
            {
                return errorMsgBuilder.ToString();
            }

            if (string.IsNullOrWhiteSpace(scenario.Name))
            {
                errorMsgBuilder.Append("Scenario name is required.");
            }

            var scenarioName = !string.IsNullOrWhiteSpace(scenario.Name) ? scenario.Name : "with no Name.";
            if (scenario.Passes is null || !scenario.Passes.Any())
            {
                errorMsgBuilder.AppendLine($"Scenario: {scenarioName} contains no passes, One or more passes is required.");
            }
            else
            {
                if (scenario.Passes.All(p => p.Id != 0) && scenario.Passes.Select(p => p.Id).Distinct().Count() != scenario.Passes.Count)
                {
                    errorMsgBuilder.AppendLine($"Scenario: {scenarioName} contains duplicate passes.");
                }
            }

            if (scenario.CampaignPassPriorities?.Any() == true)
            {
                var cppErrors = ValidateCampaignPassPriorities(scenario.CampaignPassPriorities, scenario.Passes);
                if (!string.IsNullOrWhiteSpace(cppErrors))
                {
                    errorMsgBuilder.AppendLine($"Scenario: {scenarioName} CampaignPassPriorities contains error: {cppErrors}");
                }
            }

            return errorMsgBuilder.ToString();
        }

        /// <summary>
        /// Validate CampaignPassPriority PassPriorities count vs scenarioPasses count
        /// </summary>
        /// <param name="campaignPassPriorities"></param>
        /// <param name="scenarioPasses"></param>
        /// <returns></returns>
        public static string ValidateCampaignPassPriorities(List<CampaignPassPriority> campaignPassPriorities, List<PassReference> scenarioPasses)
        {
            var errorMsgBuilder = new StringBuilder();

            if (scenarioPasses?.Any() == true)
            {
                var noOfPasses = scenarioPasses.Count;
                foreach (var cpp in campaignPassPriorities.Where(c => c.PassPriorities.Count != noOfPasses).ToList())
                {
                    errorMsgBuilder.AppendLine($"CampaignExternalId: {cpp.Campaign.ExternalId} should contain {noOfPasses.ToString()} PassPriorities.");
                }
            }

            return errorMsgBuilder.ToString();
        }

        /// <summary>
        /// Validates PassSalesAreaPriorities for salesareas, dates, times, daysofweek
        /// </summary>
        /// <param name="run"></param>
        /// <param name="pass"></param>
        /// <param name="tenantSettings"></param>
        public static string ValidatePassSalesAreaPriorities(Run run, Pass pass, TenantSettings tenantSettings, List<string> runSalesAreas)
        {
            if (run is null || pass is null || pass.PassSalesAreaPriorities is null)
            {
                return string.Empty;
            }

            var errorMsgBuilder = new StringBuilder(string.Empty);

            var passName = !string.IsNullOrWhiteSpace(pass.Name) ? pass.Name : "with no Name";

            var validationMessage = ValidateSalesAreaPrioritiesCollection(passName, pass.PassSalesAreaPriorities, runSalesAreas);

            if (!string.IsNullOrWhiteSpace(validationMessage))
            {
                errorMsgBuilder.AppendLine(validationMessage);
            }

            validationMessage = ValidateSalesAreaPrioritiesDates(passName, pass.PassSalesAreaPriorities, run.StartDate, run.EndDate);

            if (!string.IsNullOrWhiteSpace(validationMessage))
            {
                errorMsgBuilder.AppendLine(validationMessage);
            }

            validationMessage = ValidateSalesAreaPrioritiesTimes(passName, pass.PassSalesAreaPriorities, run.StartTime, run.EndTime, tenantSettings);

            if (!string.IsNullOrWhiteSpace(validationMessage))
            {
                errorMsgBuilder.AppendLine(validationMessage);
            }

            return errorMsgBuilder.ToString();
        }

        /// <summary>
        /// Check that PassSalesAreaPriorities.SalesAreaPriorities are a subset
        /// of this run.SalesAreaPriorities
        /// </summary>
        /// <param name="passName"></param>
        /// <param name="passSalesAreaPriority"></param>
        /// <param name="runSalesAreas"></param>
        /// <returns></returns>
        public static string ValidateSalesAreaPrioritiesCollection(string passName, PassSalesAreaPriority passSalesAreaPriority, List<string> runSalesAreas)
        {
            if (passSalesAreaPriority.SalesAreaPriorities is null || !passSalesAreaPriority.SalesAreaPriorities.Any())
            {
                return string.Empty;
            }

            var passSalesAreas = passSalesAreaPriority.SalesAreaPriorities
                .Where(sa => sa.Priority != SalesAreaPriorityType.Exclude)
                .Select(x => x.SalesArea)
                .ToList();

            var passSalesAreasNotInRunSalesAreas = passSalesAreas.Except(runSalesAreas).ToArray();

            return passSalesAreasNotInRunSalesAreas.Any()
                ? $"Pass: {passName}, contains SalesAreaPriorities for SalesArea(s):[{string.Join(", ", passSalesAreasNotInRunSalesAreas)}] which are not found in the Run level SalesAreaPriorities"
                : string.Empty;
        }

        /// <summary>
        /// Check that PassSalesAreaPriorities dates are valid and are contained
        /// within run dates range
        /// </summary>
        /// <param name="passName"></param>
        /// <param name="passSalesAreaPriority"></param>
        /// <param name="runStartDate"></param>
        /// <param name="runEndDate"></param>
        /// <returns></returns>
        public static string ValidateSalesAreaPrioritiesDates(
            string passName,
            PassSalesAreaPriority passSalesAreaPriority,
            DateTime runStartDate,
            DateTime runEndDate)
        {
            if (passSalesAreaPriority.StartDate > passSalesAreaPriority.EndDate)
            {
                return $"Pass: {passName}, passSalesAreaPriority StartDate is greater than EndDate";
            }

            if ((runStartDate.Date > passSalesAreaPriority.StartDate) || (runEndDate.Date < passSalesAreaPriority.EndDate))
            {
                return $"Pass: {passName}, SalesAreaPriorities has StartDate/EndDate not within run StartDate/EndDate";
            }

            return string.Empty;
        }

        /// <summary>
        /// Check that either both or none of PassSalesAreaPriority times have
        /// value, that they are valid times of the day and contained within run StartTime/EndTime
        /// </summary>
        /// <param name="passName"></param>
        /// <param name="passSalesAreaPriority"></param>
        /// <param name="runStartTime"></param>
        /// <param name="runEndTime"></param>
        /// <param name="tenantSettings"></param>
        /// <returns></returns>
        public static string ValidateSalesAreaPrioritiesTimes(
            string passName,
            PassSalesAreaPriority
            passSalesAreaPriority,
            TimeSpan runStartTime,
            TimeSpan runEndTime,
            TenantSettings tenantSettings)
        {
            // XGGT-17121: Request to allow midnight-to-midnight runs and passes
            if (runStartTime != _solarDayStartTime && runEndTime != _solarDayEndTime)
            {
                runStartTime = TimeHelper.ConvertToBroadcast(runStartTime);
                runEndTime = TimeHelper.ConvertToBroadcast(runEndTime);
            }

            var isAnyDaypartSelected = passSalesAreaPriority.IsOffPeakTime
                                       || passSalesAreaPriority.IsPeakTime
                                       || passSalesAreaPriority.IsMidnightTime;

            if (isAnyDaypartSelected)
            {
                return ValidatePassSalesAreaPrioritiesDayparts(passName, passSalesAreaPriority, runStartTime, runEndTime, tenantSettings);
            }

            if (!passSalesAreaPriority.StartTime.HasValue && !passSalesAreaPriority.EndTime.HasValue)
            {
                return String.Empty;
            }

            if (!passSalesAreaPriority.StartTime.HasValue || !passSalesAreaPriority.EndTime.HasValue)
            {
                return $"Pass: {passName}, SalesAreaPriorities StartTime/EndTime is not valid";
            }

            if ((passSalesAreaPriority.StartTime < _solarDayStartTime)
                || (passSalesAreaPriority.StartTime > _solarDayEndTime)
                || (passSalesAreaPriority.EndTime < _solarDayStartTime)
                || (passSalesAreaPriority.EndTime > _solarDayEndTime))
            {
                return $"Pass: {passName}, SalesAreaPriorities StartTime/EndTime are invalid";
            }

            // XGGT-17121: Request to allow midnight-to-midnight runs and passes
            var passStartTime = passSalesAreaPriority.StartTime.Value;
            var passEndTime = passSalesAreaPriority.EndTime.Value;

            if (runStartTime != _solarDayStartTime && runStartTime != _solarDayEndTime)
            {
                passStartTime = TimeHelper.ConvertToBroadcast(passStartTime);
                passEndTime = TimeHelper.ConvertToBroadcast(passEndTime);
            }

            var isPassWithinRun = CheckTimesWithinRange(passStartTime, passEndTime, runStartTime, runEndTime);

            if (!isPassWithinRun)
            {
                return $"Pass: {passName}, SalesAreaPriorities StartTime/EndTime not within run StartTime/EndTime";
            }

            return String.Empty;
        }

        /// <summary>
        /// Validates PassSalesAreaPriorities dayparts to be within run StartTime/EndTime
        /// </summary>
        /// <param name="pass"></param>
        /// <param name="runStartTime"></param>
        /// <param name="runEndTime"></param>
        /// <param name="tenantSettings"></param>
        public static string ValidatePassSalesAreaPrioritiesDayparts(
            string passName,
            PassSalesAreaPriority passSalesAreaPriority,
            TimeSpan runStartTime,
            TimeSpan runEndTime,
            TenantSettings tenantSettings)
        {
            var daypartErrorMsgBuilder = new StringBuilder();
            string daypartValidationMessage;

            if (passSalesAreaPriority.IsOffPeakTime)
            {
                daypartValidationMessage = ValidatePassSalesAreaPrioritiesDaypart(Dayparts.OffPeak, runStartTime, runEndTime, tenantSettings, passName);

                if (!string.IsNullOrWhiteSpace(daypartValidationMessage))
                {
                    daypartErrorMsgBuilder.AppendLine(daypartValidationMessage);
                }
            }

            if (passSalesAreaPriority.IsPeakTime)
            {
                daypartValidationMessage = ValidatePassSalesAreaPrioritiesDaypart(Dayparts.Peak, runStartTime, runEndTime, tenantSettings, passName);

                if (!string.IsNullOrWhiteSpace(daypartValidationMessage))
                {
                    daypartErrorMsgBuilder.AppendLine(daypartValidationMessage);
                }
            }

            if (passSalesAreaPriority.IsMidnightTime)
            {
                daypartValidationMessage = ValidatePassSalesAreaPrioritiesDaypart(Dayparts.MidnightToDawn, runStartTime, runEndTime, tenantSettings, passName);

                if (!string.IsNullOrWhiteSpace(daypartValidationMessage))
                {
                    daypartErrorMsgBuilder.AppendLine(daypartValidationMessage);
                }
            }

            return daypartErrorMsgBuilder.ToString();
        }

        /// <summary>
        /// Validates PassSalesAreaPriorities daypart to be within run StartTime/EndTime
        /// </summary>
        /// <param name="daypart"></param>
        /// <param name="runStartTime"></param>
        /// <param name="runEndTime"></param>
        /// <param name="tenantSettings"></param>
        /// <param name="passName"></param>
        public static string ValidatePassSalesAreaPrioritiesDaypart(
            Dayparts daypart,
            TimeSpan runStartTime,
            TimeSpan runEndTime,
            TenantSettings tenantSettings,
            string passName)
        {
            TimeSpan startTime = default;
            TimeSpan endTime = default;

            if (daypart == Dayparts.OffPeak)
            {
                startTime = _broadcastDayStartTime;

                // since potentially we can have 3 off-peak time segments:
                // 06:00-peak, peak-midnight and midnight-05:59
                // we should check if midnight time setting ends with the broadcast day at 05:59:59
                var midnightEndTime = AgConversions.ParseTotalHHMMSSFormat(tenantSettings.MidnightEndTime, false);
                var broadcastDayEndTime = _broadcastDayStartTime - TimeSpan.FromSeconds(1);

                endTime = midnightEndTime == broadcastDayEndTime
                    ? AgConversions.ParseTotalHHMMSSFormat(tenantSettings.MidnightStartTime, false).Subtract(new TimeSpan(0, 0, 1))
                    : broadcastDayEndTime + TimeSpan.FromDays(1);
            }
            else if (daypart == Dayparts.Peak)
            {
                startTime += AgConversions.ParseTotalHHMMSSFormat(tenantSettings.PeakStartTime, false);
                endTime += AgConversions.ParseTotalHHMMSSFormat(tenantSettings.PeakEndTime, false);
            }
            else if (daypart == Dayparts.MidnightToDawn)
            {
                var midnightStartTime = AgConversions.ParseTotalHHMMSSFormat(tenantSettings.MidnightStartTime, false);
                var midnightEndTime = AgConversions.ParseTotalHHMMSSFormat(tenantSettings.MidnightEndTime, false);

                startTime = TimeHelper.ConvertToBroadcast(midnightStartTime);
                endTime = TimeHelper.ConvertToBroadcast(midnightEndTime);
            }

            bool isPassTimesWithinRunRange = CheckTimesWithinRange(startTime, endTime, runStartTime, runEndTime);

            if (!isPassTimesWithinRunRange)
            {
                return $"Pass: {passName}, SalesAreaPriorities {daypart.ToString()} daypart is not within run StartTime/EndTime";
            }

            return String.Empty;
        }

        private static bool CheckTimesWithinRange(
            TimeSpan innerRangeStartTime,
            TimeSpan innerRangeEndTime,
            TimeSpan outerRangeStartTime,
            TimeSpan outerRangeEndTime)
        {
            var isStartTimeWithinRun = innerRangeStartTime >= outerRangeStartTime && innerRangeStartTime <= outerRangeEndTime;
            var isEndTimeWithinRun = innerRangeEndTime >= outerRangeStartTime && innerRangeEndTime <= outerRangeEndTime;

            return isStartTimeWithinRun && isEndTimeWithinRun;
        }
    }
}
