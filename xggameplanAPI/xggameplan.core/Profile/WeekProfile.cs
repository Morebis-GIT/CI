using System;
using System.Collections.Generic;
using System.Linq;
using xggameplan.Model.AutoGen;

namespace xggameplan.Profile
{
    internal class WeekProfile : AutoMapper.Profile
    {
        public WeekProfile()
        {
            CreateMap<Dictionary<DateTime, DateTime>, List<AgWeek>>().ConstructUsing(times => _loadAgWeek(times));
        }

        private readonly Func<Dictionary<DateTime, DateTime>, List<AgWeek>> _loadAgWeek = dateRange =>
        {
            return dateRange.Select(d => new AgWeek
            {
                StartDate = d.Key.ToString("yyyyMMdd"),
                EndDate = d.Value.ToString("yyyyMMdd")
            }).ToList();
        };
    }
}
