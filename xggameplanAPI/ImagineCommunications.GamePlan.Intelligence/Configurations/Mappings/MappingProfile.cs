using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.BookingPositionGroup;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Breaks;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.BreakTypes;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Campaign;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Clash;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.ClashExceptions;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.DayPartGroups;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.DayParts;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Demographics;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.InventoryStatus.InventoryLock;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.InventoryStatus.InventoryType;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.InventoryStatus.LockType;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.LengthFactor;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Product;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Programme;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.ProgrammeCategory;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.ProgrammeClassification;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.RatingsPredictionSchedules;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Restriction;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.SalesArea;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Spot;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.SpotBookingRules;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.TotalRatings;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Universe;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.SalesArea;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions.Objects;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.DayParts.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Domain.InventoryStatuses.Objects;
using ImagineCommunications.GamePlan.Domain.PositionInBreaks.BookingPositionGroups.Objects;
using ImagineCommunications.GamePlan.Domain.RatingSchedules;
using ImagineCommunications.GamePlan.Domain.Shared.Metadatas;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.SpotBookingRules;
using ImagineCommunications.GamePlan.Intelligence.Configurations.Converters;
using NodaTime;
using xggameplan.common.Extensions;
using xggameplan.core.Helpers;
using Clash = ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects.Clash;
using ClashDifference = ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects.ClashDifference;
using Demographic = ImagineCommunications.GamePlan.Domain.Shared.Demographics.Demographic;
using IncludeOrExclude = ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions.IncludeOrExclude;
using IntegrationDomain = ImagineCommunications.Gameplan.Integration.Contracts.Models;
using PositionGroupAssociation = ImagineCommunications.Gameplan.Integration.Contracts.Models.BookingPositionGroup.PositionGroupAssociation;
using ProgrammeClassification = ImagineCommunications.GamePlan.Domain.Shared.ProgrammeClassifications.ProgrammeClassification;
using ProgrammeDictionaryEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ProgrammeDictionary;
using ProgrammeEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Programmes.Programme;
using ProgrammeEpisodeEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Programmes.ProgrammeEpisode;
using Spot = ImagineCommunications.GamePlan.Domain.Spots.Spot;
using TotalRating = ImagineCommunications.GamePlan.Domain.TotalRatings.TotalRating;
using Universe = ImagineCommunications.GamePlan.Domain.Shared.Universes.Universe;

namespace ImagineCommunications.GamePlan.Intelligence.Configurations.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TimeSpan, Duration>().ConvertUsing<TimeSpanToNodaDurationConverter>();

            _ = CreateMap<IProgrammeCreated, ProgrammeDictionaryEntity>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ProgrammeName));

            _ = CreateMap<IProgrammeCreated, ProgrammeEntity>()
                .ForMember(dest => dest.SalesArea, opt => opt.Ignore())
                .ForMember(dest => dest.Episode, opt => opt.Ignore());

            _ = CreateMap<IntegrationDomain.Shared.ProgrammeEpisode, ProgrammeEpisodeEntity>();

            _ = CreateMap<IDemographicCreatedOrUpdated, Demographic>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.CustomId)).ReverseMap();
            _ = CreateMap<IUniverseCreated, Universe>();
            _ = CreateMap<IProgrammeClassificationCreated, ProgrammeClassification>().ReverseMap();
            _ = CreateMap<IClashCreatedOrUpdated, Clash>()
                .ForMember(dest => dest.DefaultPeakExposureCount, opt => opt.MapFrom(src => src.ExposureCount))
                .ForMember(dest => dest.DefaultOffPeakExposureCount, opt => opt.MapFrom(src => src.ExposureCount)).ReverseMap();

            _ = CreateMap<ClashDifference, IntegrationDomain.Clash.ClashDifference>().ReverseMap();

            _ = CreateMap<IClashUpdated, Clash>()
                .ForMember(dest => dest.DefaultPeakExposureCount, opt => opt.MapFrom(src => src.ExposureCount))
                .ForMember(dest => dest.DefaultOffPeakExposureCount, opt => opt.MapFrom(src => src.ExposureCount)).ReverseMap();
            _ = CreateMap<IProductCreatedOrUpdated, Product>()
                .ForMember(dest => dest.AdvertiserIdentifier, opt => opt.MapFrom(src => src.Advertisers.First().AdvertiserIdentifier))
                .ForMember(dest => dest.AdvertiserName, opt => opt.MapFrom(src => src.Advertisers.First().Name))
                .ForMember(dest => dest.AdvertiserLinkStartDate, opt => opt.MapFrom(src => src.Advertisers.First().StartDate))
                .ForMember(dest => dest.AdvertiserLinkEndDate, opt => opt.MapFrom(src => src.Advertisers.First().EndDate))
                .ForMember(dest => dest.AgencyName, opt => opt.MapFrom(src => src.Agencies.First().Name))
                .ForMember(dest => dest.AgencyStartDate, opt => opt.MapFrom(src => src.Agencies.First().StartDate))
                .ForMember(dest => dest.AgencyLinkEndDate, opt => opt.MapFrom(src => src.Agencies.First().EndDate))
                .ForMember(dest => dest.AgencyIdentifier, opt => opt.MapFrom(src => src.Agencies.First().AgencyIdentifier))
                .ReverseMap();
            _ = CreateMap<IRatingsPredictionScheduleCreated, RatingsPredictionSchedule>().ReverseMap();
            _ = CreateMap<ISalesAreaCreatedOrUpdated, SalesArea>().ReverseMap();
            _ = CreateMap<ISalesAreaUpdated, SalesArea>().ReverseMap();
            _ = CreateMap<IRestrictionCreatedOrUpdated, Restriction>()
                .ForMember(x => x.SchoolHolidayIndicator,
                    y => y.MapFrom(m => Enum.Parse(typeof(IncludeOrExcludeOrEither), m.SchoolHolidayIndicator.ToString())))
                .ForMember(x => x.PublicHolidayIndicator,
                    y => y.MapFrom(m => Enum.Parse(typeof(IncludeOrExcludeOrEither), m.PublicHolidayIndicator.ToString())))
                .ForMember(x => x.LiveProgrammeIndicator,
                    y => y.MapFrom(m => Enum.Parse(typeof(IncludeOrExclude), m.LiveProgrammeIndicator.ToString())))
                .ForMember(x => x.RestrictionType,
                    y => y.MapFrom(m => Enum.Parse(typeof(RestrictionType), m.RestrictionType.ToString())))
                .ForMember(x => x.RestrictionBasis,
                    y => y.MapFrom(m => Enum.Parse(typeof(RestrictionBasis), m.RestrictionBasis.ToString())))
                .ForMember(x => x.ProgrammeClassificationIndicator,
                    y => y.MapFrom(m => Enum.Parse(typeof(IncludeOrExclude), m.ProgrammeClassificationIndicator.ToString())))
                .ForMember(x => x.EpisodeNumber, opt => opt.MapFrom(src => src.EpisodeNo));
            _ = CreateMap<ISpotCreatedOrUpdated, Spot>()
                .ForMember(y => y.SpotLength, y => y.MapFrom(m => Duration.FromTimeSpan(m.SpotLength))).ReverseMap();
            _ = CreateMap<IBreakCreated, Break>().ReverseMap();
            _ = CreateMap<IBulkBreakCreated, List<Break>>().ReverseMap();
            _ = CreateMap<ICampaignCreatedOrUpdated, Campaign>()
                .ForMember(dest => dest.DeliveryType,
                    opt => opt.MapFrom(src => Enum.Parse(typeof(CampaignDeliveryType), src.DeliveryType)))
                .ForMember(dest => dest.CampaignGroup,
                    opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.CampaignGroup) ? null : src.CampaignGroup))
                .ForMember(dest => dest.IncludeRightSizer, opt => opt.MapFrom(campaign => ResolveIncludeRightSizer(campaign)))
                .ForMember(dest => dest.RightSizerLevel, opt => opt.MapFrom(campaign => ResolveRightSizerLevel(campaign)))
                .ForMember(dest => dest.DeliveryCurrency, opt => opt.MapFrom(src => src.DeliveryCurrency))
                .ForMember(dest => dest.TopTail, opt => opt.MapFrom(src => src.TopTail));
            _ = CreateMap<IntegrationDomain.Campaign.SalesAreaCampaignTarget, SalesAreaCampaignTarget>().ReverseMap();
            _ = CreateMap<IntegrationDomain.Campaign.CampaignBookingPositionGroup, CampaignBookingPositionGroup>().ReverseMap();
            _ = CreateMap<IntegrationDomain.Campaign.CampaignTarget, CampaignTarget>().ReverseMap();

            _ = CreateMap<IntegrationDomain.Shared.MultipartLength, MultipartLength>().ReverseMap();
            _ = CreateMap<IntegrationDomain.Shared.RatingModel, Rating>().ReverseMap();
            _ = CreateMap<IntegrationDomain.Shared.TimeRestriction, TimeRestriction>().ReverseMap();
            _ = CreateMap<IntegrationDomain.Shared.ProgrammeRestriction, ProgrammeRestriction>()
                .ForMember(dest => dest.CategoryOrProgramme, opt =>
                {
                    opt.MapFrom(src =>
                        src.CategoryOrProgramme == null
                            ? null
                            : (src.IsCategoryOrProgramme == "P"
                                ? src.CategoryOrProgramme.Select(Lmks353InterimHelper.GetExternalRefIfExists).ToList()
                                : src.CategoryOrProgramme));
                });
            _ = CreateMap<IntegrationDomain.Shared.SalesAreaGroup, SalesAreaGroup>().ReverseMap();
            _ = CreateMap<IntegrationDomain.Shared.Multipart, Multipart>().ReverseMap();
            _ = CreateMap<IntegrationDomain.Shared.StrikeWeight, StrikeWeight>().ReverseMap();
            _ = CreateMap<IntegrationDomain.Shared.Length, Length>().ReverseMap();
            _ = CreateMap<IntegrationDomain.Shared.DayPart, DayPart>().ReverseMap();
            _ = CreateMap<IntegrationDomain.Shared.Timeslice, Timeslice>().ReverseMap();
            _ = CreateMap<IntegrationDomain.Shared.DayPartLength, DayPartLength>().ReverseMap();
            _ = CreateMap<IntegrationDomain.Campaign.CampaignPayback, CampaignPayback>().ReverseMap();

            _ = CreateMap<IClashExceptionCreated, ClashException>().ReverseMap();
            _ = CreateMap<IntegrationDomain.SharedModels.TimeAndDow, TimeAndDow>().ReverseMap();

            _ = CreateMap<IBookingPositionGroupCreated, BookingPositionGroup>().ReverseMap();
            _ = CreateMap<PositionGroupAssociation, Domain.PositionInBreaks.BookingPositionGroups.Objects.PositionGroupAssociation>().ReverseMap();

            _ = CreateMap<SalesAreaDemographic, Domain.Shared.SalesAreaDemographics.SalesAreaDemographic>().ReverseMap();
            _ = CreateMap<IProgrammeCategoryCreated, Domain.ProgrammeCategory.ProgrammeCategoryHierarchy>().ReverseMap();
            _ = CreateMap<ILengthFactorCreated, Domain.LengthFactors.LengthFactor>().ReverseMap();

            _ = CreateMap<ILockTypeCreated, InventoryLockType>().ReverseMap();
            _ = CreateMap<IBulkLockTypeCreated, List<InventoryLockType>>().ReverseMap();
            _ = CreateMap<IInventoryTypeCreated, InventoryType>().ReverseMap();
            _ = CreateMap<IBulkInventoryTypeCreated, List<InventoryType>>().ReverseMap();
            _ = CreateMap<IInventoryLockCreated, InventoryLock>().ReverseMap();
            _ = CreateMap<IBulkInventoryLockCreated, List<InventoryLock>>().ReverseMap();
            _ = CreateMap<ITotalRatingCreated, TotalRating>().ReverseMap();
            _ = CreateMap<IStandardDayPartCreated, StandardDayPart>().ReverseMap();
            _ = CreateMap<IntegrationDomain.Shared.StandardDayPartTimeslice, StandardDayPartTimeslice>().ReverseMap();
            _ = CreateMap<IStandardDayPartGroupCreated, StandardDayPartGroup>().ReverseMap();
            _ = CreateMap<IntegrationDomain.Shared.StandardDayPartSplit, StandardDayPartSplit>().ReverseMap();
            _ = CreateMap<ISpotBookingRuleCreated, SpotBookingRule>().ReverseMap();

            _ = CreateMap<IBreakTypeCreated, Data>()
                .ForMember(x => x.Value, y => y.MapFrom(m => m.Name))
                .ReverseMap();

            _ = CreateMap<CampaignBreakRequirement, IntegrationDomain.Campaign.CampaignBreakRequirement>().ReverseMap();
            _ = CreateMap<CampaignBreakRequirementItem, IntegrationDomain.Campaign.CampaignBreakRequirementItem>().ReverseMap();
        }

        private static object ResolveIncludeRightSizer(ICampaignCreatedOrUpdated source)
        {
            if (!source.IncludeRightSizer.TryGetValueFromDescription<IncludeRightSizer>(out var value))
            {
                return false;
            }

            switch (value)
            {
                case IncludeRightSizer.No:
                    return false;

                case IncludeRightSizer.CampaignLevel:
                case IncludeRightSizer.DetailLevel:
                    return true;

                default:
                    return false;
            }
        }

        private static object ResolveRightSizerLevel(ICampaignCreatedOrUpdated source)
        {
            if (!source.IncludeRightSizer.TryGetValueFromDescription<IncludeRightSizer>(out var value))
            {
                return null;
            }

            switch (value)
            {
                case IncludeRightSizer.No:
                    return null;

                case IncludeRightSizer.CampaignLevel:
                    return RightSizerLevel.CampaignLevel;

                case IncludeRightSizer.DetailLevel:
                    return RightSizerLevel.DetailLevel;

                default:
                    return null;
            }
        }
    }
}
