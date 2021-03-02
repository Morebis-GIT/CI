using System;
using ImagineCommunications.GamePlan.Domain.Generic.Types;

namespace ImagineCommunications.GamePlan.Domain.Generic.Repository
{
    public interface IRepositoryScope : IRepositoryScopeCreator, IDisposable
    {
        /// <summary>
        /// Creates multiple repositories of the specified types.
        /// </summary>
        RepositoryDictionary CreateRepositories(params Type[] repositoryTypes);

        /// <summary>
        /// Creates an instance of a repository of type <typeparamref name="TRepository"/>.
        /// </summary>
        /// <typeparam name="TRepository">The type of repository to create.</typeparam>
        /// <returns>Returns an instances of the repository type.</returns>
        TRepository CreateRepository<TRepository>() where TRepository : class;
    }
}
