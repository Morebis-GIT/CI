using System;
using System.Collections.Generic;
using AutoMapper;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.PostProcessing.Descriptors
{
    public class EntityCollectionActionDescriptor
    {
        public IEnumerable<object> MapToObjects { get; set; }
        public Action<IMappingOperationOptions> MappingOptions { get; set; }
    }
}
