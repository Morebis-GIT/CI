using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using NodaTime;
using xggameplan.common.Extensions;
using xggameplan.CSVImporter;
using xggameplan.model.External.Campaign;
using xggameplan.Model;
using xggameplan.OutputFiles.Processing;

namespace xggameplan.Profile
{
    public class CampaignProfile : AutoMapper.Profile
    {
        public const string MultiplePaybackTypesSign = "MULTIPLE";

        public CampaignProfile()
        {
            CreateMap<CreateCampaign, Campaign>()
                .ForMember(dest => dest.CampaignGroup, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.CampaignGroup) ? null : src.CampaignGroup))
                .ForMember(dest => dest.IncludeRightSizer, opt => opt.MapFrom(campaign => ResolveIncludeRightSizer(campaign)))
                .ForMember(dest => dest.RightSizerLevel, opt => opt.MapFrom(campaign => ResolveRightSizerLevel(campaign)))
                .ForMember(dest => dest.DeliveryType, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.DeliveryType) ? default(CampaignDeliveryType) : Enum.Parse(typeof(CampaignDeliveryType), src.DeliveryType)));

            CreateMap<Tuple<decimal, decimal>, AgRequirement>().ConstructUsing(src => LoadAgRequirement(src));

            CreateMap<Campaign, CampaignModel>()
                .ForMember(campaignModel => campaignModel.Uid, expression => expression.MapFrom(campaign => campaign.Id))
                .ForMember(campaignModel => campaignModel.ProgrammeRestrictions, expression => expression.MapFrom(campaign => campaign.ProgrammeRestrictions.Select(pr =>
                    new ProgrammeRestrictionViewModel
                    {
                        CategoryOrProgramme = pr.CategoryOrProgramme,
                        IsCategoryOrProgramme = pr.IsCategoryOrProgramme,
                        IsIncludeOrExclude = pr.IsIncludeOrExclude,
                        SalesAreas = pr.SalesAreas
                    }).ToList()));

            CreateMap<Campaign, CampaignReportModel>().ForMember(campaignModel => campaignModel.Uid, expression => expression.MapFrom(campaign => campaign.Id));

            CreateMap<CampaignWithProductFlatModel, CampaignReportModel>()
                .ForMember(dest => dest.CampaignPassPriority, opt => opt.MapFrom(src => src.DefaultCampaignPassPriority))
                .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.ProductExternalRef))
                .ForMember(dest => dest.RightSizerLevel, opt => opt.MapFrom(src =>
                    src.IncludeRightSizer == IncludeRightSizer.CampaignLevel
                        ? RightSizerLevel.CampaignLevel
                        : src.IncludeRightSizer == IncludeRightSizer.DetailLevel
                            ? RightSizerLevel.DetailLevel
                            : (RightSizerLevel?)null))
                .ForMember(dest => dest.IncludeRightSizer, opt => opt.MapFrom(src =>
                    src.IncludeRightSizer == IncludeRightSizer.CampaignLevel || src.IncludeRightSizer == IncludeRightSizer.DetailLevel))
                .ForMember(dest => dest.MediaSalesGroup, opt => opt.MapFrom(src => src.MediaGroup == null ? null : src.MediaGroup.ShortName))
                .ForMember(dest => dest.ProductAssignee, opt => opt.MapFrom(src => src.ProductAssigneeName))
                .ForMember(dest => dest.Payback, opt => opt.MapFrom(src =>
                    src.CampaignPaybacks == null || !src.CampaignPaybacks.Any() ? default(double?) : src.CampaignPaybacks.Sum(x => x.Amount)))
                .ForMember(dest => dest.PaybackType, opt => opt.MapFrom(src => ResolvePaybackType(src.CampaignPaybacks)));

            CreateMap<Campaign, CampaignWithProductFlatModel>()
                .ForMember(cl => cl.ProductExternalRef, dest => dest.MapFrom(ca => ca.Product))
                .ForMember(cl => cl.IncludeRightSizer, dest => dest.MapFrom(ca => ca.RightSizerLevel));

            CreateMap<SalesAreaCampaignTarget, Model.SalesAreaCampaignTargetModel>();
            CreateMap<Multipart, MultipartModel>()
                .ForMember(dest => dest.Lengths, opt => opt.MapFrom(src => src.Lengths.Select(x => x.Length).ToList()));
            CreateMap<CampaignTarget, CampaignTargetModel>();
            CreateMap<StrikeWeight, StrikeWeightModel>();
            CreateMap<DayPart, DayPartModel>()
                .ForMember(dest => dest.Timeslices, opt => opt.MapFrom(src => src.Timeslices.OrderBy(x => x.DowPattern.First())));
            CreateMap<Timeslice, TimesliceModel>().ReverseMap();
            CreateMap<DayPartLength, DayPartLengthModel>().ReverseMap();
            CreateMap<CampaignProgramme, CampaignProgrammeModel>().ReverseMap();
            CreateMap<CampaignPayback, CampaignPaybackModel>().ReverseMap();

            CreateMap<CompactCampaign, CampaignWithProductFlatModel>().ReverseMap();

            CreateMap<CampaignPassPriority, CreateCampaignPassPriorityModel>()
                    .ForMember(c => c.CampaignExternalId, o => o.MapFrom(cc => cc != null && cc.Campaign != null ?
                                                                               cc.Campaign.ExternalId : string.Empty))
                    .ForMember(c => c.PassPriorities, o => o.MapFrom(cc => cc != null && cc.PassPriorities != null ?
                                                                           cc.PassPriorities : new List<PassPriority>()));

            CreateMap<Tuple<List<CreateCampaignPassPriorityModel>, List<CampaignWithProductFlatModel>>, List<CampaignPassPriority>>()
                    .ConvertUsing((src, list, rc) => MapToCampaignPassPriorities(src, rc.Mapper));

            CreateMap<Tuple<List<CampaignWithProductFlatModel>, List<PassModel>>, List<CreateCampaignPassPriorityModel>>()
                    .ConvertUsing(src => MapToCreateCampaignPassPriorityModels(src));

            CreateMap<Tuple<List<CampaignWithProductFlatModel>, List<Pass>>, List<CreateCampaignPassPriorityModel>>()
                   .ConvertUsing(src => MapToCreateCampaignPassPriorityModels(src));
            CreateMap<Tuple<List<CampaignWithProductFlatModel>, List<Pass>>, List<CreateCampaignPassPriorityModel>>()
                   .ConvertUsing(tuple => MapToCreateCampaignPassPriorityModels(tuple));

            CreateMap<Tuple<CampaignBaseFlattenedModel, SalesAreaCampaignTargetFlattenModel, DurationFlattenModel,
                StrikeWeightFlattenModel, DayPartFlattenModel>, CampaignFlattenedModel>().
                ConvertUsing((tuple, model, rc) =>
                    MapToCreateCampaignFlattenedModel(tuple.Item1, tuple.Item2, tuple.Item3, rc.Mapper, tuple.Item4, tuple.Item5));
            CreateMap<CampaignBaseFlattenedModel, CampaignFlattenedModel>();
            CreateMap<SalesAreaCampaignTargetFlattenModel, CampaignFlattenedModel>();
            CreateMap<DurationFlattenModel, CampaignFlattenedModel>();
            CreateMap<StrikeWeightFlattenModel, CampaignFlattenedModel>();
            CreateMap<DayPartFlattenModel, CampaignFlattenedModel>();

            CreateMap<model.External.Campaign.SalesAreaCampaignTargetViewModel, SalesAreaCampaignTarget>();
            CreateMap<MultipartModel, Multipart>();
            CreateMap<Duration, MultipartLength>()
                .ConstructUsing(src => new MultipartLength
                {
                    Length = src
                });

            CreateMap<CampaignsReqmImport, CampaignsReqm>()
                .ForMember(dst => dst.Requirement, opt => opt.MapFrom(src => src.requirement))
                .ForMember(dst => dst.TotalSupplied, opt => opt.MapFrom(src => src.total_supplied));
        }

        private CampaignFlattenedModel MapToCreateCampaignFlattenedModel(
            CampaignBaseFlattenedModel baseCampaignFlattenedModel,
            SalesAreaCampaignTargetFlattenModel salesAreaCampaignTargetFlattenModel,
            DurationFlattenModel durationFlattenModel,
            IMapper mapper,
            StrikeWeightFlattenModel strikeWeightFlattenModel = null,
            DayPartFlattenModel dayPartFlattenModel = null)
        {
            var campaignFlattenedModel = mapper.Map<CampaignFlattenedModel>(baseCampaignFlattenedModel);
            campaignFlattenedModel = mapper.Map(salesAreaCampaignTargetFlattenModel, campaignFlattenedModel);
            campaignFlattenedModel = mapper.Map(durationFlattenModel, campaignFlattenedModel);
            if (strikeWeightFlattenModel != null)
            {
                campaignFlattenedModel = mapper.Map(strikeWeightFlattenModel, campaignFlattenedModel);
                if (dayPartFlattenModel != null)
                {
                    campaignFlattenedModel = mapper.Map(dayPartFlattenModel, campaignFlattenedModel);
                }
            }
            return campaignFlattenedModel;
        }

        private static List<CampaignPassPriority> MapToCampaignPassPriorities(
            Tuple<List<CreateCampaignPassPriorityModel>, List<CampaignWithProductFlatModel>> src, IMapper mapper)
        {
            var (createCampaignPassPriorityModels, campaigns) = src;

            var campaignPassPriorities = new List<CampaignPassPriority>();
            if (createCampaignPassPriorityModels != null && createCampaignPassPriorityModels.Any() && campaigns != null)
            {
                campaignPassPriorities.AddRange(
                createCampaignPassPriorityModels.Select(a => new CampaignPassPriority
                {
                    Campaign = mapper.Map<CompactCampaign>(campaigns.FirstOrDefault(c => c.ExternalId == a.CampaignExternalId)),
                    PassPriorities = mapper.Map<List<PassPriority>>(a.PassPriorities)
                }));
            }
            return campaignPassPriorities;
        }

        private static List<CreateCampaignPassPriorityModel> MapToCreateCampaignPassPriorityModels(
            Tuple<List<CampaignWithProductFlatModel>, List<PassModel>> src)
        {
            var (campaigns, passes) = src;

            var createCampaignPassPriorityModels = new List<CreateCampaignPassPriorityModel>();

            if (campaigns != null && campaigns.Any() && passes != null && passes.Any())
            {
                createCampaignPassPriorityModels = campaigns.Select(c => new CreateCampaignPassPriorityModel
                {
                    CampaignExternalId = c.ExternalId,
                    PassPriorities = passes.Select(p => new CreatePassPriorityModel
                    {
                        PassId = p.Id,
                        PassName = p.Name,
                        Priority = c.DefaultCampaignPassPriority
                    }).ToList()
                }).ToList();
            }

            return createCampaignPassPriorityModels;
        }

        private static List<CreateCampaignPassPriorityModel> MapToCreateCampaignPassPriorityModels(
            Tuple<List<CampaignWithProductFlatModel>, List<Pass>> src)
        {
            var (campaigns, passes) = src;

            var createCampaignPassPriorityModels = new List<CreateCampaignPassPriorityModel>();

            if (campaigns != null && campaigns.Any() && passes != null && passes.Any())
            {
                createCampaignPassPriorityModels = campaigns.Select(c => new CreateCampaignPassPriorityModel
                {
                    CampaignExternalId = c.ExternalId,
                    PassPriorities = passes.Select(p => new CreatePassPriorityModel
                    {
                        PassId = p.Id,
                        PassName = p.Name,
                        Priority = c.DefaultCampaignPassPriority
                    }).ToList()
                }).ToList();
            }

            return createCampaignPassPriorityModels;
        }

        private static AgRequirement LoadAgRequirement(Tuple<decimal, decimal> value)
        {
            return new AgRequirement
            {
                Required = value.Item1,
                TgtRequired = value.Item1,
                SareRequired = value.Item1,
                Supplied = value.Item2
            };
        }

        private static bool ResolveIncludeRightSizer(CreateCampaign source)
        {
            if (!source.IncludeRightSizer.TryGetValueFromDescription(out IncludeRightSizer value))
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

                default: //should we throw an exception here in case, when somehow validation was avoided and inappropriate value passed?
                    return false;
            }
        }

        private static RightSizerLevel? ResolveRightSizerLevel(CreateCampaign source)
        {
            if (!source.IncludeRightSizer.TryGetValueFromDescription(out IncludeRightSizer value))
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

        /// <summary>
        /// Resolves the type of the payback for campaign.
        /// </summary>
        /// <param name="src">The source.</param>
        /// <returns></returns>
        private static string ResolvePaybackType(IEnumerable<CampaignPayback> src)
        {
            if (src is null)
            {
                return null;
            }

            var distinctTypes = src.Select(x => x.Name).Distinct().Take(2).ToList();

            return distinctTypes.Count > 1 ? MultiplePaybackTypesSign : distinctTypes.FirstOrDefault();
        }
    }

    public class SalesAreaCampaignProfile : AutoMapper.Profile
    {
        public SalesAreaCampaignProfile()
        {
            _ = CreateMap<Tuple<List<Length>, int, int>, List<AgLength>>()
                .ConstructUsing((t, context) => LoadAgLengths(t.Item1, t.Item2, t.Item3, context.Mapper));
            _ = CreateMap<Tuple<List<StrikeWeight>, int, int>, List<AgStrikeWeight>>()
                .ConstructUsing((t, context) => LoadAgStrikeWeights(t.Item1, t.Item2, t.Item3, context.Mapper));
            _ = CreateMap<Tuple<List<DayPartLength>, int, int, int, string>, List<AgDayPartLength>>()
                .ConstructUsing((t, context) => LoadAgDayPartLengths(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, context.Mapper));

            _ = CreateMap<List<AgDayPart>, List<AgPart>>().ConstructUsing(src => LoadAgParts(src));

            _ = CreateMap<Tuple<List<Multipart>, int, int>, List<AgLength>>()
                .ConstructUsing((t, context) => LoadAgLengths(t.Item1, t.Item2, t.Item3, context.Mapper));

            _ = CreateMap<List<AgDayPartLengths>, List<AgPartLength>>()
                .ForMember(dest => dest.Capacity, opt => opt.MapFrom(
                    src => src.Count + src.SelectMany(x => x).Count()))
                .ConstructUsing(src => LoadAgPartLengths(src));

            _ = CreateMap<Tuple<Multipart, int, int>, List<AgMultiPart>>()
                .ConstructUsing(t => LoadAgMultiParts(t.Item1, t.Item2, t.Item3));
            _ = CreateMap<Tuple<List<Length>, int, int, DateTime, DateTime>, List<AgStrikeWeightLength>>()
                .ConstructUsing((t, context) => LoadAgStrikeWeightLengths(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, context.Mapper));
        }

        private static List<AgPart> LoadAgParts(IEnumerable<AgDayPart> agDayParts)
        {
            return agDayParts.Select(agDayPart => new AgPart
            {
                CampaignNo = agDayPart.CampaignNo,
                StartDate = agDayPart.StartDate,
                SalesAreaNo = agDayPart.SalesAreaNo,
                DayPartNo = agDayPart.DayPartNo,
                AgPartRequirement = agDayPart.AgDayPartRequirement
            }).ToList();
        }

        private static List<AgPartLength> LoadAgPartLengths(IEnumerable<AgDayPartLengths> agDayPartLengths)
        {
            return agDayPartLengths.SelectMany(x => x).Select(x => new AgPartLength
            {
                CampaignNo = x.CampaignNo,
                SalesAreaNo = x.SalesAreaNo,
                MultipartNumber = x.MultipartNumber,
                DayPartNo = x.DayPartNo,
                DayPartType = x.DayPartType,
                StartDate = x.StartDate,
                SpotLength = x.SpotLength,
                AgPartLengthRequirement = x.AgPartLengthRequirement
            }).ToList();
        }

        private static List<AgStrikeWeightLength> LoadAgStrikeWeightLengths(IEnumerable<Length> lengths, int campaignNo,
            int channelGroupNo, DateTime starDateTime, DateTime enDateTime, IMapper mapper)
        {
            return lengths.Select(l => new AgStrikeWeightLength
            {
                CampaignNo = campaignNo,
                SalesAreaNo = channelGroupNo,
                StartDate = starDateTime.ToString("yyyyMMdd"),
                EndDate = enDateTime.ToString("yyyyMMdd"),
                SpotLength = Convert.ToInt32(l.length.BclCompatibleTicks / NodaConstants.TicksPerSecond),
                MultiPartNo = l.MultipartNumber,
                AgStrikeWeightLengthRequirement =
                    mapper.Map<AgRequirement>(Tuple.Create(l.DesiredPercentageSplit,
                        l.CurrentPercentageSplit)),
            }).ToList();
        }

        private static List<AgStrikeWeight> LoadAgStrikeWeights(IEnumerable<StrikeWeight> strikeWeights, int campaignNo,
            int channelGroupNo, IMapper mapper)
        {
            return strikeWeights.Select(strikeWeight =>
            {
                var agStrikeWeightLengths = new AgStrikeWeightLengths();

                if (strikeWeight.Lengths?.Count > 0)
                {
                    var nonMultipartLengths = mapper.Map<List<AgStrikeWeightLength>>(Tuple.Create(
                        strikeWeight.Lengths.Where(x => x.MultipartNumber == 0).ToList(), campaignNo, channelGroupNo,
                        strikeWeight.StartDate, strikeWeight.EndDate));

                    var multipartLengths = mapper.Map<List<AgStrikeWeightLength>>(Tuple.Create(
                        strikeWeight.Lengths.Where(x => x.MultipartNumber > 0).ToList(), campaignNo, channelGroupNo,
                        strikeWeight.StartDate, strikeWeight.EndDate));

                    agStrikeWeightLengths.AddRange(nonMultipartLengths);
                    agStrikeWeightLengths.AddRange(MergeMultipartStrikeWeightLengths(multipartLengths, mapper));
                }

                return new AgStrikeWeight
                {
                    CampaignNo = campaignNo,
                    SalesAreaNo = channelGroupNo,
                    StartDate = strikeWeight.StartDate.ToString("yyyyMMdd"),
                    EndDate = strikeWeight.EndDate.ToString("yyyyMMdd"),
                    AgStikeWeightRequirement =
                        mapper.Map<AgRequirement>(Tuple.Create(strikeWeight.DesiredPercentageSplit,
                            strikeWeight.CurrentPercentageSplit)),
                    AgStrikeWeightLengths = agStrikeWeightLengths,
                    NbrAgStrikeWeightLengths = agStrikeWeightLengths.Count,
                    SpotMaxRatings = strikeWeight.SpotMaxRatings
                };
            }).ToList();
        }

        /// <summary>
        /// Merges the MultiPart strike weight lengths by MultiPartNo.
        /// </summary>
        /// <param name="originalItems">Original lengths.</param>
        /// <param name="mapper">Mapper.</param>
        /// <returns></returns>
        private static IEnumerable<AgStrikeWeightLength> MergeMultipartStrikeWeightLengths(IEnumerable<AgStrikeWeightLength> originalItems, IMapper mapper)
        {
            return originalItems.GroupBy(x => new
            {
                x.CampaignNo,
                x.SalesAreaNo,
                x.MultiPartNo,
                x.StartDate,
                x.EndDate
            }).Select(group =>
            {
                var strikeWeightLength = group.First();
                decimal desiredPercentageSplit = 0, currentPercentageSplit = 0;

                foreach (var item in group)
                {
                    desiredPercentageSplit += item.AgStrikeWeightLengthRequirement.Required;
                    currentPercentageSplit += item.AgStrikeWeightLengthRequirement.Supplied;
                }

                strikeWeightLength.SpotLength = 0;
                strikeWeightLength.AgStrikeWeightLengthRequirement = mapper.Map<AgRequirement>(Tuple.Create(desiredPercentageSplit, currentPercentageSplit));

                return strikeWeightLength;
            });
        }

        private static List<AgMultiPart> LoadAgMultiParts(Multipart multiPart, int campaignNo, int channelGroupNo)
        {
            return multiPart.Lengths.Select(l =>
            {
                var spotLength = l.Length.BclCompatibleTicks / NodaConstants.TicksPerSecond;
                return new AgMultiPart
                {
                    CampaignNo = campaignNo,
                    SalesAreaNo = channelGroupNo,
                    MultiPartNo = multiPart.MultipartNumber,
                    SeqNo = l.Sequencing,
                    SpotLength = Convert.ToInt32(spotLength),
                    BookingPosition = l.BookingPosition,
                    PriceFactor = Math.Round(Convert.ToDouble(spotLength / (double)30), 2)
                };
            }).ToList();
        }

        private static List<AgLength> LoadAgLengths(IEnumerable<Multipart> multiParts, int campaignNo,
            int channelGroupNo, IMapper mapper)
        {
            return multiParts.Select(m =>
            {
                var agMultiParts = new AgMultiParts();
                var agMultiPartList = mapper.Map<List<AgMultiPart>>(Tuple.Create(m, campaignNo, channelGroupNo));
                if (agMultiPartList != null && agMultiPartList.Count > 0)
                {
                    agMultiParts.AddRange(agMultiPartList);
                }

                var spotLength = m.Lengths.Sum(l => l.Length.BclCompatibleTicks) / NodaConstants.TicksPerSecond;
                return new AgLength
                {
                    CampaignNo = campaignNo,
                    SalesAreaNo = channelGroupNo,
                    MultiPartNo = m.MultipartNumber,
                    SpotLength = Convert.ToInt32(spotLength),
                    PriceFactor = Math.Round(Convert.ToDouble(spotLength / (double)30), 2),
                    AgLengthRequirement = mapper.Map<AgRequirement>(Tuple.Create(m.DesiredPercentageSplit,
                        m.CurrentPercentageSplit)),
                    NbrAgMultiParts = agMultiPartList?.Count ?? 0,
                    AgMultiParts = agMultiParts
                };
            }).ToList();
        }

        private static List<AgLength> LoadAgLengths(IEnumerable<Length> lengths, int campaignNo,
            int channelGroupNo, IMapper mapper)
        {
            return lengths.Select(l =>
            {
                var spotLength = Convert.ToInt32(l.length.BclCompatibleTicks / NodaConstants.TicksPerSecond);
                return new AgLength
                {
                    CampaignNo = campaignNo,
                    SalesAreaNo = channelGroupNo,
                    SpotLength = Convert.ToInt32(l.length.BclCompatibleTicks / NodaConstants.TicksPerSecond),
                    PriceFactor = spotLength / 30,
                    AgLengthRequirement = mapper.Map<AgRequirement>((l.DesiredPercentageSplit - l.CurrentPercentageSplit)),
                };
            }).ToList();
        }

        private static List<AgDayPartLength> LoadAgDayPartLengths(List<DayPartLength> dayPartLengths, int campaignNo, int channelGroupNo,
            int dayPartNo, string startDate, IMapper mapper)
        {
            var agDayPartLengths = new List<AgDayPartLength>();
            dayPartLengths.ForEach(length =>
            {
                agDayPartLengths.Add(new AgDayPartLength
                {
                    CampaignNo = campaignNo,
                    SalesAreaNo = channelGroupNo,
                    StartDate = startDate,
                    DayPartNo = dayPartNo,
                    SpotLength = length.Length.BclCompatibleTicks / NodaConstants.TicksPerSecond,
                    MultipartNumber = length.MultipartNumber,
                    AgPartLengthRequirement = mapper.Map<AgRequirement>(Tuple.Create(length.DesiredPercentageSplit, length.CurrentPercentageSplit))
                });
            });
            return agDayPartLengths;
        }
    }
}
