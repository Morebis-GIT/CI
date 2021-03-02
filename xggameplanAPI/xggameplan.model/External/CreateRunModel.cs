using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Domain.Runs;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using xggameplan.model.External;

namespace xggameplan.Model
{
    public class CreateRunModel : RunModelBase
    {
        public bool AddDefaultScenario { get; set; }
    }

    public class UpdateRunModel : RunModelBase
    {
    }

    public abstract class RunModelBase
    {
        public RunModelBase()
        {
            // TODO: Remove these property sets when API caller sets them in request
            Optimisation = true;    // Default
            RightSizer = false;     // Default
        }

        public Guid Id { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }
        public TimeSpan? StartTime { get; set; }

        public DateTime EndDate { get; set; }
        public TimeSpan? EndTime { get; set; }

        public bool Real { get; set; }

        public bool Smooth { get; set; }
        public DateRange SmoothDateRange { get; set; }

        public bool ISR { get; set; }
        public DateRange ISRDateRange { get; set; }

        public bool Optimisation { get; set; }
        public DateRange OptimisationDateRange { get; set; }

        public bool RightSizer { get; set; }
        public DateRange RightSizerDateRange { get; set; }

        public bool SpreadProgramming { get; set; } = false;

        public bool SkipLockedBreaks { get; set; } = false;

        public bool IgnorePremiumCategoryBreaks { get; set; } = false;

        public bool ExcludeBankHolidays { get; set; } = false;

        public bool ExcludeSchoolHolidays { get; set; } = false;

        public bool IgnoreZeroPercentageSplit { get; set; } = false;

        public bool BookTargetArea { get; set; } = false;

        public bool IsLocked { get; set; }

        public string Objectives { get; set; }

        public AuthorModel Author { get; set; }

        public EfficiencyCalculationPeriod? EfficiencyPeriod { get; set; }
        public int? NumberOfWeeks { get; set; }

        public string PositionInProgramme { get; set; }

        public List<SalesAreaPriorityModel> SalesAreaPriorities = new List<SalesAreaPriorityModel>();

        public List<CampaignReferenceModel> Campaigns = new List<CampaignReferenceModel>();

        public List<CampaignRunProcessesSettingsModel> CampaignsProcessesSettings = new List<CampaignRunProcessesSettingsModel>();

        public List<CreateRunScenarioModel> Scenarios { get; set; }

        public bool Manual { get; set; }

        public List<InventoryStatusModel> ExcludedInventoryStatuses = new List<InventoryStatusModel>();

        public List<CreateOrUpdateAnalysisGroupTargetModel> AnalysisGroupTargets = new List<CreateOrUpdateAnalysisGroupTargetModel>();

        public int BRSConfigurationTemplateId { get; set; }

        public int RunTypeId { get; set; }

        public RunScheduleSettingsModel ScheduleSettings { get; set; }
    }
}
