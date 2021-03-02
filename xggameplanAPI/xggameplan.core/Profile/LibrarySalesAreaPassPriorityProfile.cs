using System;
using ImagineCommunications.GamePlan.Domain.LibrarySalesAreaPassPriorities;
using ImagineCommunications.GamePlan.Domain.Shared.System.Models;
using xggameplan.Extensions;
using xggameplan.Model;

namespace xggameplan.core.Profile
{
    public class LibrarySalesAreaPassPriorityProfile : AutoMapper.Profile
    {
        public LibrarySalesAreaPassPriorityProfile()
        {
            CreateMap<LibrarySalesAreaPassPriority, LibrarySalesAreaPassPriorityModel>()
                    .IncludeBase<EntityBase, BaseModel>();

            CreateMap<CreateLibrarySalesAreaPassPriorityModel, LibrarySalesAreaPassPriority>()
                 .IncludeBase<LibrarySalesAreaPassPriorityModelBase, LibrarySalesAreaPassPriority>();

            CreateMap<EntityBase, BaseModel>()
                    .IncludeBase<AuditBase, AuditBaseModel>()
                    .ForSourceMember(s => s.Id, o => o.DoNotValidate());

            CreateMap<AuditBase, AuditBaseModel>();

            CreateMap<SalesAreaPriority, SalesAreaPriorityModel>()
                    .ForMember(d => d.Priority, o => o.MapFrom(s => Enum.IsDefined(typeof(SalesAreaPriorityType), s.Priority) ?
                                                                    s.Priority : SalesAreaPriorityType.Exclude))
                    .ReverseMap()
                    .ForMember(d => d.SalesArea, o => o.MapFrom(s => !string.IsNullOrWhiteSpace(s.SalesArea) ? s.SalesArea.Trim() : null))
                    .ForMember(d => d.Priority, o => o.MapFrom(s => Enum.IsDefined(typeof(SalesAreaPriorityType), s.Priority) ?
                                                                    s.Priority : SalesAreaPriorityType.Exclude));

            CreateMap<UpdateLibrarySalesAreaPassPriorityModel, LibrarySalesAreaPassPriority>()
                    .IncludeBase<LibrarySalesAreaPassPriorityModelBase, LibrarySalesAreaPassPriority>();

            CreateMap<LibrarySalesAreaPassPriorityModelBase, LibrarySalesAreaPassPriority>()
                    .ForMember(d => d.Name, o => o.MapFrom(s => !string.IsNullOrWhiteSpace(s.Name) ? s.Name.ReduceExcessSpace() : null))
                    .ForMember(d => d.EndTime, o => o.MapFrom(s => s.EndTime.ToTime()))
                    .ForMember(d => d.StartTime, o => o.MapFrom(s => s.StartTime.ToTime()));
        }
    }
}
