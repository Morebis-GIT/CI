using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions;
using ImagineCommunications.GamePlan.Domain.Generic.Types;

namespace ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions.Objects
{
    public class ClashExceptionModel
    {
        /// <summary>
        /// Raven Unique Id for the ClashException.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Start date of ClashException
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// End date of ClashException
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// From Type of ClashException
        /// </summary>
        public ClashExceptionType FromType { get; set; }

        /// <summary>
        /// To Type of ClashException
        /// </summary>
        public ClashExceptionType ToType { get; set; }

        /// <summary>
        /// Include or Exclude
        /// </summary>
        public IncludeOrExclude IncludeOrExclude { get; set; }

        /// <summary>
        /// From Value of ClashException
        /// </summary>
        public string FromValue { get; set; }

        /// <summary>
        /// From Value Description of ClashException
        /// </summary>
        public string FromValueDescription { get; set; }

        /// <summary>
        /// To Value of ClashException
        /// </summary>
        public string ToValue { get; set; }

        /// <summary>
        /// To Value Description of ClashException
        /// </summary>
        public string ToValueDescription { get; set; }

        /// <summary>
        /// To Value of ClashException
        /// </summary>
        public List<TimeAndDow> TimeAndDows { get; set; }

        public string ExternalRef { get; set; }
    }
}
