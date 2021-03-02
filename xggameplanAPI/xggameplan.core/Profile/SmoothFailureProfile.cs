using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.SmoothFailureMessages;
using ImagineCommunications.GamePlan.Domain.SmoothFailures;
using xggameplan.Model;

namespace xggameplan.Profile
{
    public class SmoothFailureProfile : AutoMapper.Profile
    {
        public SmoothFailureProfile()
        {
            CreateMap<SmoothFailure, SmoothFailureModel>();
            CreateMap<SmoothFailureModel, SmoothFailure>();
            CreateMap<SmoothFailureModel, SmoothFailure>();

            CreateMap<Tuple<List<SmoothFailureModel>, IEnumerable<SmoothFailureMessage>>, List<SmoothFailureExtendedModel>>()
                    .ConvertUsing((tuple, list, context) => MapToSmoothFailureExtendedModels(tuple.Item1, tuple.Item2, context.Mapper));


            CreateMap<Tuple<SmoothFailureModel, IEnumerable<SmoothFailureMessage>>, SmoothFailureExtendedModel>()
                .ForMember(d => d.BreakDateTime, m => m.MapFrom(src => ConvertToDateTimeFormat(src.Item1.BreakDateTime)))
                .ForMember(d => d.BreakDate, m => m.MapFrom(src => ConvertToShortDate(src.Item1.BreakDateTime)))
                .ForMember(d => d.BreakTime, m => m.MapFrom(src => ConvertToTimeFormat(src.Item1.BreakDateTime)))
                .ForMember(d => d.RestrictionStartDate, m => m.MapFrom(src => ConvertToShortDate(src.Item1.RestrictionStartDate)))
                .ForMember(d => d.RestrictionStartTime, m => m.MapFrom(src => ConvertToTimeFormat(src.Item1.RestrictionStartTime)))
                .ForMember(d => d.RestrictionEndDate, m => m.MapFrom(src => ConvertToShortDate(src.Item1.RestrictionEndDate)))
                .ForMember(d => d.RestrictionEndTime, m => m.MapFrom(src => ConvertToTimeFormat(src.Item1.RestrictionEndTime)))
                .ForMember(d => d.ExternalSpotReference, m => m.MapFrom(src => TryConvertToInt(src.Item1.ExternalSpotRef)))
                .ForMember(d => d.SmoothFailureMessages, m => m.MapFrom(src => GetFailureMessages(src.Item1.MessageIds, src.Item2.ToList())))
                .ForMember(d => d.SalesArea, m => m.MapFrom(src => src.Item1.SalesArea))
                .ForMember(d => d.ExternalBreakRef, m => m.MapFrom(src => src.Item1.ExternalBreakRef))
                .ForMember(d => d.SpotLength, m => m.MapFrom(src => src.Item1.SpotLength))
                .ForMember(d => d.ExternalCampaignRef, m => m.MapFrom(src => src.Item1.ExternalCampaignRef))
                .ForMember(d => d.CampaignName, m => m.MapFrom(src => src.Item1.CampaignName))
                .ForMember(d => d.CampaignGroup, m => m.MapFrom(src => src.Item1.CampaignGroup))
                .ForMember(d => d.AdvertiserName, m => m.MapFrom(src => src.Item1.AdvertiserName))
                .ForMember(d => d.ProductName, m => m.MapFrom(src => src.Item1.ProductName))
                .ForMember(d => d.ClashDescription, m => m.MapFrom(src => src.Item1.ClashDescription))
                .ForMember(d => d.IndustryCode, m => m.MapFrom(src => src.Item1.IndustryCode))
                .ForMember(d => d.ClearanceCode, m => m.MapFrom(src => src.Item1.ClearanceCode))
                .ForMember(d => d.RestrictionDays, m => m.MapFrom(src => src.Item1.RestrictionDays));
        }

        private List<SmoothFailureExtendedModel> MapToSmoothFailureExtendedModels(
            List<SmoothFailureModel> smoothFailureModels, IEnumerable<SmoothFailureMessage> failureMessages, IMapper mapper)
        {
            var result = new List<SmoothFailureExtendedModel>();
            if (smoothFailureModels == null)
            {
                return result;
            }
            smoothFailureModels.ForEach(model => result.Add(mapper.Map<SmoothFailureExtendedModel>(Tuple.Create(model, failureMessages))));
            return result;
        }

        private object TryConvertToInt(string value)
        {
            int k;
            if (Int32.TryParse(value, out k))
            {
                return k;
            }
            return value;
        }

        private string GetFailureMessages(IList<int> messageIds, IList<SmoothFailureMessage> failureMessages)
        {
            const string language = "ENG";
            if (messageIds != null && messageIds.Count != 0 && failureMessages != null && failureMessages.Any())
            {
                return string.Join(", ", failureMessages.Where(m => messageIds.Contains(m.Id)).
                    Select(m => m.Description!=null && m.Description.ContainsKey(language) ? m.Description[language] : m.Id.ToString()));
            }
            return string.Empty;
        }
        private string ConvertToDateTimeFormat(DateTime dateTime) => dateTime.ToString("dd/MM/yyyy HH:mm:ss");
        private string ConvertToShortDate(DateTime dateTime) => dateTime.ToString("dd/MM/yyyy");
        private string ConvertToShortDate(DateTime? dateTime) => dateTime.HasValue ? ConvertToShortDate(dateTime.Value) : string.Empty;
        private string ConvertToTimeFormat(DateTime dateTime) => dateTime.ToString("HH:mm:ss");
        private string ConvertToTimeFormat(TimeSpan? timeSpan) => timeSpan.HasValue ? timeSpan.Value.ToString("c") : string.Empty;
    }
}
