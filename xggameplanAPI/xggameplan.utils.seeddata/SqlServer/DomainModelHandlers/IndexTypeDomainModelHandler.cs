using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.IndexTypes;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class IndexTypeDomainModelHandler : IDomainModelHandler<IndexType>
    {
        private readonly IIndexTypeRepository _indexTypeRepository;

        public IndexTypeDomainModelHandler(IIndexTypeRepository indexTypeRepository)
        {
            _indexTypeRepository =
                indexTypeRepository ?? throw new ArgumentNullException(nameof(indexTypeRepository));
        }

        public IndexType Add(IndexType model)
        {
            _indexTypeRepository.Add(new[] {model});
            return model;
        }

        public void AddRange(params IndexType[] models) => _indexTypeRepository.Add(models);

        public int Count() => _indexTypeRepository.CountAll;

        public void DeleteAll() => _indexTypeRepository.Truncate();

        public IEnumerable<IndexType> GetAll() => _indexTypeRepository.GetAll();
    }
}
