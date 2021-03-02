using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas;
using xggameplan.core.Extensions.AutoMapper;
using SmoothConfigEntities = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    using BestBreakFactorEntities = SmoothConfigEntities.BestBreakFactorEntities;
    using SmoothPassEntities = SmoothConfigEntities.SmoothPassEntities;

    public class SmoothConfigurationProfile : Profile
    {
        public SmoothConfigurationProfile()
        {
            //From Domain
            _ = CreateMap<SmoothConfiguration, SmoothConfigEntities.SmoothConfiguration>().ReverseMap();
            _ = CreateMap<SmoothDiagnosticConfiguration, SmoothConfigEntities.SmoothDiagnosticConfiguration>();

            _ = CreateMap<SmoothPassDefault, SmoothPassEntities.SmoothPassDefault>();
            _ = CreateMap<SmoothPassBooked, SmoothPassEntities.SmoothPassBooked>().ReverseMap();
            _ = CreateMap<SmoothPassUnplaced, SmoothPassEntities.SmoothPassUnplaced>();
            _ = CreateMap<SmoothPassDefaultIteration, SmoothPassEntities.SmoothPassDefaultIteration>();
            _ = CreateMap<SmoothPassUnplacedIteration, SmoothPassEntities.SmoothPassUnplacedIteration>();

            _ = CreateMap<SmoothPass, SmoothPassEntities.SmoothPass>()
                .Include<SmoothPassDefault, SmoothPassEntities.SmoothPassDefault>()
                .Include<SmoothPassBooked, SmoothPassEntities.SmoothPassBooked>()
                .Include<SmoothPassUnplaced, SmoothPassEntities.SmoothPassUnplaced>();

            _ = CreateMap<SpotsCriteria, SmoothConfigEntities.SpotsCriteria>().ReverseMap();
            _ = CreateMap<SpotsCriteria, BestBreakFactorEntities.BestBreakFactorGroupSpotsCriteria>().ReverseMap();
            _ = CreateMap<SpotsCriteria, SmoothPassEntities.SmoothPassIterationSpotsCriteria>().ReverseMap();

            _ = CreateMap<BestBreakFactorGroupRecord, BestBreakFactorEntities.BestBreakFactorGroupRecord>()
                .ForMember(d => d.PassSequences,
                    opt => opt.MapFrom(s =>
                        s.PassSequences.Select(x => new BestBreakFactorEntities.BestBreakFactorGroupRecordPassSequenceItem { Value = x })));

            _ = CreateMap<BestBreakFactorGroup, BestBreakFactorEntities.BestBreakFactorGroup>();
            _ = CreateMap<BestBreakFactor, BestBreakFactorEntities.BestBreakFactor>().ReverseMap();
            _ = CreateMap<BestBreakFactorGroupItem, BestBreakFactorEntities.BestBreakFactorGroupItem>();
            _ = CreateMap<BestBreakFactor, BestBreakFactorEntities.BestBreakDefaultFactor>().ReverseMap();
            _ = CreateMap<BestBreakFactor, BestBreakFactorEntities.BestBreakFilterFactor>().ReverseMap();
            _ = CreateMap<BestBreakFactor, BestBreakFactorEntities.SameBreakGroupScoreFactor>().ReverseMap();

            _ = CreateMap<SmoothPassIterationRecord, SmoothPassEntities.SmoothPassIterationRecord>()
                .ForMember(d => d.PassSequences,
                    opt => opt.MapFrom(s =>
                        s.PassSequences.Select(x => new SmoothPassEntities.SmoothPassIterationRecordPassSequenceItem { Value = x })));

            //To Domain
            CreateMap<string, SmoothConfigEntities.SmoothDiagnosticConfigurationSalesArea>()
                .ForMember(dest => dest.SalesAreaId,
                    opts => opts.FromEntityCache(opt => opt.Entity<SalesArea>(x => x.Id)))
                .ReverseMap()
                .FromEntityCache(x => x.SalesAreaId, opt => opt.Entity<SalesArea>(x => x.Name));

            _ = CreateMap<SmoothConfigEntities.SmoothDiagnosticConfiguration, SmoothDiagnosticConfiguration>()
                .ForMember(d => d.SpotSalesAreas, opt => opt.Condition(s => s.SpotSalesAreas.Any()))
                .ForMember(d => d.SpotDemographics, opt => opt.Condition(s => s.SpotDemographics.Any()))
                .ForMember(d => d.SpotExternalRefs, opt => opt.Condition(s => s.SpotExternalRefs.Any()))
                .ForMember(d => d.SpotExternalCampaignRefs, opt => opt.Condition(s => s.SpotExternalCampaignRefs.Any()))
                .ForMember(d => d.SpotMultipartSpots, opt => opt.Condition(s => s.SpotMultipartSpots.Any()));

            _ = CreateMap<SmoothPassEntities.SmoothPass, SmoothPass>()
                    .Include<SmoothPassEntities.SmoothPassDefault, SmoothPassDefault>()
                    .Include<SmoothPassEntities.SmoothPassBooked, SmoothPassBooked>()
                    .Include<SmoothPassEntities.SmoothPassUnplaced, SmoothPassUnplaced>();

            _ = CreateMap<SmoothPassEntities.SmoothPassIterationRecord, SmoothPassIterationRecord>()
                    .ForMember(d => d.PassSequences, opt => opt.MapFrom(s => s.PassSequences.Select(x => x.Value)));

            CreateMap<SmoothPassEntities.SmoothPassUnplaced, SmoothPassUnplaced>()
                .ConvertUsing(s => s != null ? new SmoothPassUnplaced(s.Sequence) : null);

            CreateMap<SmoothPassEntities.SmoothPassDefault, SmoothPassDefault>()
                .ConvertUsing(s => s != null ? new SmoothPassDefault(s.Sequence, s.Sponsored, s.HasMultipartSpots,
                    s.CanSplitMultipartSpots, s.Preemptable, s.BreakRequests.ToList(), s.HasProductClashCode,
                    s.HasSpotEndTime) : null);

            CreateMap<SmoothPassEntities.SmoothPassDefaultIteration, SmoothPassDefaultIteration>()
            .ConvertUsing(s => s != null ? new SmoothPassDefaultIteration(s.Sequence, s.RespectSpotTime, s.RespectCampaignClash,
                (ProductClashRules)s.ProductClashRules, (SpotPositionRules)s.BreakPositionRules,
                (SpotPositionRules)s.RequestedPositionInBreakRules, s.RespectRestrictions,
                s.RespectClashExceptions) : null);
            CreateMap<SmoothPassEntities.SmoothPassUnplacedIteration, SmoothPassUnplacedIteration>()
                .ConvertUsing(s => s != null ? new SmoothPassUnplacedIteration(s.Sequence, s.RespectSpotTime,
                    s.RespectCampaignClash, (ProductClashRules)s.ProductClashRule, s.RespectRestrictions,
                    s.RespectClashExceptions) : null);

            _ = CreateMap<BestBreakFactorEntities.BestBreakFactorGroupRecord, BestBreakFactorGroupRecord>()
                .ForMember(d => d.PassSequences, opt => opt.MapFrom(s => s.PassSequences.Select(x => x.Value)));

            CreateMap<BestBreakFactorEntities.BestBreakFactorGroup, BestBreakFactorGroup>()
                .ConvertUsing((s, d, rc) => s != null ? new BestBreakFactorGroup(s.Sequence, s.Name,
                    (BestBreakFactorGroupEvaluation)s.Evaluation,
                    (SameBreakGroupScoreActions)s.SameBreakGroupScoreAction,
                    rc.Mapper.Map<BestBreakFactor>(s.SameBreakGroupScoreFactor),
                    rc.Mapper.Map<List<BestBreakFactorGroupItem>>(s.Items)) : null);

            CreateMap<BestBreakFactorEntities.BestBreakFactorGroupItem, BestBreakFactorGroupItem>()
                .ConvertUsing((s, d, rc) => s != null ? new BestBreakFactorGroupItem((BestBreakFactorItemEvaluation)s.Evaluation,
                    s.UseZeroScoresInEvaluation, s.AllFilterFactorsMustBeNonZero,
                    rc.Mapper.Map<List<BestBreakFactor>>(s.FilterFactors),
                    rc.Mapper.Map<List<BestBreakFactor>>(s.DefaultFactors)) : null);
        }
    }
}
