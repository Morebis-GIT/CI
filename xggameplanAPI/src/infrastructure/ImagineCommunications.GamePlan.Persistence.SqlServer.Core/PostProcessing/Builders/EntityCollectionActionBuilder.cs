using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.PostProcessing.Actions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.PostProcessing.Descriptors;
using xggameplan.common.ActionProcessing;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.PostProcessing.Builders
{
    public class EntityCollectionActionBuilder<TEntity> : IPostActionBuilder
        where TEntity : class
    {
        private readonly IEnumerable<TEntity> _entities;
        private EntityCollectionActionDescriptor _actionDescriptor;
        private readonly IMapper _mapper;

        protected EntityCollectionActionDescriptor ActionDescriptor =>
            _actionDescriptor ?? (_actionDescriptor = new EntityCollectionActionDescriptor());

        public EntityCollectionActionBuilder(IEnumerable<TEntity> entities, IMapper mapper)
        {
            _entities = entities;
            _mapper = mapper;
        }

        /// <summary>
        /// Updates each instance of the specified collection by the values of
        /// entities one by one.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instances">     </param>
        /// <param name="mappingOptions">
        /// mapping context options for custom mapping actions
        /// </param>
        public void MapToCollection<T>(IEnumerable<T> instances, Action<IMappingOperationOptions> mappingOptions = null) where T : class
        {
            if (_entities.Count() != (instances ?? throw new ArgumentNullException(nameof(instances))).Count())
            {
                throw new Exception("Collections of entities and instances for mapping are different");
            }

            ActionDescriptor.MapToObjects = instances;
            ActionDescriptor.MappingOptions = mappingOptions;
        }

        public IAction Build()
        {
            return _actionDescriptor == null ? null : new EntityCollectionAction<TEntity>(_entities, _actionDescriptor, _mapper);
        }
    }
}