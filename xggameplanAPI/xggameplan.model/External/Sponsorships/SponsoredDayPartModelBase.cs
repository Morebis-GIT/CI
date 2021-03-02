using System;
using System.ComponentModel.DataAnnotations;

namespace xggameplan.Model
{
    public class SponsoredDayPartModelBase
    {
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan? EndTime { get; set; }

        public string[] DaysOfWeek { get; set; }
    }
}
