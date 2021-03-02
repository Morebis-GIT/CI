using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Helpers;

namespace ImagineCommunications.GamePlan.Domain.Optimizer.ISRSettings.Objects
{
    /// <summary>
    /// Inefficient Spot Removal settings
    /// </summary>
    public class ISRSettings
    {
        public int Id { get; set; }
        public string SalesArea { get; set; }
        public int DefaultEfficiencyThreshold { get; set; }
        //public List<string> BreakTypes { get; set; }    // Should it be a single break type?
        public string BreakType { get; set; }
        public List<DayOfWeek> SelectableDays { get; set; } = new List<DayOfWeek>();    // TODO: Make Bitwise?: MTWTFSS
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public bool ExcludePublicHolidays { get; set; } // Dates from SalesArea.PublicHolidays
        public bool ExcludeSchoolHolidays { get; set; }	// Dates from SalesArea.SchoolHolidays

        public List<ISRDemographicSettings> DemographicsSettings = new List<ISRDemographicSettings>();

        /// <summary>
        /// Updates from other ISRSettings instances, leaves Id & SalesArea
        /// Update Mode:
        /// 0=Current sales area only (include demographics) - Not passed through
        /// 1=All sales areas (include demographics)
        /// 2=All sales areas (exclude demographics)
        /// 3=All sales areas (Demographics only)
        /// </summary>
        /// <param name="isrSettings"></param>
        /// <param name="updateMode"></param>
        public void UpdateFrom(ISRSettings isrSettings, int updateMode)
        {
            // Update main settings
            if (updateMode == 1 || updateMode == 2)
            {
                DefaultEfficiencyThreshold = isrSettings.DefaultEfficiencyThreshold;
                BreakType = isrSettings.BreakType;
                SelectableDays.Clear();
                isrSettings.SelectableDays.ForEach(sd => SelectableDays.Add(sd));
                StartTime = isrSettings.StartTime;
                EndTime = isrSettings.EndTime;
                ExcludePublicHolidays = isrSettings.ExcludePublicHolidays;
                ExcludeSchoolHolidays = isrSettings.ExcludeSchoolHolidays;
            }

            // Update demographics
            if (updateMode == 1 || updateMode == 3)
            {
                DemographicsSettings.Clear();
                isrSettings.DemographicsSettings.ForEach(ds => DemographicsSettings.Add(new ISRDemographicSettings() { DemographicId = ds.DemographicId, EfficiencyThreshold = ds.EfficiencyThreshold }));
            }
        }

        /// <summary>
        /// Returns whether instance has same settings as input
        /// Compare Mode:
        /// 0=Full settings comparison (include demographics)
        /// 1=Top level settings comparison (exclude demographics)
        /// 2=Demographic settings only
        /// </summary>
        /// <param name="isrSettings"></param>
        /// <param name="compareMode"></param>
        /// <returns></returns>
        public bool IsSame(ISRSettings isrSettings, int compareMode)
        {
            // Compare main settings
            if (compareMode == 0 || compareMode == 1)
            {
                if ((DefaultEfficiencyThreshold != isrSettings.DefaultEfficiencyThreshold) ||
                    (BreakType != isrSettings.BreakType) ||
                    (!DateHelper.IsSame(SelectableDays, isrSettings.SelectableDays)) ||
                    (!DateHelper.IsSame(StartTime, isrSettings.StartTime)) ||
                    (!DateHelper.IsSame(EndTime, isrSettings.EndTime)) ||
                    (ExcludePublicHolidays != isrSettings.ExcludePublicHolidays) ||
                    (ExcludeSchoolHolidays != isrSettings.ExcludeSchoolHolidays))
                {
                    return false;
                }
            }

            // Compare demographics
            if (compareMode == 0 || compareMode == 2)
            {
                return ISRDemographicSettings.IsSame(DemographicsSettings, isrSettings.DemographicsSettings);
            }
            return true;
        }

        public static void ValidateForSave(ISRSettings isrSettings)
        {
            if (String.IsNullOrEmpty(isrSettings.SalesArea))
            {
                throw new Exception("Sales Area is not set");
            }
            if (String.IsNullOrEmpty(isrSettings.BreakType))
            {
                throw new Exception("Break Type is not set");
            }
            if (isrSettings.DefaultEfficiencyThreshold < 0 || isrSettings.DefaultEfficiencyThreshold > 9999.9)
            {
                throw new Exception("Default Efficiency Threshold for sales area is invalid");
            }
            if (isrSettings.DemographicsSettings.FindAll(ds => ds.EfficiencyThreshold < 0 || ds.EfficiencyThreshold > 9999.99).Count > 0)
            {
                throw new Exception("Efficiency Threshold for demographic is invalid");
            }
        }
    }
}
