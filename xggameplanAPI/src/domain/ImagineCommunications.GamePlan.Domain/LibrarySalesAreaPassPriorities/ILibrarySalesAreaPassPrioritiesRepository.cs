using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ImagineCommunications.GamePlan.Domain.LibrarySalesAreaPassPriorities
{
    public interface ILibrarySalesAreaPassPrioritiesRepository
    {
        Task AddAsync(LibrarySalesAreaPassPriority entity);

        Task UpdateAsync(LibrarySalesAreaPassPriority entity);

        Task SaveChanges();

        Task<IEnumerable<LibrarySalesAreaPassPriority>> GetAllAsync();

        Task<LibrarySalesAreaPassPriority> GetAsync(Guid uId);

        Task<bool> ExistsAsync(Expression<Func<LibrarySalesAreaPassPriority, bool>> condition);

        Task<bool> IsNameUniqueForCreateAsync(string name);

        Task Delete(Guid id);

        Task<bool> IsNameUniqueForUpdateAsync(string name, Guid uId);
    }
}
