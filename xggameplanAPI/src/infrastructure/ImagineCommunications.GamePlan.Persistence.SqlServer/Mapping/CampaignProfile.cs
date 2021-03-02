using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Dto;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using NodaTime;
using CampaignBookingPositionGroup = ImagineCommunications.GamePlan.Domain.Campaigns.Objects.CampaignBookingPositionGroup;
using CampaignBreakRequirement = ImagineCommunications.GamePlan.Domain.Campaigns.Objects.CampaignBreakRequirement;
using CampaignBreakRequirementItem = ImagineCommunications.GamePlan.Domain.Campaigns.Objects.CampaignBreakRequirementItem;
using CampaignPayback = ImagineCommunications.GamePlan.Domain.Campaigns.Objects.CampaignPayback;
using CampaignSettings = ImagineCommunications.GamePlan.Domain.Campaigns.Objects.CampaignSettings;
using RightSizerLevel = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.RightSizerLevel;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class CampaignProfile : Profile
    {
        public CampaignProfile()
        {
            _ = CreateMap<CampaignWithProductFlatModel, CompactCampaign>()
                .ReverseMap()
                .ForMember(dest => dest.MediaGroup, opt => opt.MapFrom(src =>
                    src.AgencyGroup != null || src.AgencyGroup.ShortName != null
                        ? new AgencyGroupModel { ShortName = src.AgencyGroup.ShortName, Code = src.AgencyGroup.Code }
                        : null))
                .ForMember(dest => dest.ProductAssigneeName, opt => opt.MapFrom(src => src.SalesExecutiveName));

            _ = CreateMap<CampaignTargetStrikeWeightDayPartTimeSlice, Timeslice>()
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CampaignTargetStrikeWeightDayPartId, opt => opt.Ignore());

            _ = CreateMap<CampaignTargetStrikeWeightDayPartLength, DayPartLength>()
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CampaignTargetStrikeWeightDayPartId, opt => opt.Ignore());

            _ = CreateMap<CampaignTargetStrikeWeightDayPart, DayPart>()
                .ReverseMap()
                .ForMember(dest => dest.Lengths, opt =>
                {
                    opt.PreCondition(src => src.Lengths != null);
                    opt.MapFrom(src => src.Lengths);
                })
                .ForMember(dest => dest.Timeslices, opt =>
                {
                    opt.PreCondition(src => src.Timeslices != null);
                    opt.MapFrom(src => src.Timeslices);
                })
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CampaignTargetStrikeWeightId, opt => opt.Ignore());

            _ = CreateMap<CampaignTargetStrikeWeightLength, Domain.Campaigns.Objects.Length>()
                .ForMember(dest => dest.length, opt => opt.MapFrom(src => src.Length))
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CampaignTargetStrikeWeightId, opt => opt.Ignore());

            _ = CreateMap<CampaignTargetStrikeWeight, StrikeWeight>()
                .ReverseMap()
                .ForMember(dest => dest.Lengths, opt =>
                {
                    opt.PreCondition(src => src.Lengths != null);
                    opt.MapFrom(src => src.Lengths);
                })
                .ForMember(dest => dest.DayParts, opt =>
                {
                    opt.PreCondition(src => src.DayParts != null);
                    opt.MapFrom(src => src.DayParts);
                })
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CampaignTargetId, opt => opt.Ignore());

            _ = CreateMap<Entities.Tenant.Campaigns.CampaignTarget, Domain.Campaigns.Objects.CampaignTarget>()
                .ReverseMap()
                .ForMember(dest => dest.StrikeWeights, opt =>
                {
                    opt.PreCondition(src => src.StrikeWeights != null);
                    opt.MapFrom(src => src.StrikeWeights);
                })
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CampaignSalesAreaTargetId, opt => opt.Ignore());

            _ = CreateMap<CampaignSalesAreaTargetGroup, SalesAreaGroup>()
                .ForMember(dest => dest.SalesAreas, opt => opt.MapFrom(src => src.SalesAreas.Select(x => x.Name).ToList()))
                .ReverseMap()
                .ForMember(dest => dest.SalesAreas, opt =>
                {
                    opt.PreCondition(src => src.SalesAreas != null);
                    opt.MapFrom(src => new HashSet<CampaignSalesAreaTargetGroupSalesArea>(
                        src.SalesAreas.Select(x => new CampaignSalesAreaTargetGroupSalesArea
                        {
                            Name = x
                        })));
                })
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CampaignSalesAreaTargetId, opt => opt.Ignore());

            _ = CreateMap<CampaignSalesAreaTargetMultipart, Multipart>()
                .ForMember(dest => dest.Lengths,
                    opt => opt.MapFrom(src => src.Lengths.Select(x => new MultipartLength
                    {
                        Length = Duration.FromTimeSpan(x.Length),
                        BookingPosition = x.BookingPosition,
                        Sequencing = x.Sequencing
                    }).ToList()))
                .ReverseMap()
                .ForMember(dest => dest.Lengths, opt =>
                {
                    opt.PreCondition(src => src.Lengths != null);
                    opt.MapFrom(src => src.Lengths.Select(x => new CampaignSalesAreaTargetMultipartLength
                    {
                        Length = x.Length.ToTimeSpan(),
                        BookingPosition = x.BookingPosition,
                        Sequencing = x.Sequencing
                    }));
                })
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CampaignSalesAreaTargetId, opt => opt.Ignore());

            _ = CreateMap<CampaignSalesAreaTarget, SalesAreaCampaignTarget>()
                .ReverseMap()
                .ForMember(dest => dest.Multiparts, opt =>
                {
                    opt.PreCondition(src => src.Multiparts != null);
                    opt.MapFrom(src => src.Multiparts);
                })
                .ForMember(dest => dest.CampaignTargets, opt =>
                {
                    opt.PreCondition(src => src.CampaignTargets != null);
                    opt.MapFrom(src => src.CampaignTargets);
                })
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CampaignId, opt => opt.Ignore());

            _ = CreateMap<CampaignProgrammeRestriction, ProgrammeRestriction>()
                .ForMember(dest => dest.SalesAreas, opt => opt.MapFrom(src => src.SalesAreas.Select(x => x.Name).ToList()))
                .ForMember(dest => dest.CategoryOrProgramme, opt => opt.MapFrom(src => src.CategoryOrProgramme.Select(x => x.Name).ToList()))
                .ReverseMap()
                .ForMember(dest => dest.SalesAreas, opt =>
                {
                    opt.PreCondition(src => src.SalesAreas != null);
                    opt.MapFrom(src => new HashSet<CampaignProgrammeRestrictionSalesArea>(
                        src.SalesAreas.Select(x => new CampaignProgrammeRestrictionSalesArea
                        {
                            Name = x
                        })));
                })
                .ForMember(dest => dest.CategoryOrProgramme, opt =>
                {
                    opt.PreCondition(src => src.CategoryOrProgramme != null);
                    opt.MapFrom(src => new HashSet<CampaignProgrammeRestrictionCategoryOrProgramme>(
                        src.CategoryOrProgramme.Select(x => new CampaignProgrammeRestrictionCategoryOrProgramme
                        {
                            Name = x
                        })));
                })
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CampaignId, opt => opt.Ignore());

            _ = CreateMap<CampaignTimeRestriction, TimeRestriction>()
                .ForMember(dest => dest.SalesAreas,
                    opt => opt.MapFrom(src => src.SalesAreas.Select(x => x.Name).ToList()))
                .ReverseMap()
                .ForMember(dest => dest.SalesAreas, opt =>
                {
                    opt.PreCondition(src => src.SalesAreas != null);
                    opt.MapFrom(src => new HashSet<CampaignTimeRestrictionSalesArea>(
                        src.SalesAreas.Select(x => new CampaignTimeRestrictionSalesArea
                        {
                            Name = x
                        })));
                })
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CampaignId, opt => opt.Ignore());

            _ = CreateMap<Entities.Tenant.Campaigns.CampaignBookingPositionGroup, CampaignBookingPositionGroup>()
                .ForMember(dest => dest.SalesAreas,
                    opt => opt.MapFrom(src => src.SalesAreas.Select(x => x.Name).ToList()))
                .ReverseMap()
                .ForMember(dest => dest.SalesAreas, opt =>
                {
                    opt.PreCondition(src => src.SalesAreas != null);
                    opt.MapFrom(src => new HashSet<CampaignBookingPositionGroupSalesArea>(
                        src.SalesAreas.Select(x => new CampaignBookingPositionGroupSalesArea
                        {
                            Name = x
                        })));
                })
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CampaignId, opt => opt.Ignore());

            _ = CreateMap<Entities.Tenant.Campaigns.Campaign, Domain.Campaigns.Objects.Campaign>()
                .ForMember(dest => dest.DemoGraphic, opt => opt.MapFrom(src => src.Demographic))
                .ForMember(dest => dest.BreakType,
                    opt => opt.MapFrom(src => src.BreakTypes.Select(x => x.Name).ToList()))
                .ForMember(dest => dest.SalesAreaCampaignTarget,
                    opt => opt.MapFrom(src => src.SalesAreaCampaignTargets))
                .ForMember(dest => dest.DeliveryType, opt => opt.MapFrom(src => (CampaignDeliveryType)src.DeliveryType))
                .ForMember(dest => dest.RightSizerLevel, opt => opt.MapFrom(src => (Domain.Campaigns.RightSizerLevel?)src.RightSizerLevel))
                .ForMember(dest => dest.DeliveryCurrency, opt => opt.MapFrom(src => (Domain.Campaigns.DeliveryCurrency)src.DeliveryCurrency))
                .ForMember(dest => dest.TopTail, opt => opt.MapFrom(src => (TopTail?)src.TopTail))
                .ReverseMap()
                .ForMember(dest => dest.BreakTypes, opt =>
                {
                    opt.PreCondition((sourceValue, rc) =>
                        !rc.Options.AreCollectionsIgnored() &&
                        sourceValue.BreakType != null);
                    opt.MapFrom(src => new HashSet<CampaignBreakType>(src.BreakType.Select(x => new CampaignBreakType
                    {
                        Name = x
                    })));
                })
                .ForMember(dest => dest.TimeRestrictions, opt =>
                {
                    opt.PreCondition((sourceValue, rc) =>
                        !rc.Options.AreCollectionsIgnored() &&
                        sourceValue.TimeRestrictions != null);
                    opt.MapFrom(src => src.TimeRestrictions);
                })
                .ForMember(dest => dest.ProgrammeRestrictions, opt =>
                {
                    opt.PreCondition((sourceValue, rc) =>
                        !rc.Options.AreCollectionsIgnored() &&
                        sourceValue.ProgrammeRestrictions != null);
                    opt.MapFrom(src => src.ProgrammeRestrictions);
                })
                .ForMember(dest => dest.SalesAreaCampaignTargets, opt =>
                {
                    opt.PreCondition((sourceValue, rc) =>
                        !rc.Options.AreCollectionsIgnored() &&
                        sourceValue.SalesAreaCampaignTarget != null);
                    opt.MapFrom(src => src.SalesAreaCampaignTarget);
                })
                .ForMember(dest => dest.BookingPositionGroups, opt =>
                {
                    opt.PreCondition((sourceValue, rc) =>
                        !rc.Options.AreCollectionsIgnored() &&
                        sourceValue.BookingPositionGroups != null);
                    opt.MapFrom(src => src.BookingPositionGroups);
                })
                .ForMember(dest => dest.CampaignPaybacks, opt =>
                {
                    opt.PreCondition((sourceValue, rc) =>
                        !rc.Options.AreCollectionsIgnored() &&
                        sourceValue.CampaignPaybacks != null);
                    opt.MapFrom(src => src.CampaignPaybacks);
                });

            _ = CreateMap<CampaignSearchDto, CampaignWithProductFlatModel>()
                .ForMember(dest => dest.DeliveryType, opt => opt.MapFrom(src => (CampaignDeliveryType)src.DeliveryType))
                .ForMember(dest => dest.TopTail, opt => opt.MapFrom(src => (TopTail?)src.TopTail))
                .ForMember(dest => dest.IncludeRightSizer, opt => opt.MapFrom(src => src.IncludeRightSizer
                    ? (src.RightSizerLevel == RightSizerLevel.CampaignLevel
                        ? IncludeRightSizer.CampaignLevel
                        : IncludeRightSizer.DetailLevel)
                    : IncludeRightSizer.No))
                .ForMember(dest => dest.DefaultCampaignPassPriority,
                    opt => opt.MapFrom(src =>
                        ValidateCampaignPassPriority(src.CampaignPassPriority)
                            ? src.CampaignPassPriority
                            : (int)PassPriorityType.Exclude))
                .ForMember(dest => dest.ProductAssigneeName, opt => opt.MapFrom(src => src.ProductAssigneeName))
                .ForMember(dest => dest.MediaGroup,
                    opt => opt.MapFrom(src =>
                        src.MediaGroupCode == null && src.MediaGroupShortName == null
                            ? null
                            : new AgencyGroupModel
                            {
                                Code = src.MediaGroupCode,
                                ShortName = src.MediaGroupShortName
                            }));

            _ = CreateMap<Entities.Tenant.Campaigns.Campaign, CampaignReducedModel>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(IsActivePredicate))
                .ForMember(dest => dest.DemoGraphic, opt => opt.MapFrom(src => src.Demographic))
                .ForMember(dest => dest.DeliveryType, opt => opt.MapFrom(src => (CampaignDeliveryType)src.DeliveryType));

            _ = CreateMap<Entities.Tenant.Campaigns.Campaign, CampaignNameModel>();
            _ = CreateMap<CampaignBreakRequirement, Entities.Tenant.Campaigns.CampaignBreakRequirement>().ReverseMap();
            _ = CreateMap<CampaignBreakRequirementItem, Entities.Tenant.Campaigns.CampaignBreakRequirementItem>().ReverseMap();
            _ = CreateMap<CampaignBreakRequirementItem, Entities.Tenant.Campaigns.CampaignCentreBreakRequirementItem>().ReverseMap();
            _ = CreateMap<CampaignBreakRequirementItem, Entities.Tenant.Campaigns.CampaignEndBreakRequirementItem>().ReverseMap();

            _ = CreateMap<Entities.Tenant.Campaigns.CampaignPayback, CampaignPayback>().ReverseMap();

            _ = CreateMap<Entities.Tenant.Campaigns.CampaignSettings, CampaignSettings>().ReverseMap();
        }

        private static bool ValidateCampaignPassPriority(int campaignPassPriority)
        {
            return campaignPassPriority >= (int)PassPriorityType.Exclude
                   && campaignPassPriority <= (int)PassPriorityType.Include;
        }

        internal static readonly Expression<Func<Entities.Tenant.Campaigns.Campaign, bool>> IsActivePredicate = c =>
            c.Status != Entities.CampaignStatus.Cancelled &&
            (c.DeliveryType == Entities.CampaignDeliveryType.Spot
                ? c.TargetRatings >= default(decimal)
                : c.TargetZeroRatedBreaks || c.TargetRatings > default(decimal)) && c.SalesAreaCampaignTargets.Any();
    }
}
