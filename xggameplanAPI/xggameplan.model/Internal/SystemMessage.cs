using System.Collections.Generic;
using System.Net;

namespace xggameplan.Model
{
    /// <summary>
    /// Group that a system message can be a member of
    /// </summary>
    public enum SystemMessageGroups : byte
    {
        /// <summary>
        /// Messages that can be returned by called to Validate For Run.
        /// </summary>
        ValidateForExecuteRun = 0,
        /// <summary>
        /// System test result
        /// </summary>
        //SystemTestResult = 1
        /// <summary>
        /// Normal data validation (E.g. Data upload)
        /// </summary>
        //ValidateData = 2
    }

    public enum SystemMessageSeverityLevel
    {
        Error,
        Warning
    }

    /// <summary>
    /// System message
    /// </summary>
    public class SystemMessage
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Groups that message is a member of
        /// </summary>
        public List<SystemMessageGroups> MessageGroups { get; set; }

        /// <summary>
        /// Description (Multi-language)
        /// </summary>
        public Dictionary<string, string> Description { get; set; }

        public SystemMessageSeverityLevel SeverityLevel { get; set; }

        /// <summary>
        /// Link with additional information
        /// </summary>
        public string Link { get; set; }

        public HttpStatusCode HttpCode { get; set; }

        // MessageIds
        public static int SalesAreaDataMissing = 1;
        public static int MaintenanceMode = 2;
        public static int NoAutoBooksBigEnoughForRun = 3;
        public static int NoWorkingAutoBooks = 4;
        public static int AutoBookSettingsBinariesVersionotSet = 5;
        public static int NoISRSettingsForSalesArea = 6;
        public static int NoProcessingSpecified = 7;
        public static int NoRSSettingsForSalesArea = 8;
        public static int AnotherRunIsScheduledOrActive = 9;
        public static int DemographicDataMissing = 10;
        public static int OutputFileDataMissing = 11;
        public static int UniverseDataMissingForSalesArea = 12;
        public static int UniverseDataMissingForDateRangeAndSalesArea = 13;
        public static int SpotDataMissing = 14;
        public static int BreakDataMissing = 15;
        public static int ScheduleDataMissing = 16;
        public static int RatingsDataMissing = 17;
        public static int CampaignDataMissing = 18;
        public static int ProductDataMissing = 19;
        public static int ProgrammeDataMissing = 20;
        public static int RunHasNoScenarios = 21;
        public static int RunAlreadyStarted = 22;
        public static int RunScenarioHasNoPasses = 23;
        public static int NotAllowedToExecuteRun = 24;
        public static int CampaignDataInvalid = 25;
        public static int RatingsPredictionDataMissing = 26;
        public static int NoActiveCampaigns = 27;
        public static int SmoothConfigurationMissing = 28;
        public static int SmoothConfigurationInvalid = 29;
        public static int ClashDataMissing = 30;
        public static int ClearanceDataMissing = 31;
        public static int RunRestrictionsMissing = 32;
        public static int RunDateRangeInvalid = 33;
        public static int NoAutoBooksOfCorrectSizeAndNotAutoProvisioning = 34;
        public static int DataSynchronizationInProgress = 35;

        public SystemMessage(int id, List<SystemMessageGroups> messageGroups, Dictionary<string, string> description, string link, HttpStatusCode httpCode)
        {
            Id = id;
            MessageGroups = messageGroups;
            Description = description;
            Link = link;
            HttpCode = httpCode;
        }
    }
}
