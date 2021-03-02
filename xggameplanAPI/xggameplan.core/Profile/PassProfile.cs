using System;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Domain.Passes.Queries;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using xggameplan.Model;

namespace xggameplan.Profile
{
    internal class PassProfile : AutoMapper.Profile
    {
        public PassProfile()
        {
            CreateMap<PassSalesAreaPriority, PassSalesAreaPriorityModel>().ReverseMap();

            CreateMap<CreatePassModel, Pass>();

            CreateMap<Pass, PassModel>().ReverseMap();
            
            CreateMap<PassModel, PassReference>().ReverseMap();
            CreateMap<PassPriority, CreatePassPriorityModel>().ReverseMap();
            CreateMap<PassPriority, PassPriorityModel>().ReverseMap();

            CreateMap<PassDigestListItem, PassDigestListItemModel>();
        }

        private static PassSalesAreaPriority ConvertToPassSalesAreaPriority(PassSalesAreaPriority source)
        {
            if (source == null)
            {
                return source;
            }
            DateTime? nullableDateTime = null;
            source.StartDate = source.StartDate.HasValue ? source.StartDate.Value.Date : nullableDateTime;
            source.EndDate = source.EndDate.HasValue ? source.EndDate.Value.Date : nullableDateTime;
            return source;
        }
    }
}
