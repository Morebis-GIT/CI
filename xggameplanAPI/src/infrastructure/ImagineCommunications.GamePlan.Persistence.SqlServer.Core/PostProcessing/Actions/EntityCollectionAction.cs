using System;
using System.Collections.Generic;
using AutoMapper;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.PostProcessing.Descriptors;
using xggameplan.common.ActionProcessing;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.PostProcessing.Actions
{
    public class EntityCollectionAction<TEntity> : IAction
        where TEntity : class
    {
        private readonly IEnumerable<TEntity> _entities;
        private readonly EntityCollectionActionDescriptor _actionDescriptor;
        private readonly IMapper _mapper;

        public EntityCollectionAction(IEnumerable<TEntity> entities, EntityCollectionActionDescriptor actionDescriptor, IMapper mapper)
        {
            _entities = entities ?? throw new ArgumentNullException(nameof(entities));
            _actionDescriptor = actionDescriptor ?? throw new ArgumentNullException(nameof(actionDescriptor));
            _mapper = mapper;
        }

        public void Execute()
        {
            using (var entitiesEnumerator = _entities.GetEnumerator())
            using (var objectsEnumerator = _actionDescriptor.MapToObjects.GetEnumerator())
            {
                while (entitiesEnumerator.MoveNext() && objectsEnumerator.MoveNext())
                {
                    _ = _mapper.Map(entitiesEnumerator.Current, objectsEnumerator.Current,
                        opts => _actionDescriptor.MappingOptions?.Invoke(opts));
                }
            }
        }
    }
}
