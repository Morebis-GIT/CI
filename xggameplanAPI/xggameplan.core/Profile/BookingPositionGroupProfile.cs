using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.PositionInBreaks.BookingPositionGroups.Objects;
using ImagineCommunications.GamePlan.Domain.PositionInBreaks.BookingPositions;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using xggameplan.Model.AutoGen;

namespace xggameplan.Profile
{
    public class BookingPositionGroupProfile : AutoMapper.Profile
    {
        public BookingPositionGroupProfile()
        {
            CreateMap<(CampaignBookingPositionGroup, int, List<AgPositionGroupAssociation>, List<SalesArea>), List<AgBookingPositionGroup>>()
                .ConstructUsing((g, rc) => LoadAgBookingPositionGroups(g.Item1, g.Item2, g.Item3, g.Item4, rc.Mapper));

            CreateMap<(PositionGroupAssociation, BookingPosition), AgPositionGroupAssociation>()
                .ForMember(ga => ga.BookingPosition, exp => exp.MapFrom(x => x.Item1.BookingPosition))
                .ForMember(ga => ga.BookingOrder, exp => exp.MapFrom(x => x.Item1.BookingOrder))
                .ForMember(ga => ga.SubBookingOrder, exp => exp.MapFrom(x => x.Item2.BookingOrder));
        }

        private static List<AgBookingPositionGroup> LoadAgBookingPositionGroups(
            CampaignBookingPositionGroup campaignPositionGroup, int campaignNo,
            List<AgPositionGroupAssociation> positionGroupAssociationsList, List<SalesArea> allSalesAreas, IMapper mapper)
        {
            List<SalesArea> salesAreas;
            if (campaignPositionGroup.SalesAreas == null || !campaignPositionGroup.SalesAreas.Any())
            {
                salesAreas = new List<SalesArea> {new SalesArea {CustomId = 0}};
            }
            else
            {
                salesAreas = allSalesAreas.Where(s => campaignPositionGroup.SalesAreas.Contains(s.Name)).ToList();
            }

            var agRequirement = mapper.Map<AgRequirement>(Tuple.Create(campaignPositionGroup.DesiredPercentageSplit, campaignPositionGroup.CurrentPercentageSplit));
            var positionGroupAssociations = new AgPositionGroupAssociations();

            if (positionGroupAssociationsList != null)
            {
                positionGroupAssociations.AddRange(positionGroupAssociationsList);
            }

            return salesAreas
                .Select(salesArea => new AgBookingPositionGroup
                {
                    CampaignNo = campaignNo,
                    SalesAreaNo = salesArea?.CustomId ?? 0,
                    GroupId = campaignPositionGroup.GroupId,
                    DiscountSurchargePercentage = campaignPositionGroup.DiscountSurchargePercentage,
                    AgBookingPositionGroupRequirement = agRequirement,
                    PositionGroupAssociationsCount = positionGroupAssociationsList?.Count ?? 0,
                    PositionGroupAssociations = positionGroupAssociations
                }).ToList();
        }
    }
}
