using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Maintenance.TaskInstances;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class TaskInstanceProfile : Profile
    {
        public TaskInstanceProfile() => CreateMap<TaskInstance, Entities.Master.TaskInstance>()
                .ForMember(x => x.Parameters, opt =>
                     opt.MapFrom(src =>
                         src.Parameters.Select(c => new Entities.Master.TaskInstanceParameter()
                         { Name = c.Key, Value = c.Value.ToString(), TaskInstanceId = src.Id })))
                .ForMember(x => x.Status, opt => opt.MapFrom(src => (int)src.Status))
                .ReverseMap()
                .ForMember(x => x.Parameters, opt =>
                     opt.MapFrom(src =>
                         src.Parameters.ToDictionary(k => k.Name, v => v.Value)))
                .ForMember(x => x.Status, opt => opt.MapFrom(src => (TaskInstanceStatues)src.Status));
    }
}
