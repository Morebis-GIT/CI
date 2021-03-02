using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;

namespace xggameplan.Model
{
    /// <summary>
    /// Schedule DTO model to expose in UI
    /// </summary>
    public class ScheduleModel
    {
        /// <summary>
        /// Unique Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Schedule date
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// scheduled programmes sales area name
        /// </summary>
        public string SalesArea { get; set; }

        /// <summary>
        /// List of programmes
        /// </summary>
        public List<Programme> Programmes { get; set; }

        /// <summary>
        /// List of break
        /// </summary>
        public List<Break> Breaks { get; set; }
    }



}
