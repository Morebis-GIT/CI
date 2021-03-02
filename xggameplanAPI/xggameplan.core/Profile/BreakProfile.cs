using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using xggameplan.core.Extensions;
using xggameplan.Model;

namespace xggameplan.Profile
{
    internal class BreakProfile : AutoMapper.Profile
    {
        public BreakProfile()
        {
            CreateMap<Break, BreakModel>();
            CreateMap<CreateBreak, Break>();

            CreateMap<Tuple<BreakWithProgramme, IReadOnlyCollection<SalesArea>, IReadOnlyCollection<Demographic>, List<int>, int, AgBreak>, AgBreak>()
                .ConstructUsing((brk, context) =>
                    LoadAgBreak(brk, context.Mapper))
                .AfterMap((_, dest) =>
                {
                    if (dest.AgRegionalBreaks is null)
                    {
                        dest.NbrBkrgs = 0;
                        dest.NbrZeroBkrgs = 0;
                    }
                    else
                    {
                        var count = dest.AgRegionalBreaks.Count;
                        dest.NbrBkrgs = count;
                        dest.NbrZeroBkrgs = count;
                    }

                    if (dest.AgProgCategories is null)
                    {
                        dest.NbrPrgcs = 0;
                        dest.MaxPrgcs = 0;
                    }
                    else
                    {
                        int count = dest.AgProgCategories.Count;
                        dest.NbrPrgcs = count;
                        dest.MaxPrgcs = count;
                    }

                    dest.NbrPreds = dest.AgPredictions?.Count ?? 0;
                    dest.NbrAvals = dest.AgAvals?.Count ?? 0;
                });
        }

        private static AgBreak LoadAgBreak(
            Tuple<BreakWithProgramme, IReadOnlyCollection<SalesArea>, IReadOnlyCollection<Demographic>, List<int>, int, AgBreak> source,
            IMapper mapper)
        {
            var (breakModel, salesAreas, demographics, programmeCategories, uniqueId, agBreak) = source;

            var brk = breakModel.Break;

            var salesArea = salesAreas?.FirstOrDefault(s => s.Name == brk.SalesArea);
            var salesAreaId = salesArea?.CustomId ?? 0;

            var agProgCategories = new AgProgCategories();

            if (programmeCategories != null)
            {
                agProgCategories.AddRange(programmeCategories);
            }

            var agBreakClone = agBreak.Clone();

            agBreakClone.SalesAreaNo = salesAreaId;
            agBreakClone.SalesAreaId = salesAreaId;

            DecomposeScheduledDate(brk.ScheduledDate);

            void DecomposeScheduledDate(DateTime scheduledDate)
            {
                agBreakClone.ScheduledDate = scheduledDate.ToString("yyyyMMdd");
                agBreakClone.NominalTime = scheduledDate.ToString("HHmmss");
                agBreakClone.DayNumber = scheduledDate.DayOfWeek == 0 ? 7 : (int)scheduledDate.DayOfWeek;
            }

            agBreakClone.BroadcastDate = brk.BroadcastDate.HasValue ? brk.BroadcastDate.Value.ToString("yyyyMMdd") : "0";
            agBreakClone.ClockHour = brk.ClockHour ?? 0;

            agBreakClone.BreakNo = brk.CustomId;
            agBreakClone.ExternalNo = brk.ExternalBreakRef;
            agBreakClone.Uid = uniqueId;

            agBreakClone.Duration = (int)brk.Duration.ToTimeSpan().TotalSeconds;
            agBreakClone.BreakPrice = brk.BreakPrice;
            agBreakClone.FloorRate = brk.FloorRate;
            agBreakClone.PositionInProg = breakModel.PositionInProgram.ToString();
            agBreakClone.ProgNo = breakModel.ProgrammeNo;
            agBreakClone.ProgEventNo = breakModel.PrgtNo;
            agBreakClone.EpisNo = breakModel.EpisodeNumber.GetValueOrDefault();
            agBreakClone.BreakTypeCode = brk.BreakType.Substring(0, 2).ToUpperInvariant();
            agBreakClone.AgSalesAreaPtrRef = agBreak.AgSalesAreaPtrRef.Clone();
            agBreakClone.AgSalesAreaPtrRef.SalesAreaNo = salesAreaId;

            var availInSec = (int)brk.OptimizerAvail.ToTimeSpan().TotalSeconds;

            agBreakClone.AgRegionalBreaks = new AgRegionalBreaks
            {
                new AgRegionalBreak
                {
                    OpenAvail = availInSec,
                    TregNo = salesAreaId
                }
            };

            agBreakClone.AgAvals = new AgAvals
            {
                new AgAval
                {
                    OpenAvail = availInSec,
                    SalesAreaNo = salesAreaId,
                    ReserveDuration = (int)brk.ReserveDuration.ToTimeSpan().TotalSeconds
                }
            };

            agBreakClone.AgProgCategories = agProgCategories;
            agBreakClone.AgPredictions = mapper.Map<AgPredictions>(
                Tuple.Create(breakModel.Predictions, salesArea, demographics)
                );

            agBreakClone.Solus = AgConversions.ToAgBooleanAsString(brk.Solus);
            agBreakClone.PremiumCategory = brk.PremiumCategory ?? string.Empty;
            agBreakClone.AllowSplit = AgConversions.ToAgBooleanAsString(brk.AllowSplit);
            agBreakClone.NationalRegionalSplit = AgConversions.ToAgBooleanAsString(brk.NationalRegionalSplit);
            agBreakClone.ExcludePackages = AgConversions.ToAgBooleanAsString(brk.ExcludePackages);
            agBreakClone.BonusAllowed = AgConversions.ToAgBooleanAsString(brk.BonusAllowed);
            agBreakClone.LongForm = AgConversions.ToAgBooleanAsString(brk.LongForm);

            return agBreakClone;
        }
    }
}
