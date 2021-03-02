using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using xggameplan.core.Validators.ProductAdvertiser;

namespace xggameplan.model.External
{
    public class CreateClashException : IAdvertiserPeriodInfoContainer
    {
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
        /// To Value of ClashException
        /// </summary>
        public string ToValue { get; set; }

        /// <summary>
        /// To Value of ClashException
        /// </summary>
        public List<TimeAndDow> TimeAndDows { get; set; }

        public IEnumerable<IAdvertiserPeriodInfo> GetAdvertiserPeriodInfo()
        {
            if (FromType == ClashExceptionType.Advertiser || ToType == ClashExceptionType.Advertiser)
            {
                var data = new List<IAdvertiserPeriodInfo>();
                if (FromType == ClashExceptionType.Advertiser)
                {
                    data.Add(new AdvertiserPeriodInfo
                    {
                        Period = new DateTimeRange(StartDate, AdjustDate(EndDate) ?? DateTime.MaxValue),
                        ExternalIdentifier = FromValue
                    });
                }
                if (ToType == ClashExceptionType.Advertiser)
                {
                    data.Add(new AdvertiserPeriodInfo
                    {
                        Period = new DateTimeRange(StartDate, AdjustDate(EndDate) ?? DateTime.MaxValue),
                        ExternalIdentifier = ToValue
                    });
                }

                return data;
            }

            return Enumerable.Empty<IAdvertiserPeriodInfo>();

            DateTime? AdjustDate(DateTime? date)
            {
                if (date.HasValue)
                {
                    return date.Value == date.Value.Date ? date.Value.AddDays(1) : date.Value;
                }

                return null;
            }
        }

        private class AdvertiserPeriodInfo : IAdvertiserPeriodInfo
        {
            public DateTimeRange Period { get; set; }
            public string ExternalIdentifier { get; set; }
        }
    }

    public class UpdateClashException
    {
        /// <summary>
        /// End date of ClashException
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Include or Exclude
        /// </summary>
        public IncludeOrExclude IncludeOrExclude { get; set; }

        /// <summary>
        /// To Value of ClashException
        /// </summary>
        public List<TimeAndDow> TimeAndDows { get; set; }
    }
}
