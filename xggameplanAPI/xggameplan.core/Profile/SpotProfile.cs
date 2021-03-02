using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.Spots;
using xggameplan.Model;

namespace xggameplan.Profile
{
    internal class SpotProfile : AutoMapper.Profile
    {
        public SpotProfile()
        {
            CreateMap<CreateSpot, Spot>();
            CreateMap<Spot, SpotModel>().ForMember(sm => sm.ProductCode,
                    expression => expression.MapFrom(p => p.Product))
                .ForMember(sm => sm.Product,
                    expression => expression.Ignore());

            CreateMap<Tuple<Spot, Campaign, Product, List<Clash>>, SpotModel>()
                .ConstructUsing((s, context) => MapSpotModel(s, context.Mapper));

            CreateMap<Tuple<List<SpotModel>, List<Tuple<Break, Programme>>>, List<SpotWithBreakAndProgrammeInfo>>()
                .ConstructUsing(s => MapSpotWithBreakAndProgramme(s.Item1, s.Item2));
        }

        private static SpotModel MapSpotModel(Tuple<Spot, Campaign, Product, List<Clash>> src, IMapper mapper)
        {
            var (spot, campaigns, products, clashes) = src;

            var spotModel = mapper.Map<SpotModel>(spot);
            spotModel.Campaign = campaigns;
            spotModel.Product = products;
            spotModel.Clashes = clashes;
            return spotModel;
        }

        public static List<SpotWithBreakAndProgrammeInfo> MapSpotWithBreakAndProgramme(List<SpotModel> spots,
            List<Tuple<Break, Programme>> breakAndProgrammes)
        {
            spots = spots?.Where(s => !string.IsNullOrWhiteSpace(s.ExternalBreakNo)).ToList();
            var index = (from brkAndPgm in breakAndProgrammes
                         let brkSpots = !string.IsNullOrWhiteSpace(brkAndPgm.Item1?.ExternalBreakRef) && spots != null &&
                                        spots.Any()
                             ? spots.Where(s => s.ExternalBreakNo.Equals(brkAndPgm.Item1.ExternalBreakRef,
                                 StringComparison.OrdinalIgnoreCase)).ToList()
                             : new List<SpotModel>()
                         from spot in brkSpots.DefaultIfEmpty()
                         let breakEfficiency = !string.IsNullOrWhiteSpace(spot?.Demographic)
                             ? brkAndPgm?.Item1?.BreakEfficiencyList?.FirstOrDefault(
                                 _ => _.Demographic.Equals(spot.Demographic, StringComparison.OrdinalIgnoreCase))
                             : null
                         select new SpotWithBreakAndProgrammeInfo
                         {
                             BreakDuration = brkAndPgm?.Item1?.Duration,
                             BreakStartTime = brkAndPgm?.Item1?.ScheduledDate,
                             ProgrammeName = brkAndPgm?.Item2?.ProgrammeName,
                             ProgrammeDuration = brkAndPgm?.Item2?.Duration,
                             ProgrammeStartTime = brkAndPgm?.Item2?.StartDateTime,
                             Efficiency = breakEfficiency?.Efficiency,
                             ExternalBreakNo = spot?.ExternalBreakNo,
                             ExternalCampaignNumber = spot?.ExternalCampaignNumber,
                             CampaignGroup = spot?.Campaign?.CampaignGroup,
                             CampaignName = spot?.Campaign?.Name,
                             SpotLength = spot?.SpotLength,
                             ProductName = spot?.Product?.Name,
                             Demographic = spot?.Demographic,
                             AdvertiserName = spot?.Product?.AdvertiserName,
                             ClashCode = spot?.Product?.ClashCode,
                             ClashDescription = spot?.Clashes?.FirstOrDefault()?.Description
                         }).ToList();

            return index;
        }
    }
}
