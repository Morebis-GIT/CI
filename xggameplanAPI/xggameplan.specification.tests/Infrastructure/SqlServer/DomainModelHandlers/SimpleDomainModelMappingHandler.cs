using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.Infrastructure.SqlServer.DomainModelHandlers
{
    public class SimpleDomainModelMappingHandler<TEntity, TModel> : IDomainModelHandler<TModel>
        where TEntity : class
        where TModel : class
    {
        private readonly ISqlServerTestDbContext _dbContext;
        protected readonly IMapper Mapper;

        protected virtual TEntity MapToEntity(TModel model) => Mapper.Map<TEntity>(model);

        protected virtual IEnumerable<TEntity> MapToEntity(IEnumerable<TModel> models) => Mapper.Map<IEnumerable<TEntity>>(models);

        protected virtual IEnumerable<TModel> MapToModel(IEnumerable<TEntity> entities) => Mapper.Map<IEnumerable<TModel>>(entities);

        public SimpleDomainModelMappingHandler(ISqlServerTestDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            Mapper = mapper;
        }

        public virtual TModel Add(TModel model) =>
            Mapper.Map<TModel>(_dbContext.Add(MapToEntity(model ?? throw new ArgumentNullException(nameof(model)))));

        public virtual void AddRange(params TModel[] models) => _dbContext.AddRange(MapToEntity(models).ToArray());

        public virtual int Count() => _dbContext.Query<TEntity>().Count();

        public virtual void DeleteAll() => _dbContext.RemoveRange(_dbContext.Query<TEntity>().ToArray());

        public virtual IEnumerable<TModel> GetAll() => MapToModel(_dbContext.Query<TEntity>());
    }
}
