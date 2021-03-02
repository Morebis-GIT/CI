using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios.Queries;
using xggameplan.Model;

namespace xggameplan.Profile
{
    public class ScenarioProfile : AutoMapper.Profile
    {
        public ScenarioProfile()
        {
            CreateMap<CreateScenarioModel, Scenario>();
            CreateMap<Scenario, ScenarioModel>().ReverseMap();
            CreateMap<ScenarioModel, RunScenario>();
            CreateMap<CreateRunScenarioModel, RunScenario>();
            CreateMap<AutopilotScenarioEngageModel, AutopilotScenarioModel>();
            CreateMap<CampaignPriorityRoundsModel, CampaignPriorityRounds>().ReverseMap();
            CreateMap<PriorityRoundModel, PriorityRound>().ReverseMap();

            CreateMap<Tuple<CreateRunScenarioModel, List<CampaignWithProductFlatModel>>, Scenario>()
                .ConvertUsing((tuple, scenario, context) => MapToScenario(tuple.Item1, tuple.Item2, context.Mapper));

            // RunScenario, List<Scenario>, List<Pass>, Guid(DefaultScenarioID), List<int>(Library PassIDs)
            CreateMap<Tuple<RunScenario, List<Scenario>, List<Pass>, Guid>, ScenarioModel>()
                    .ForMember(s1 => s1.Id, exp => exp.MapFrom(rs => rs.Item1.Id))
                    .ForMember(s1 => s1.CompletedDateTime, exp => exp.MapFrom(rs => rs.Item1.CompletedDateTime))
                    .ForMember(s1 => s1.DateCreated, exp => exp.MapFrom(rs => rs.Item1.DateCreated))
                    .ForMember(s1 => s1.ExternalRunId, exp => exp.MapFrom((rs, _) => rs.Item1.ExternalRunInfo?.ExternalRunId))
                    .ForMember(s1 => s1.ExternalStatus, exp => exp.MapFrom((rs, _) => rs.Item1.ExternalRunInfo?.ExternalStatus))
                    .ForMember(s1 => s1.ExternalStatusModifiedDate, exp => exp.MapFrom((rs, _) => rs.Item1.ExternalRunInfo?.ExternalStatusModifiedDate))
                    .ForMember(s1 => s1.CompletedDateTime, exp => exp.MapFrom(rs => rs.Item1.DateModified))
                    .ForMember(s1 => s1.Name, exp => exp.MapFrom(rs => rs.Item2.Where(a => a.Id == rs.Item1.Id).Select(a => a.Name).FirstOrDefault()))
                    .ForMember(s1 => s1.IsDefault, exp => exp.MapFrom(rs => rs.Item4 == rs.Item1.Id))
                    .ForMember(s1 => s1.IsLibraried, exp => exp.MapFrom(rs => rs.Item2.Where(a => a.Id == rs.Item1.Id).Select(a => a.IsLibraried).FirstOrDefault()))
                    .ForMember(s1 => s1.IsAutopilot, exp => exp.MapFrom(rs => rs.Item2.Where(a => a.Id == rs.Item1.Id).Select(a => a.IsAutopilot).FirstOrDefault()))
                    .ForMember(s1 => s1.Progress, exp => exp.MapFrom(rs => rs.Item1.Progress))
                    .ForMember(s1 => s1.StartedDateTime, exp => exp.MapFrom(rs => rs.Item1.StartedDateTime))
                    .ForMember(s1 => s1.Status, exp => exp.MapFrom(rs => rs.Item1.Status))
                    .ForMember(s1 => s1.Passes, exp => exp.MapFrom((rs, model, passModels, context) => GetPassModels(rs.Item1, rs.Item2, rs.Item3, context.Mapper)))
                    .ForMember(s => s.CampaignPassPriorities, o => o.MapFrom((tuple, model, campaignPassPriorityModels, context) => MapToCampaignPassPriorityModels(tuple.Item1, tuple.Item2, context.Mapper)))
                    .ForMember(s1 => s1.CampaignPriorityRounds, exp => exp.MapFrom((rs, model, passModels, context) =>
                    MapToCampaignPriorityRoundsModel(rs.Item2.Where(a => a.Id == rs.Item1.Id).Select(a => a.CampaignPriorityRounds).FirstOrDefault(), context.Mapper)));

            // Scenario, List<Pass>, Guid(DefaultScenarioID), List<int>(Library PassIDs)

            CreateMap<Tuple<RunScenario, List<Scenario>>, ScenarioModel>()
                    .ForMember(s1 => s1.Id, exp => exp.MapFrom(rs => rs.Item1.Id))
                    .ForMember(s1 => s1.CompletedDateTime, exp => exp.MapFrom(rs => rs.Item1.CompletedDateTime))
                    .ForMember(s1 => s1.DateCreated, exp => exp.MapFrom(rs => rs.Item1.DateCreated))
                    .ForMember(s1 => s1.CompletedDateTime, exp => exp.MapFrom(rs => rs.Item1.DateModified))
                    .ForMember(s1 => s1.Name, exp => exp.MapFrom(rs => rs.Item2.Where(a => a.Id == rs.Item1.Id).Select(a => a.Name).FirstOrDefault()))
                    .ForMember(s1 => s1.StartedDateTime, exp => exp.MapFrom(rs => rs.Item1.StartedDateTime))
                    .ForMember(s1 => s1.Status, exp => exp.MapFrom(rs => rs.Item1.Status));

            // We don't set RunScenario properties as we don't have a current run
            CreateMap<Tuple<Scenario, List<Pass>, Guid>, ScenarioModel>()
                    .ForMember(s1 => s1.Id, exp => exp.MapFrom(rs => rs.Item1.Id))
                    .ForMember(s1 => s1.Name, exp => exp.MapFrom(rs => rs.Item1.Name))
                    .ForMember(s1 => s1.Passes, exp => exp.MapFrom((rs, model, passModels, context) => GetPassModels(rs.Item1, rs.Item2, context.Mapper)))
                    .ForMember(s1 => s1.DateCreated, exp => exp.MapFrom(rs => rs.Item1.DateCreated))
                    .ForMember(s1 => s1.DateModified, exp => exp.MapFrom(rs => rs.Item1.DateModified))
                    .ForMember(s1 => s1.DateUserModified, exp => exp.MapFrom(rs => rs.Item1.DateUserModified))
                    .ForMember(s1 => s1.IsDefault, exp => exp.MapFrom(rs => rs.Item3 == rs.Item1.Id))
                    .ForMember(s1 => s1.IsLibraried, exp => exp.MapFrom(rs => rs.Item1.IsLibraried))
                    .ForMember(s => s.CampaignPassPriorities, o => o.MapFrom((tuple, model, campaignPassPriorityModels, context) => MapToCampaignPassPriorityModels(tuple.Item1, context.Mapper)))
                    .ForMember(s1 => s1.CampaignPriorityRounds, exp => exp.MapFrom((rs, model, passModels, context) => MapToCampaignPriorityRoundsModel(rs.Item1.CampaignPriorityRounds, context.Mapper)));

            // Scenario, List<Pass>, Guid(DefaultScenarioID), List<int>(Library PassIDs)
            CreateMap<Tuple<Scenario, List<Pass>, Guid>, CreateRunScenarioModel>()
                    .ForMember(s1 => s1.Id, exp => exp.MapFrom(rs => rs.Item1.Id))
                    .ForMember(s1 => s1.Name, exp => exp.MapFrom(rs => rs.Item1.Name))
                    .ForMember(s1 => s1.Passes, exp => exp.MapFrom((rs, model, passModels, context) => GetPassModels(rs.Item1, rs.Item2, context.Mapper)))
                    .ForMember(s1 => s1.CampaignPriorityRounds, exp => exp.MapFrom((rs, model, passModels, context) => MapToCampaignPriorityRoundsModel(rs.Item1.CampaignPriorityRounds, context.Mapper)));

            CreateMap<Tuple<Scenario, List<Pass>>, CreateRunScenarioModel>()
                   .ForMember(s1 => s1.Id, exp => exp.MapFrom(rs => rs.Item1.Id))
                   .ForMember(s1 => s1.Name, exp => exp.MapFrom(rs => rs.Item1.Name))
                   .ForMember(s1 => s1.Passes, exp => exp.MapFrom(rs => rs.Item2))
                   .ForMember(s => s.CampaignPassPriorities,
                              o => o.MapFrom(rs => rs.Item1.CampaignPassPriorities ?? new List<CampaignPassPriority>()))
                   .ForMember(s1 => s1.CampaignPriorityRounds, exp => exp.MapFrom((rs, model, passModels, context) => MapToCampaignPriorityRoundsModel(rs.Item1.CampaignPriorityRounds, context.Mapper)));

            CreateMap<SearchResultModel<ScenarioDigestListItem>, SearchResultModel<ScenarioDigestListItemModel>>();
            CreateMap<ScenarioDigestListItem, ScenarioDigestListItemModel>();
        }

        private static Scenario MapToScenario(
            CreateRunScenarioModel createRunScenarioModel, List<CampaignWithProductFlatModel> campaigns, IMapper mapper)
        {
            if (createRunScenarioModel == null)
            {
                return null;
            }

            return new Scenario
            {
                Id = createRunScenarioModel.Id,
                Name = createRunScenarioModel.Name,
                Passes = createRunScenarioModel.Passes?.Select(p => new PassReference { Id = p.Id }).ToList(),
                CampaignPassPriorities = MapToCampaignPassPriorities(createRunScenarioModel, campaigns, mapper),
                IsAutopilot = createRunScenarioModel.IsAutopilot,
                CampaignPriorityRounds = mapper.Map<CampaignPriorityRounds>(createRunScenarioModel.CampaignPriorityRounds)
            };
        }

        private static List<CampaignPassPriority> MapToCampaignPassPriorities(
            CreateRunScenarioModel createRunScenarioModel, List<CampaignWithProductFlatModel> campaigns, IMapper mapper)
        {
            var campaignPassPriorities = new List<CampaignPassPriority>();

            if (createRunScenarioModel != null && createRunScenarioModel.CampaignPassPriorities != null &&
                createRunScenarioModel.CampaignPassPriorities.Any() && campaigns != null && campaigns.Any())
            {
                campaignPassPriorities.AddRange(createRunScenarioModel.CampaignPassPriorities.Select(a => new CampaignPassPriority
                {
                    Campaign = mapper.Map<CompactCampaign>(campaigns.FirstOrDefault(c => c.ExternalId == a.CampaignExternalId)),
                    PassPriorities = mapper.Map<List<PassPriority>>(a.PassPriorities)
                }));
            }

            return campaignPassPriorities;
        }

        private static CampaignPriorityRoundsModel MapToCampaignPriorityRoundsModel(CampaignPriorityRounds campaignPriorityRounds, IMapper mapper)
        {
            if (campaignPriorityRounds != null)
            {
                campaignPriorityRounds.Rounds = campaignPriorityRounds.Rounds?.OrderBy(r => r.Number).ToList();
            }

            return mapper.Map<CampaignPriorityRoundsModel>(campaignPriorityRounds);
        }

        private static List<CampaignPassPriorityModel> MapToCampaignPassPriorityModels(
            RunScenario runScenario, IList<Scenario> scenarios, IMapper mapper)
        {
            return MapToCampaignPassPriorityModels(scenarios.FirstOrDefault(s => s.Id == runScenario.Id), mapper);
        }

        private static List<CampaignPassPriorityModel> MapToCampaignPassPriorityModels(Scenario scenario, IMapper mapper)
        {
            var campaignPassPriorityModels = new List<CampaignPassPriorityModel>();
            if (scenario?.CampaignPassPriorities != null && scenario.CampaignPassPriorities.Any())
            {
                campaignPassPriorityModels.AddRange(scenario.CampaignPassPriorities.Select(a => new CampaignPassPriorityModel
                {
                    CampaignExternalId = a.Campaign != null && !string.IsNullOrWhiteSpace(a.Campaign.ExternalId) ? a.Campaign.ExternalId : string.Empty,
                    Campaign = mapper.Map<CampaignWithProductFlatModel>(a.Campaign),
                    PassPriorities = mapper.Map<List<PassPriorityModel>>(a.PassPriorities)
                }));
            }

            return campaignPassPriorityModels;
        }

        /// <summary>
        /// Returns PassModels for Scenario
        /// </summary>
        private static List<PassModel> GetPassModels(
            Scenario scenario, IList<Pass> passes, IMapper mapper)
        {
            var passModels = new List<PassModel>();

            foreach (var passReference in scenario.Passes)
            {
                var pass = passes.FirstOrDefault(p => p.Id == passReference.Id);

                if (pass != null)
                {
                    passModels.Add(mapper.Map<PassModel>(pass));
                }
            }

            return passModels;
        }

        /// <summary>
        /// Returns PassModels for RunScenario
        /// </summary>
        private static List<PassModel> GetPassModels(
            RunScenario runScenario, IList<Scenario> scenarios, IList<Pass> passes, IMapper mapper)
        {
            var passModels = new List<PassModel>();
            var scenario = scenarios.FirstOrDefault(s => s.Id == runScenario.Id);
            if (scenario?.Passes == null || !scenario.Passes.Any())
            {
                return passModels;
            }

            foreach (var passReference in scenario.Passes)
            {
                var pass = passes.FirstOrDefault(p => p.Id == passReference.Id);
                if (pass != null)
                {
                    passModels.Add(mapper.Map<PassModel>(pass));
                }
            }

            return passModels;
        }
    }
}
