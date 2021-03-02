using System;
using System.Collections.Generic;
using System.Linq;

namespace ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions.Objects
{
    public class Restriction
    {
        /// <summary>
        /// Raven Unique Id for the restriction. Used for autogen maily.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Unique Id for the restriction.
        /// </summary>
        public Guid Uid { get; set; }

        /// <summary>
        /// Sales Area names for the restriction. If it's not assigned the
        /// restriction applies to all sales areas.
        /// </summary>
        public List<string> SalesAreas { get; set; }

        /// <summary>
        /// Start date of restriction
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// End date of restriction
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Start time of restriction
        /// </summary>
        public TimeSpan? StartTime { get; set; }

        /// <summary>
        /// End time of restriction
        /// </summary>
        public TimeSpan? EndTime { get; set; }

        /// <summary>
        /// ”1111111” Days of the week that the restriction applies to Monday to Sunday
        /// where 1 means applies and 0 means does not - this will always have 7 digits
        /// </summary>
        public string RestrictionDays { get; set; }

        /// <summary>
        /// Days of week that the restriction applies to
        /// </summary>
        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
        public IEnumerable<DayOfWeek> RestrictionDaysOfWeek
        {
            get
            {
                if (String.IsNullOrEmpty(RestrictionDays))
                {
                    return Enumerable.Empty<DayOfWeek>();
                }

                var daysOfWeek = new List<DayOfWeek>();
                var values = RestrictionDays.ToCharArray();

                for (int index = 0; index < values.Length; index++)
                {
                    if (values[index] == '1')
                    {
                        daysOfWeek.Add(_allDaysOfWeek[index]);
                    }
                }

                return daysOfWeek;
            }
        }

        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
        private static readonly DayOfWeek[] _allDaysOfWeek =
        {
            DayOfWeek.Monday,
            DayOfWeek.Tuesday,
            DayOfWeek.Wednesday,
            DayOfWeek.Thursday,
            DayOfWeek.Friday,
            DayOfWeek.Saturday,
            DayOfWeek.Sunday
        };

        /// <summary>
        /// Include / Exclude / Either – if not provided defaults to Either
        /// </summary>
        public IncludeOrExcludeOrEither SchoolHolidayIndicator { get; set; }

        /// <summary>
        /// Include / Exclude / Either – if not provided defaults to Either
        /// </summary>
        public IncludeOrExcludeOrEither PublicHolidayIndicator { get; set; }

        /// <summary>
        /// Include or exclude the restriction from live programmes.
        /// If not present will assume includes.
        /// </summary>
        public IncludeOrExclude LiveProgrammeIndicator { get; set; }

        /// <summary>
        /// Type of Restriction
        /// </summary>
        public RestrictionType RestrictionType { get; set; }

        /// <summary>
        /// Basis of restriction
        /// </summary>
        public RestrictionBasis RestrictionBasis { get; set; }

        /// <summary>
        /// Programme that has triggered the restriction.
        /// This may or may not be populated based on the restriction type.
        /// </summary>
        public string ExternalProgRef { get; set; }

        /// <summary>
        /// Programme category that has triggered the restriction.
        /// </summary>
        public string ProgrammeCategory { get; set; }

        /// <summary>
        /// Programme Classification
        /// </summary>
        public string ProgrammeClassification { get; set; }

        /// <summary>
        /// Programme Classification Indicator. If not provided defaults to include
        /// </summary>
        public IncludeOrExclude ProgrammeClassificationIndicator { get; set; }

        /// <summary>
        /// Time tolerance before programme restriction in minutes.
        /// Defaulted to zero mins if not included.
        /// </summary>
        public int TimeToleranceMinsBefore { get; set; }

        /// <summary>
        /// Time tolerance after programme restriction in minutes.
        /// Defaulted to zero mins if not included.
        /// </summary>
        public int TimeToleranceMinsAfter { get; set; }

        /// <summary>
        /// Index Type
        /// </summary>
        public int IndexType { get; set; }

        /// <summary>
        /// Index Threshold
        /// </summary>
        public int IndexThreshold { get; set; }

        /// <summary>
        /// Product Code
        /// </summary>
        public int ProductCode { get; set; }

        /// <summary>
        /// Clash code
        /// </summary>
        public string ClashCode { get; set; }

        /// <summary>
        /// Clearance Code
        /// </summary>
        public string ClearanceCode { get; set; }

        /// <summary>
        /// Clock Number / Copy Code / Industry Code
        /// </summary>
        public string ClockNumber { get; set; }

        /// <summary>
        /// Identifier used by external systems to identify and update restrictions
        /// </summary>
        public string ExternalIdentifier { get; set; }

        /// <summary>
        /// Episode Number
        /// </summary>
        public int EpisodeNumber { get; set; }
    }
}
