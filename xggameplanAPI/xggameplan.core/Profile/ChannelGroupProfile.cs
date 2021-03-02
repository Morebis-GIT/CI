using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using xggameplan.Model.AutoGen;

namespace xggameplan.Profile
{
    internal class ChannelGroupProfile : AutoMapper.Profile
    {
        public ChannelGroupProfile()
        {
            CreateMap<Tuple<IReadOnlyCollection<Tuple<int, int, SalesAreaGroup>>, List<SalesArea>>, List<AgChannelGroup>>()
                .ConstructUsing(t => LoadAgChannelGroup(t.Item1, t.Item2));
        }

        private static AgChannelGroupSalesAreas LoadAgChannelGroupSalesAreas(SalesAreaGroup salesAreaGroup,
            IReadOnlyCollection<SalesArea> salesAreas, out int salesAreaCount)
        {
            salesAreaCount = 0;
            var agChannelGroupSalesAreas = new AgChannelGroupSalesAreas();
            if (salesAreaGroup?.SalesAreas == null || salesAreaGroup.SalesAreas.Count <= 0)
            {
                return agChannelGroupSalesAreas;
            }

            var agChannelGroupSalesAreaList = salesAreaGroup.SalesAreas.Select(salesArea =>
            {
                return new AgChannelGroupSalesArea
                {
                    SalesAreaNo =
                        salesAreas?.FirstOrDefault(s => s.Name == salesArea)?.CustomId ?? 0
                };
            }).ToList();
            agChannelGroupSalesAreas.AddRange(agChannelGroupSalesAreaList);
            salesAreaCount = agChannelGroupSalesAreaList.Count;

            return agChannelGroupSalesAreas;
        }

        private static List<AgChannelGroup> LoadAgChannelGroup(IEnumerable<Tuple<int, int, SalesAreaGroup>> channelGroup,
            IReadOnlyCollection<SalesArea> salesAreas)
        {
            return channelGroup.Select(cg => new AgChannelGroup()
            {
                ChannelGroupNo = cg.Item1,
                CampaignNo = cg.Item2,
                AgChannelGroupSalesAreas = LoadAgChannelGroupSalesAreas(cg.Item3, salesAreas, out int salesAreaCount),
                NbrAgChannelGroupSalesArea = salesAreaCount
            }).ToList();
        }
    }
}
