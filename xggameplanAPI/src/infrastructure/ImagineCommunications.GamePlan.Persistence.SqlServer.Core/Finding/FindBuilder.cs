using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Finding
{
    public class FindBuilder<TEntity>
        where TEntity : class
    {
        private readonly EntityEntry<TEntity> _entry;

        public FindBuilder(EntityEntry<TEntity> entry)
        {
            _entry = entry ?? throw new ArgumentNullException(nameof(entry));
        }

        public FindBuilder<TEntity> IncludeCollection<TProperty>(Expression<Func<TEntity, IEnumerable<TProperty>>> propertyExpression)
            where TProperty : class
        {
            _entry.Collection(propertyExpression).Load();
            return this;
        }

        public FindBuilder<TEntity> IncludeReference<TProperty>(Expression<Func<TEntity, TProperty>> propertyExpression)
            where TProperty : class
        {
            _entry.Reference(propertyExpression).Load();
            return this;
        }
    }
}
