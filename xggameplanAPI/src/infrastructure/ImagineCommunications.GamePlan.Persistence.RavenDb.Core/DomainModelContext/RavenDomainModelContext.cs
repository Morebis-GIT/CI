using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Generic.Interfaces;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Core.DomainModelContext
{
    public class RavenDomainModelContext : IDomainModelContext
    {
        private readonly IRavenDbContext _dbContext;

        public RavenDomainModelContext(IRavenDbContext dbContext) =>
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

        public virtual TModel Add<TModel>(TModel model) where TModel : class =>
            _dbContext.Add(model);

        public virtual object Add(object model) =>
            _dbContext.Add(model);

        public virtual void AddRange<TModel>(params TModel[] models) where TModel : class =>
            _dbContext.AddRange(models);

        public virtual void AddRange(params object[] models) =>
            AddRange(models.AsEnumerable());

        public virtual void AddRange(IEnumerable<object> models) =>
            _dbContext.AddRange(models);

        public virtual int Count<TModel>() where TModel : class =>
            _dbContext.Query<TModel>().Count();

        public virtual void DeleteAll<TModel>() where TModel : class =>
            _dbContext.Truncate<TModel>();

        public virtual IEnumerable<TModel> GetAll<TModel>() where TModel : class =>
            _dbContext.Specific.GetAll<TModel>();
    }
}
