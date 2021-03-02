using ImagineCommunications.GamePlan.Domain.Generic.Repository;

namespace ImagineCommunications.GamePlan.Persistence.File.Repositories
{
    /// <summary>
    /// Factory for file tenant repositories
    /// </summary>
    public class FileRepositoryFactory : IRepositoryFactory
    {
        // Root folder, each repo has a separate folder
        private readonly string _rootFolder;

        public FileRepositoryFactory(string rootFolder) => _rootFolder = rootFolder;

        public IRepositoryScope BeginRepositoryScope() => new FileRepositoryScope(_rootFolder);
    }
}
