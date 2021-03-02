using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Runs;
using ImagineCommunications.GamePlan.Domain.Shared.System.Settings;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using NodaTime;
using xggameplan.common.Extensions;
using EmailNotificationSettings = ImagineCommunications.GamePlan.Domain.Shared.System.Settings.EmailNotificationSettings;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class SharedTypesProfile : Profile
    {
        private const string DefaultTimeSpanFormat = "g";
        private static readonly List<string> _dowPattern = new List<string> {"Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"};

        public SharedTypesProfile()
        {
            CreateMap<TimeSpan, Duration>().ConvertUsing(s => Duration.FromTicks(s.Ticks));
            CreateMap<Duration, TimeSpan>().ConvertUsing(s => TimeSpan.FromTicks(s.BclCompatibleTicks));

            CreateMap<TimeSpan, string>()
                .ConstructUsing(src =>
                    new string(src.ToString(DefaultTimeSpanFormat, CultureInfo.InvariantCulture).ToCharArray()))
                .ConvertUsing(src => src.ToString(DefaultTimeSpanFormat, CultureInfo.InvariantCulture));
            CreateMap<string, TimeSpan>()
                .ConvertUsing(src => TimeSpan.ParseExact(src, DefaultTimeSpanFormat, CultureInfo.InvariantCulture));

            CreateMap<SortedSet<DayOfWeek>, string>().ConvertUsing(src => src.GetStringDowPattern(DayOfWeek.Monday));
            CreateMap<string, SortedSet<DayOfWeek>>().ConvertUsing(src => src.GetSortedSetDowPattern(DayOfWeek.Monday));

            CreateMap<DayOfWeek, string>()
                .ConstructUsing(src => new string(_dowPattern[(int) src].ToCharArray()))
                .ConvertUsing(src => _dowPattern[(int) src]);
            CreateMap<string, DayOfWeek>().ConvertUsing(src =>
                (DayOfWeek) _dowPattern.FindIndex(day => day.Equals(src, StringComparison.InvariantCultureIgnoreCase)));

            CreateMap<int, DayOfWeek>().ConvertUsing(src => (DayOfWeek)src);
            CreateMap<int, RunEvents>().ConvertUsing(src => (RunEvents)src);
            CreateMap<RunEvents, int>().ConvertUsing(src => (int)src);
            CreateMap<int, WebhookEvents>().ConvertUsing(src => (WebhookEvents)src);
            CreateMap<WebhookEvents, int>().ConvertUsing(src => (int)src);

            CreateMap<CategoryOrProgramme, string>().ConvertUsing(src => src.GetDescription());
            CreateMap<string, CategoryOrProgramme>().ConvertUsing(src => Enum.GetValues(typeof(CategoryOrProgramme))
                .Cast<CategoryOrProgramme>()
                .Single(x => x.GetDescription() == src));

            CreateMap<IncludeOrExclude, string>().ConvertUsing(src => src.GetDescription());
            CreateMap<string, IncludeOrExclude>().ConvertUsing(src => Enum.GetValues(typeof(IncludeOrExclude))
                .Cast<IncludeOrExclude>()
                .Single(x => x.GetDescription() == src));
            CreateMap<IncludeOrExclude, Domain.BusinessRules.Restrictions.IncludeOrExclude>()
                .ConvertUsing(src => (Domain.BusinessRules.Restrictions.IncludeOrExclude) src);

            CreateMap<CampaignStatus, string>().ConvertUsing(src => src.GetDescription());
            CreateMap<string, CampaignStatus>().ConvertUsing(src => Enum.GetValues(typeof(CampaignStatus))
                .Cast<CampaignStatus>()
                .Single(x => x.GetDescription() == src));

            CreateMap<IncludeOrExcludeOrEither, string>().ConvertUsing(src => src.GetDescription());
            CreateMap<string, IncludeOrExcludeOrEither>().ConvertUsing(src => Enum.GetValues(typeof(IncludeOrExcludeOrEither))
                .Cast<IncludeOrExcludeOrEither>()
                .Single(x => x.GetDescription() == src));
            CreateMap<IncludeOrExcludeOrEither, Domain.BusinessRules.Restrictions.IncludeOrExcludeOrEither>()
                .ConvertUsing(src => (Domain.BusinessRules.Restrictions.IncludeOrExcludeOrEither) src);

            CreateMap<RestrictionType, Domain.BusinessRules.Restrictions.RestrictionType>()
                .ConvertUsing(src => (Domain.BusinessRules.Restrictions.RestrictionType) src);

            CreateMap<RestrictionBasis, Domain.BusinessRules.Restrictions.RestrictionBasis>()
                .ConvertUsing(src => (Domain.BusinessRules.Restrictions.RestrictionBasis) src);

            CreateMap<SalesAreaPriorityType, Domain.Shared.System.Models.SalesAreaPriorityType>()
                .ConvertUsing(src => (Domain.Shared.System.Models.SalesAreaPriorityType) src);

            _ = CreateMap<EmailNotificationSettings, Entities.Tenant.EmailNotificationSettings>().ReverseMap();
        }
    }
}
