namespace ImagineCommunications.GamePlan.Domain.Generic.Repository
{
    public interface IRepositoryScopeCreator
    {
        /// <summary>
        /// Creates a new scope of repositories based on the new connection.
        /// </summary>
        /// <returns></returns>
        IRepositoryScope BeginRepositoryScope();
    }
}
