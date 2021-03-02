using System;
using System.Collections.Generic;
using AutoMapper;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.PostProcessing.Actions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.PostProcessing.Descriptors;
using xggameplan.common.ActionProcessing;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.PostProcessing.Builders
{
    public class BulkInsertActionBuilder<TEntity> : IPostActionBuilder
        where TEntity : class
    {
        protected class InternalBulkInsertActionBuilder<TModel> : IPostActionBuilder
            where TModel : class
        {
            private readonly IList<TEntity> _entities;
            private readonly BulkInsertActionDescriptor<TModel> _actionDescriptor;
            private readonly IMapper _mapper;

            public InternalBulkInsertActionBuilder(IList<TEntity> entities, BulkInsertActionDescriptor<TModel> actionDescriptor, IMapper mapper)
            {
                _entities = entities;
                _actionDescriptor = actionDescriptor;
                _mapper = mapper;
            }

            public IAction Build()
            {
                return new BulkInsertAction<TEntity, TModel>(_entities, _actionDescriptor, _mapper);
            }
        }

        private readonly IList<TEntity> _entities;
        private IPostActionBuilder _internalBuilder;
        private readonly IMapper _mapper;

        public BulkInsertActionBuilder(IList<TEntity> entities, IMapper mapper)
        {
            _entities = entities;
            _mapper = mapper;
        }

        /// <summary>
        /// Updates the specified enumeration if the implementation of <see cref="collection"/> is <see cref="IList{TModel}"/>.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="collection"></param>
        public void TryToUpdate<TModel>(IEnumerable<TModel> collection, Action<IMappingOperationOptions> mappingOptions = null) where TModel : class
        {
            if (collection is IList<TModel> modelList)
            {
                UpdateList<TModel>(modelList, mappingOptions);
            }
        }

        /// <summary>
        /// Clears the specified list and populate it by updated models.
        /// The order of models might be different then the previous one due to bulk insert particularities.
        /// </summary>
        public void UpdateList<TModel>(IList<TModel> modelList, Action<IMappingOperationOptions> mappingOptions = null) where TModel : class
        {
            if ((modelList ?? throw new ArgumentNullException(nameof(modelList))).IsReadOnly)
            {
                return;
            }

            var actionDescriptor = new BulkInsertActionDescriptor<TModel>();
            actionDescriptor.UpdateList = modelList;
            actionDescriptor.MappingOptions = mappingOptions;
            _internalBuilder = new InternalBulkInsertActionBuilder<TModel>(_entities, actionDescriptor, _mapper);
        }

        public IAction Build()
        {
            return _internalBuilder?.Build();
        }
    }
}
