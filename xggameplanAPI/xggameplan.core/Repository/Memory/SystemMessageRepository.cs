using System.Collections.Generic;
using System.Linq;
using System.Net;
using ImagineCommunications.GamePlan.Domain.Generic;
using xggameplan.Model;

namespace xggameplan.Repository.Memory
{
    /// <summary>
    /// In memory repository for system messages
    /// </summary>
    public class SystemMessageRepository : ISystemMessageRepository
    {
        public SystemMessage Get(int id)
        {
            return GetAll().Where(sm => sm.Id == id).FirstOrDefault();
        }

        public List<SystemMessage> GetAll()
        {
            string[] languages = Globals.SupportedLanguages;
            List<SystemMessage> messages = new List<SystemMessage>
            {
                new SystemMessage(SystemMessage.AnotherRunIsScheduledOrActive, new List<SystemMessageGroups>() { SystemMessageGroups.ValidateForExecuteRun }, GetDescription("Cannot start the run because another run is already running", languages), null, HttpStatusCode.Conflict),
                new SystemMessage(SystemMessage.AutoBookSettingsBinariesVersionotSet, new List<SystemMessageGroups>() { SystemMessageGroups.ValidateForExecuteRun }, GetDescription("Cannot start the run because the AutoBook settings do not indicate which version of the Optimiser binaries to use", languages), null, HttpStatusCode.InternalServerError),
                new SystemMessage(SystemMessage.BreakDataMissing, new List<SystemMessageGroups>() { SystemMessageGroups.ValidateForExecuteRun }, GetDescription("Cannot start the run because break data is missing", languages), null, HttpStatusCode.InternalServerError),
                new SystemMessage(SystemMessage.CampaignDataMissing, new List<SystemMessageGroups>() { SystemMessageGroups.ValidateForExecuteRun }, GetDescription("Cannot start the run because campaign data is missing", languages), null, HttpStatusCode.InternalServerError),
                new SystemMessage(SystemMessage.DemographicDataMissing, new List<SystemMessageGroups>() { SystemMessageGroups.ValidateForExecuteRun }, GetDescription("Cannot start the run because the demographics list is empty", languages), null, HttpStatusCode.InternalServerError),
                new SystemMessage(SystemMessage.MaintenanceMode, new List<SystemMessageGroups>() { SystemMessageGroups.ValidateForExecuteRun }, GetDescription("Cannot start the run because the system is in Maintenance Mode", languages), null, HttpStatusCode.InternalServerError),
                new SystemMessage(SystemMessage.NoISRSettingsForSalesArea, new List<SystemMessageGroups>() { SystemMessageGroups.ValidateForExecuteRun }, GetDescription("Cannot start the run because there are no ISR settings for sales area {sales_area_name}", languages), null, HttpStatusCode.InternalServerError),
                new SystemMessage(SystemMessage.NoProcessingSpecified, new List<SystemMessageGroups>() { SystemMessageGroups.ValidateForExecuteRun }, GetDescription("Cannot start run the because Optimisation, ISR or Right Sizer must be selected", languages), null, HttpStatusCode.InternalServerError),
                new SystemMessage(SystemMessage.NoRSSettingsForSalesArea, new List<SystemMessageGroups>() { SystemMessageGroups.ValidateForExecuteRun }, GetDescription("Cannot start the run because there are no Right Sizer settings for sales area {sales_area_name}", languages), null, HttpStatusCode.InternalServerError),
                new SystemMessage(SystemMessage.NoWorkingAutoBooks, new List<SystemMessageGroups>() { SystemMessageGroups.ValidateForExecuteRun }, GetDescription("Cannot start the run because there are no working AutoBooks and AutoBooks are configured to be manually provisioned", languages), null, HttpStatusCode.InternalServerError),
                new SystemMessage(SystemMessage.OutputFileDataMissing, new List<SystemMessageGroups>() { SystemMessageGroups.ValidateForExecuteRun }, GetDescription("Cannot start the run because the output file list is empty", languages), null, HttpStatusCode.InternalServerError),
                new SystemMessage(SystemMessage.ProductDataMissing, new List<SystemMessageGroups>() { SystemMessageGroups.ValidateForExecuteRun }, GetDescription("Cannot start the run because product data is missing", languages), null, HttpStatusCode.InternalServerError),
                new SystemMessage(SystemMessage.ProgrammeDataMissing, new List<SystemMessageGroups>() { SystemMessageGroups.ValidateForExecuteRun }, GetDescription("Cannot start the run because programme data is missing", languages), null, HttpStatusCode.InternalServerError),
                new SystemMessage(SystemMessage.RatingsDataMissing, new List<SystemMessageGroups>() { SystemMessageGroups.ValidateForExecuteRun }, GetDescription("Cannot start the run because ratings data is missing", languages), null, HttpStatusCode.InternalServerError),
                new SystemMessage(SystemMessage.NoAutoBooksBigEnoughForRun, new List<SystemMessageGroups>() { SystemMessageGroups.ValidateForExecuteRun }, GetDescription("Cannot start the run because there are no AutoBook configurations that have sufficient resources to execute the run", languages), null, HttpStatusCode.InternalServerError),
                new SystemMessage(SystemMessage.SalesAreaDataMissing, new List<SystemMessageGroups>() { SystemMessageGroups.ValidateForExecuteRun }, GetDescription("Cannot start the run because no sales areas have been defined", languages), null, HttpStatusCode.InternalServerError),
                new SystemMessage(SystemMessage.ScheduleDataMissing, new List<SystemMessageGroups>() { SystemMessageGroups.ValidateForExecuteRun }, GetDescription("Cannot start the run because schedule data is missing", languages), null, HttpStatusCode.InternalServerError),
                new SystemMessage(SystemMessage.SpotDataMissing, new List<SystemMessageGroups>() { SystemMessageGroups.ValidateForExecuteRun }, GetDescription("Cannot start the run because spot data is missing", languages), null, HttpStatusCode.InternalServerError),
                new SystemMessage(SystemMessage.UniverseDataMissingForDateRangeAndSalesArea, new List<SystemMessageGroups>() { SystemMessageGroups.ValidateForExecuteRun }, GetDescription("Warning, run will start but might fail because there is insufficient universe data for the run date range for sales area {sales_area_name}", languages), null, HttpStatusCode.InternalServerError),
                new SystemMessage(SystemMessage.UniverseDataMissingForSalesArea, new List<SystemMessageGroups>() { SystemMessageGroups.ValidateForExecuteRun }, GetDescription("Warning, run will start but might fail because the universe list is empty for sales area {sales_area_name}", languages), null, HttpStatusCode.InternalServerError),
                new SystemMessage(SystemMessage.RunAlreadyStarted, new List<SystemMessageGroups>() { SystemMessageGroups.ValidateForExecuteRun }, GetDescription("Cannot start the run because it has already been started", languages), null, HttpStatusCode.Conflict),
                new SystemMessage(SystemMessage.RunHasNoScenarios, new List<SystemMessageGroups>() { SystemMessageGroups.ValidateForExecuteRun }, GetDescription("Cannot start the run because no scenarios have been defined", languages), null, HttpStatusCode.InternalServerError),
                new SystemMessage(SystemMessage.RunScenarioHasNoPasses, new List<SystemMessageGroups>() { SystemMessageGroups.ValidateForExecuteRun }, GetDescription("Cannot start the run because at least one scenario has no passes defined", languages), null, HttpStatusCode.InternalServerError),
                new SystemMessage(SystemMessage.NotAllowedToExecuteRun, new List<SystemMessageGroups>() { SystemMessageGroups.ValidateForExecuteRun }, GetDescription("Cannot start the run because it is not allowed", languages), null, HttpStatusCode.InternalServerError),
                new SystemMessage(SystemMessage.CampaignDataInvalid, new List<SystemMessageGroups>() { SystemMessageGroups.ValidateForExecuteRun }, GetDescription("Cannot start the run because data for campaign {campaign_name} is invalid: {invalid_data}", languages), null, HttpStatusCode.InternalServerError),
                new SystemMessage(SystemMessage.RatingsPredictionDataMissing, new List<SystemMessageGroups>() { SystemMessageGroups.ValidateForExecuteRun }, GetDescription("Warning, run will start but might fail because of ratings prediction: {missing_rating}", languages), null, HttpStatusCode.InternalServerError),
                new SystemMessage(SystemMessage.NoActiveCampaigns, new List<SystemMessageGroups>() { SystemMessageGroups.ValidateForExecuteRun }, GetDescription("Cannot start the run because there are no active campaigns", languages), null, HttpStatusCode.InternalServerError),
                new SystemMessage(SystemMessage.SmoothConfigurationMissing, new List<SystemMessageGroups>() { SystemMessageGroups.ValidateForExecuteRun }, GetDescription("Cannot start the run because the Smooth Configuration is missing", languages), null, HttpStatusCode.InternalServerError),
                new SystemMessage(SystemMessage.SmoothConfigurationInvalid, new List<SystemMessageGroups>() { SystemMessageGroups.ValidateForExecuteRun }, GetDescription("Cannot start the run because the Smooth configuration is invalid: {invalid_data}", languages), null, HttpStatusCode.InternalServerError),
                new SystemMessage(SystemMessage.ClearanceDataMissing, new List<SystemMessageGroups>() { SystemMessageGroups.ValidateForExecuteRun }, GetDescription("Cannot start the run because clearance code data is missing", languages), null, HttpStatusCode.InternalServerError),
                new SystemMessage(SystemMessage.ClashDataMissing, new List<SystemMessageGroups>() { SystemMessageGroups.ValidateForExecuteRun }, GetDescription("Cannot start the run because clash data is missing", languages), null, HttpStatusCode.InternalServerError),
                new SystemMessage(SystemMessage.RunRestrictionsMissing, new List<SystemMessageGroups>() { SystemMessageGroups.ValidateForExecuteRun }, GetDescription("Cannot start the run because restrictions settings not found", languages), null, HttpStatusCode.InternalServerError),
                new SystemMessage(SystemMessage.RunDateRangeInvalid, new List<SystemMessageGroups>() { SystemMessageGroups.ValidateForExecuteRun }, GetDescription("Cannot start the run because date range is no longer valid, please update", languages), null, HttpStatusCode.InternalServerError),
                new SystemMessage(SystemMessage.NoAutoBooksOfCorrectSizeAndNotAutoProvisioning, new List<SystemMessageGroups>() { SystemMessageGroups.ValidateForExecuteRun }, GetDescription("Cannot start the run because there are no AutoBooks of appropriate size and Auto Provisioning is not enabled", languages), null, HttpStatusCode.InternalServerError),
                new SystemMessage(SystemMessage.DataSynchronizationInProgress, new List<SystemMessageGroups>() { SystemMessageGroups.ValidateForExecuteRun }, GetDescription("Cannot start the run because data synchronization is in progress", languages), null, HttpStatusCode.InternalServerError)
            };
            return messages;
        }

        public List<SystemMessage> GetByGroup(SystemMessageGroups group)
        {
            List<SystemMessage> messages = GetAll();
            return messages.Where(m => m.MessageGroups.Contains(group)).ToList();
        }

        /// <summary>
        /// Returns default descriptions
        /// </summary>
        /// <param name="defaultDescription"></param>
        /// <param name="languages"></param>
        /// <returns></returns>
        private static Dictionary<string, string> GetDescription(string defaultDescription, string[] languages)
        {
            Dictionary<string, string> description = new Dictionary<string, string>();
            languages.ToList().ForEach(language => description.Add(language, defaultDescription));
            return description;
        }
    }
}
