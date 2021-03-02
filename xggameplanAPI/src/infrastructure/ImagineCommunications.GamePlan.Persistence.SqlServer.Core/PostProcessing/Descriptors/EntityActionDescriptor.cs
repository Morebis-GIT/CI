using System;
using AutoMapper;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.PostProcessing.Descriptors
{
    public class EntityActionDescriptor
    {
        public object MapToObject { get; set; }
        public Action<IMappingOperationOptions> MappingOptions { get; set; }
    }
}
