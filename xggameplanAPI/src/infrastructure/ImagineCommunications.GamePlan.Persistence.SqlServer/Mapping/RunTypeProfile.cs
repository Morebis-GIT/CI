using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.RunTypes.Objects;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class RunTypeProfile : Profile
    {
        public RunTypeProfile()
        {
            CreateMap<Entities.Tenant.Runs.RunTypeAnalysisGroup, RunTypeAnalysisGroup>()
                .ForMember(dest => dest.AnalysisGroupId, opt => opt.MapFrom(src => src.AnalysisGroupId))
                .ReverseMap();

            CreateMap<RunType, Entities.Tenant.RunType>()
                .ConstructUsing((rt, context) =>
                {
                    var entity = new Entities.Tenant.RunType();

                    if (!(rt.RunTypeAnalysisGroups is null) && rt.RunTypeAnalysisGroups.Any())
                    {
                        foreach (var analysisGroup in rt.RunTypeAnalysisGroups)
                        {
                            entity.RunTypeAnalysisGroups.Add(new Entities.Tenant.Runs.RunTypeAnalysisGroup
                            {
                                AnalysisGroupId = analysisGroup.AnalysisGroupId,
                                KPI = analysisGroup.KPI,
                                RunType = entity
                            });
                        }
                    }

                    return entity;
                });

            CreateMap<Entities.Tenant.RunType, RunType>()
                .ConstructUsing((rt, context) =>
                {
                    var runType = new RunType();

                    if (!(rt.RunTypeAnalysisGroups is null) && rt.RunTypeAnalysisGroups.Any())
                    {
                        foreach (var runTypeAnalysisGroup in rt.RunTypeAnalysisGroups)
                        {
                            runType.RunTypeAnalysisGroups.Add(new RunTypeAnalysisGroup
                            {
                                AnalysisGroupId = runTypeAnalysisGroup.AnalysisGroupId,
                                KPI = runTypeAnalysisGroup.KPI,
                                AnalysisGroupName = runTypeAnalysisGroup.AnalysisGroup.Name
                            });
                        }
                    }

                    return runType;
                });
        }
    }
}
