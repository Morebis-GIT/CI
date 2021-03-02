using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using NodaTime;
using xggameplan.core.Extensions;
using xggameplan.Model;
using xggameplan.Model.AutoGen;
using AgSalesArea = xggameplan.model.AutoGen.AgSponsorships.AgSalesArea;

namespace xggameplan.Profile
{
    public class SalesAreaProfile : AutoMapper.Profile
    {
        public SalesAreaProfile()
        {
            CreateMap<Tuple<SalesArea, IReadOnlyCollection<Demographic>>, AgSalesAreaPtr>()
              .ForMember(ags => ags.ShortName, exp => exp.MapFrom(s => s.Item1.ShortName))
              .ForMember(ags => ags.CurrencyCode, exp => exp.MapFrom(s => s.Item1.CurrencyCode))
              .ForMember(ags => ags.SalesAreaNo, exp => exp.MapFrom(s => s.Item1.CustomId))
              .ForMember(ags => ags.RatingSalesAreaNo, exp => exp.MapFrom(s => s.Item1.CustomId))
              .ForMember(ags => ags.SalesAreaNoRequired, exp => exp.MapFrom(s => s.Item1.CustomId))
              .ForMember(ags => ags.BaseDemoNo,
                  exp => exp.MapFrom(
                      s => (s.Item2.FirstOrDefault(m => m.ExternalRef.Equals(s.Item1.BaseDemographic1)).Id)))
              .ForMember(ags => ags.BaseDemoNo2,
                  exp => exp.MapFrom(
                      s => (s.Item2.FirstOrDefault(m => m.ExternalRef.Equals(s.Item1.BaseDemographic2)).Id)))
              .ForMember(ags => ags.StartTime,
                  exp => exp.MapFrom(
                      s => AgConversions.ToAgTimeAsHHMMSS(s.Item1.StartOffset.ToTimeSpan())))
              .ForMember(ags => ags.NbrHours,
                  exp => exp.MapFrom(
                      s => Convert.ToInt32(s.Item1.DayDuration.BclCompatibleTicks / NodaConstants.TicksPerHour)));

            CreateMap<Tuple<string, List<SalesArea>>, AgSalesArea>()
                .ForMember(d => d.SalesArea, m => m.MapFrom(args => args.Item2.Any(s => s.Name == args.Item1) ? args.Item2.First(s => s.Name == args.Item1).CustomId : -1));

            CreateMap<object, AgSalesAreaRef>().ForMember(ags => ags.SalesAreaNo, exp => exp.MapFrom(i => Convert.ToInt32(i)));

            CreateMap<SalesArea, SalesAreaModel>().ForMember(salesAreaModel => salesAreaModel.Uid, expression => expression.MapFrom(salesArea => salesArea.Id));

            CreateMap<CreateSalesAreaModel, SalesArea>();
        }
    }
}
