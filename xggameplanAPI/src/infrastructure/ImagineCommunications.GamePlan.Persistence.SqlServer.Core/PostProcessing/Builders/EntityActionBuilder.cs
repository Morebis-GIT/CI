using System;
using AutoMapper;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.PostProcessing.Actions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.PostProcessing.Descriptors;
using xggameplan.common.ActionProcessing;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.PostProcessing.Builders
{
    public class EntityActionBuilder<TEntity> : IPostActionBuilder
        where TEntity : class
    {
        private readonly TEntity _entity;
        private EntityActionDescriptor _actionDescriptor;
        private readonly IMapper _mapper;

        protected EntityActionDescriptor ActionDescriptor =>
            _actionDescriptor ?? (_actionDescriptor = new EntityActionDescriptor());

        public EntityActionBuilder(TEntity entity, IMapper mapper)
        {
            _entity = entity;
            _mapper = mapper;
        }

        /// <summary>
        /// Updates the specified instance by entity values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        public void MapTo<T>(T instance, Action<IMappingOperationOptions> mappingOptions = null) where T : class
        {
            ActionDescriptor.MapToObject = instance ?? throw new ArgumentNullException(nameof(instance));
            ActionDescriptor.MappingOptions = mappingOptions;
        }

        public IAction Build()
        {
            return _actionDescriptor == null ? null : new EntityAction<TEntity>(_entity, _actionDescriptor, _mapper);
        }
    }
}
