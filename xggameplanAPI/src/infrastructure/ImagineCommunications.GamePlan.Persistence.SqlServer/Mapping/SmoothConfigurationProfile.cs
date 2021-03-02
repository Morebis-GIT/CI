using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects;
using SmoothConfigurationEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration.SmoothConfiguration;
using SmoothDiagnosticConfigurationEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration.SmoothDiagnosticConfiguration;
using SmoothPassEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration.SmoothPassEntities.SmoothPass;
using SmoothPassDefaultEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration.SmoothPassEntities.SmoothPassDefault;
using SmoothPassBookedEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration.SmoothPassEntities.SmoothPassBooked;
using SmoothPassUnplacedEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration.SmoothPassEntities.SmoothPassUnplaced;
using SmoothPassIterationRecordEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration.SmoothPassEntities.SmoothPassIterationRecord;
using SpotsCriteriaEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration.SpotsCriteria;
using SmoothPassIterationRecordPassSequenceItem = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration.SmoothPassEntities.SmoothPassIterationRecordPassSequenceItem;
using SmoothPassDefaultIterationEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration.SmoothPassEntities.SmoothPassDefaultIteration;
using SmoothPassUnplacedIterationEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration.SmoothPassEntities.SmoothPassUnplacedIteration;
using BestBreakFactorGroupRecordEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration.BestBreakFactorEntities.BestBreakFactorGroupRecord;
using BestBreakFactorGroupRecordPassSequenceItem = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration.BestBreakFactorEntities.BestBreakFactorGroupRecordPassSequenceItem;
using BestBreakFactorGroupEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration.BestBreakFactorEntities.BestBreakFactorGroup;
using BestBreakFactorEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration.BestBreakFactorEntities.BestBreakFactor;
using BestBreakFactorGroupItemEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration.BestBreakFactorEntities.BestBreakFactorGroupItem;
using BestBreakDefaultFactorEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration.BestBreakFactorEntities.BestBreakDefaultFactor;
using BestBreakFilterFactorEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration.BestBreakFactorEntities.BestBreakFilterFactor;
using SameBreakGroupScoreFactorEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration.BestBreakFactorEntities.SameBreakGroupScoreFactor;
using SmoothPassIterationSpotsCriteriaEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration.SmoothPassEntities.SmoothPassIterationSpotsCriteria;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class SmoothConfigurationProfile : Profile
    {
        public SmoothConfigurationProfile()
        {
            //From Domain
            CreateMap<SmoothConfiguration, SmoothConfigurationEntity>().ReverseMap();

            CreateMap<SmoothDiagnosticConfiguration, SmoothDiagnosticConfigurationEntity>();

            CreateMap<SmoothPass, SmoothPassEntity>()
                .Include<SmoothPassDefault, SmoothPassDefaultEntity>()
                .Include<SmoothPassBooked, SmoothPassBookedEntity>()
                .Include<SmoothPassUnplaced, SmoothPassUnplacedEntity>();

            CreateMap<SmoothPassDefault, SmoothPassDefaultEntity>();
            CreateMap<SmoothPassBooked, SmoothPassBookedEntity>().ReverseMap();
            CreateMap<SmoothPassUnplaced, SmoothPassUnplacedEntity>();

            CreateMap<SmoothPassIterationRecord, SmoothPassIterationRecordEntity>()
                .ForMember(d => d.PassSequences,
                    opt => opt.MapFrom(s =>
                        s.PassSequences.Select(x => new SmoothPassIterationRecordPassSequenceItem {Value = x})));
            CreateMap<SpotsCriteria, SpotsCriteriaEntity>().ReverseMap();
            
            CreateMap<SpotsCriteria, SmoothPassIterationSpotsCriteriaEntity>().ReverseMap();
            CreateMap<SmoothPassDefaultIteration, SmoothPassDefaultIterationEntity>();
            CreateMap<SmoothPassUnplacedIteration, SmoothPassUnplacedIterationEntity>();

            CreateMap<BestBreakFactorGroupRecord, BestBreakFactorGroupRecordEntity>()
                .ForMember(d => d.PassSequences,
                    opt => opt.MapFrom(s =>
                        s.PassSequences.Select(x => new BestBreakFactorGroupRecordPassSequenceItem {Value = x})));

            CreateMap<BestBreakFactorGroup, BestBreakFactorGroupEntity>();
            CreateMap<BestBreakFactor, BestBreakFactorEntity>().ReverseMap();
            CreateMap<BestBreakFactorGroupItem, BestBreakFactorGroupItemEntity>();
            CreateMap<BestBreakFactor, BestBreakDefaultFactorEntity>().ReverseMap();
            CreateMap<BestBreakFactor, BestBreakFilterFactorEntity>().ReverseMap();
            CreateMap<BestBreakFactor, SameBreakGroupScoreFactorEntity>().ReverseMap();

            //To Domain
            CreateMap<SmoothDiagnosticConfigurationEntity, SmoothDiagnosticConfiguration>()
                .ForMember(d => d.SpotSalesAreas, opt => opt.Condition(s => s.SpotSalesAreas.Any()))
                .ForMember(d => d.SpotDemographics, opt => opt.Condition(s => s.SpotDemographics.Any()))
                .ForMember(d => d.SpotExternalRefs, opt => opt.Condition(s => s.SpotExternalRefs.Any()))
                .ForMember(d => d.SpotExternalCampaignRefs, opt => opt.Condition(s => s.SpotExternalCampaignRefs.Any()))
                .ForMember(d => d.SpotMultipartSpots, opt => opt.Condition(s => s.SpotMultipartSpots.Any()));

            CreateMap<SmoothPassIterationRecordEntity, SmoothPassIterationRecord>()
                .ForMember(d => d.PassSequences, opt => opt.MapFrom(s => s.PassSequences.Select(x => x.Value)));
            CreateMap<BestBreakFactorGroupRecordEntity, BestBreakFactorGroupRecord>()
                .ForMember(d => d.PassSequences, opt => opt.MapFrom(s => s.PassSequences.Select(x => x.Value)));

            CreateMap<SmoothPassEntity, SmoothPass>()
                .Include<SmoothPassDefaultEntity, SmoothPassDefault>()
                .Include<SmoothPassBookedEntity, SmoothPassBooked>()
                .Include<SmoothPassUnplacedEntity, SmoothPassUnplaced>();

            CreateMap<SmoothPassUnplacedEntity, SmoothPassUnplaced>()
                .ConvertUsing(s => s != null ? new SmoothPassUnplaced(s.Sequence) : null);
            CreateMap<SmoothPassDefaultEntity, SmoothPassDefault>()
                .ConvertUsing(s => s != null ? new SmoothPassDefault(s.Sequence, s.Sponsored, s.HasMultipartSpots,
                    s.CanSplitMultipartSpots, s.Preemptable, s.BreakRequests.ToList(), s.HasProductClashCode,
                    s.HasSpotEndTime) : null);
            CreateMap<SmoothPassDefaultIterationEntity, SmoothPassDefaultIteration>()
                .ConvertUsing(s => s != null ? new SmoothPassDefaultIteration(s.Sequence, s.RespectSpotTime, s.RespectCampaignClash,
                    (ProductClashRules) s.ProductClashRules, (SpotPositionRules) s.BreakPositionRules,
                    (SpotPositionRules) s.RequestedPositionInBreakRules, s.RespectRestrictions,
                    s.RespectClashExceptions) : null);
            CreateMap<SmoothPassUnplacedIterationEntity, SmoothPassUnplacedIteration>()
                .ConvertUsing(s => s != null ? new SmoothPassUnplacedIteration(s.Sequence, s.RespectSpotTime,
                    s.RespectCampaignClash, (ProductClashRules) s.ProductClashRule, s.RespectRestrictions,
                    s.RespectClashExceptions) : null);
            CreateMap<BestBreakFactorGroupEntity, BestBreakFactorGroup>()
                .ConvertUsing((s, d, rc) => s != null ? new BestBreakFactorGroup(s.Sequence, s.Name,
                    (BestBreakFactorGroupEvaluation) s.Evaluation,
                    (SameBreakGroupScoreActions) s.SameBreakGroupScoreAction,
                    rc.Mapper.Map<BestBreakFactor>(s.SameBreakGroupScoreFactor),
                    rc.Mapper.Map<List<BestBreakFactorGroupItem>>(s.Items)) : null);
            CreateMap<BestBreakFactorGroupItemEntity, BestBreakFactorGroupItem>()
                .ConvertUsing((s, d, rc) => s != null ? new BestBreakFactorGroupItem((BestBreakFactorItemEvaluation) s.Evaluation,
                    s.UseZeroScoresInEvaluation, s.AllFilterFactorsMustBeNonZero,
                    rc.Mapper.Map<List<BestBreakFactor>>(s.FilterFactors),
                    rc.Mapper.Map<List<BestBreakFactor>>(s.DefaultFactors)) : null);
        }
    }
}
