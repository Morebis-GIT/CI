using System.Collections;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Interfaces;

namespace ImagineCommunications.GamePlan.Infrastructure.Json.Core
{
    public abstract class JsonDomainModelContextImporter<TSource, TDeserializedType> : JsonImporter<TSource, TDeserializedType>
        where TDeserializedType : IEnumerable
    {
        private readonly IDomainModelContext _domainModelDbContext;

        protected JsonDomainModelContextImporter(IDomainModelContext domainModelDbContext)
        {
            _domainModelDbContext = domainModelDbContext;
        }

        protected override void StoreDeserializedData(IEnumerable<object> data)
        {
            _domainModelDbContext.AddRange(data);
        }
    }
}
