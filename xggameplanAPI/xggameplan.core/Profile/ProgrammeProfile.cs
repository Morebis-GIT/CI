using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.ProgrammeDictionaries;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using NodaTime;
using xggameplan.Extensions;
using xggameplan.Model;
using xggameplan.Model.AutoGen;

namespace xggameplan.Profile
{
    internal class ProgrammeProfile : AutoMapper.Profile
    {
        public ProgrammeProfile()
        {
            _ = CreateMap<Programme, ProgrammeModel>();
            _ = CreateMap<CreateProgramme, Programme>();
            CreateMap<Tuple<List<Programme>, List<SalesArea>, IReadOnlyCollection<ProgrammeDictionary>, DayOfWeek>, List<AgProgramme>>()
                .ConvertUsing(p => LoadAgProgram(p.Item1, p.Item2, p.Item3, p.Item4));
        }

        private static List<AgProgramme> LoadAgProgram(
            IEnumerable<Programme> programs,
            IReadOnlyCollection<SalesArea> salesAreas,
            IReadOnlyCollection<ProgrammeDictionary> programmeDictionaries,
            DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            return (from pgm in programs
                    group pgm by new { WeekNo = pgm.StartDateTime.GetWeekNumber(startOfWeek), pgm.SalesArea, pgm.ExternalReference }
                into grp
                    let week = grp.Select(d => d.StartDateTime).FirstOrDefault().StartAndEndOfWeekDate(startOfWeek)
                    select new AgProgramme()
                    {
                        SalesAreaNo = salesAreas.FirstOrDefault(s => s.Name.Equals(grp.Key.SalesArea))?.CustomId ?? 0,
                        WeekNo = grp.Key.WeekNo,
                        WeekStartDate = week.Item1.ToString("yyyyMMdd"),
                        WeekEndDate = week.Item2.ToString("yyyyMMdd"),
                        ProgrammeNo = grp.Key.ExternalReference != null
                            ? programmeDictionaries
                                  .FirstOrDefault(p => p.ExternalReference.Equals(grp.Key.ExternalReference,
                                      StringComparison.OrdinalIgnoreCase))?.Id ?? 0
                            : 0,
                        ShortName = grp.Select(s => s.ExternalReference).FirstOrDefault(),
                        TotalMinutes = Convert.ToInt32(grp.Sum(s => s.Duration.BclCompatibleTicks) /
                                                       NodaConstants.TicksPerMinute)
                    }).ToList();
        }
    }
}
