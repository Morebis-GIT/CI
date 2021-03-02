using AutoMapper;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class AutoBookProfile : Profile
    {
        private const string RavenCollectionName = "AutoBooks/";

        public AutoBookProfile()
        {
            CreateMap<AutoBook, Domain.AutoBookApi.Storage.Objects.AutoBook>()
                .ForMember(x => x.Id, opt => opt.MapFrom(e => $"{RavenCollectionName}{e.AutoBookId}"))
                .ForMember(x => x.Status,
                    opt => opt.MapFrom(e => (Domain.AutoBookApi.Storage.AutoBookStatuses)e.Status))
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(e => e.AutoBookId,
                    opt => opt.MapFrom(src => AutoBookCollectionIdToEntityAutoBookId(src.Id)));

            CreateMap<AutoBookTask, Domain.AutoBookApi.Storage.Objects.AutoBookTask>()
                .ReverseMap();

            CreateMap<AutoBookSettings, Domain.AutoBookApi.Settings.AutoBookSettings>()
                .ReverseMap();

            CreateMap<AutoBookInstanceConfiguration,
                    Domain.AutoBookApi.InstanceConfiguration.Objects.AutoBookInstanceConfiguration>()
                .ForMember(x => x.CloudProvider, opt =>
                    opt.MapFrom(e => (Domain.AutoBookApi.InstanceConfiguration.CloudProviders)e.CloudProvider))
                .ReverseMap();

            CreateMap<AutoBookInstanceConfigurationCriteria,
                    Domain.AutoBookApi.InstanceConfiguration.Objects.AutoBookInstanceConfigurationCriteria>()
                .ReverseMap();
        }

        internal static string AutoBookCollectionIdToEntityAutoBookId(string id)
        {
            return id.Replace(RavenCollectionName, string.Empty);
        }
    }
}
