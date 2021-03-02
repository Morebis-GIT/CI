using System;
using AutoMapper;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.PostProcessing.Descriptors;
using xggameplan.common.ActionProcessing;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.PostProcessing.Actions
{
    public class EntityAction<TEntity> : IAction
        where TEntity : class
    {
        private readonly TEntity _entity;
        private readonly EntityActionDescriptor _actionDescriptor;
        private readonly IMapper _mapper;
        private readonly Action<IMappingOperationOptions> _mappingOptions;

        public EntityAction(TEntity entity, EntityActionDescriptor actionDescriptor, IMapper mapper)
        {
            _entity = entity ?? throw new ArgumentNullException(nameof(entity));
            _actionDescriptor = actionDescriptor ?? throw new ArgumentNullException(nameof(actionDescriptor));
            _mapper = mapper;
        }

        public void Execute()
        {
            _ = _mapper.Map(_entity, _actionDescriptor.MapToObject,
                opts => _actionDescriptor.MappingOptions?.Invoke(opts));
        }
    }
}
