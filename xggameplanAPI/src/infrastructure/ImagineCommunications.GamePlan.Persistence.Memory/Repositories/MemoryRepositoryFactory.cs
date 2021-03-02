using System;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;

namespace ImagineCommunications.GamePlan.Persistence.Memory.Repositories
{
    /// <summary>
    /// Factory for memory repositories
    /// </summary>
    public class MemoryRepositoryFactory : IRepositoryFactory
    {
        private readonly Guid _repositorySegmentationSalt;
        private readonly double _objectTTL;

        public MemoryRepositoryFactory(Guid repositorySegmentationSalt) =>
            _repositorySegmentationSalt = repositorySegmentationSalt;

        public MemoryRepositoryFactory(Guid repositorySegmentationSalt, double objectTTL) =>
            (_repositorySegmentationSalt, _objectTTL) = (repositorySegmentationSalt, objectTTL);

        public IRepositoryScope BeginRepositoryScope() =>
            new MemoryRepositoryScope(_repositorySegmentationSalt, _objectTTL);
    }
}
