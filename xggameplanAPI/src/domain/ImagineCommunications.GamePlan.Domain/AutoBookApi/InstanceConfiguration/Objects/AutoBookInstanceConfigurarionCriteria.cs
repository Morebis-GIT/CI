using System;

namespace ImagineCommunications.GamePlan.Domain.AutoBookApi.InstanceConfiguration.Objects
{
    /// <summary>
    /// Single criteria for an AutoBook to be suitable to be able to execute run
    ///
    /// The importance of the factors starting with most important is breaks, campaigns.
    /// </summary>
    public class AutoBookInstanceConfigurationCriteria
    {
        public int? MaxDays { get; set; }

        public int? MaxSalesAreas { get; set; }

        public int? MaxDemographics { get; set; }

        public int? MaxCampaigns { get; set; }

        public int? MaxBreaks { get; set; }

        /// <summary>
        /// Whether run parameters meet this criteria
        /// </summary>
        /// <param name="days"></param>
        /// <param name="campaigns"></param>
        /// <returns></returns>
        public bool MeetsCriteria(int days, int salesAreas, int campaigns, int demographics, int breaks)
        {
            bool meetsCriteria = false;
            if (days <= MaxDays.GetValueOrDefault(Int32.MaxValue))
            {
                if (salesAreas <= MaxSalesAreas.GetValueOrDefault(Int32.MaxValue))
                {
                    if (demographics <= MaxDemographics.GetValueOrDefault(Int32.MaxValue))
                    {
                        if (campaigns <= MaxCampaigns.GetValueOrDefault(Int32.MaxValue))
                        {
                            if (breaks <= MaxBreaks.GetValueOrDefault(Int32.MaxValue))
                            {
                                meetsCriteria = true;
                            }
                        }
                    }
                }
            }
            return meetsCriteria;
        }
    }
}
