using System;
using System.Collections.Generic;
using AutoMapper;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.PostProcessing.Descriptors
{
    public class BulkInsertActionDescriptor<TModel>
        where TModel : class
    {
        public IList<TModel> UpdateList { get; set; }

        public Action<IMappingOperationOptions> MappingOptions { get; set; }
    }
}
