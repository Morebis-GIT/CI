using System;
using System.Collections.Generic;
using AutoMapper;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.PostProcessing.Descriptors;
using xggameplan.common.ActionProcessing;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.PostProcessing.Actions
{
    public class BulkInsertAction<TEntity, TModel> : IAction
        where TEntity : class
        where TModel : class
    {
        private readonly IEnumerable<TEntity> _entities;
        private readonly BulkInsertActionDescriptor<TModel> _actionDescriptor;
        private readonly IMapper _mapper;

        public BulkInsertAction(IList<TEntity> entities, BulkInsertActionDescriptor<TModel> actionDescriptor, IMapper mapper)
        {
            _entities = entities ?? throw new ArgumentNullException(nameof(entities));
            _actionDescriptor = actionDescriptor ?? throw new ArgumentNullException(nameof(actionDescriptor));
            _mapper = mapper;
        }

        public void Execute()
        {
            _actionDescriptor.UpdateList.Clear();
            foreach (var item in _mapper.Map<IEnumerable<TModel>>(_entities, opts => _actionDescriptor.MappingOptions?.Invoke(opts)))
            {
                _actionDescriptor.UpdateList.Add(item);
            }
        }
    }
}
