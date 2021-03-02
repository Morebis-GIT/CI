using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ImagineCommunications.GamePlan.Domain.Generic.Repository
{
    public interface IRepository<T>
    {
        [Obsolete("Use Count() to match Linq conventions")]
        int CountAll { get; }

        [Obsolete("Use Get()")]
        T Find(Guid uid);

        [Obsolete("Use Delete()")]
        void Remove(Guid uid);

        void Add(T item);

        [Obsolete("Should be called AddRange() to match .NET conventions")]
        void Add(IEnumerable<T> item);

        IEnumerable<T> GetAll();

        // NOTE: Need to add a version with zero parameters to this interface
        int Count(Expression<Func<T, bool>> query);

        IEnumerable<T> FindByExternal(string externalRef);

        IEnumerable<T> FindByExternal(List<string> externalRefs);

        [Obsolete("Try to use TruncateAsync() as it is more reliable.")]
        void Truncate();
    }
}
