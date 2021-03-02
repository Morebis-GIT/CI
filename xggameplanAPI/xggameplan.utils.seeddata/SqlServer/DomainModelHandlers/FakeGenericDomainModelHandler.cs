using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class FakeGenericDomainModelHandler<TModel> : IDomainModelHandler<TModel>
        where TModel : class
    {
        public TModel Add(TModel model) => model;

        public void AddRange(params TModel[] models)
        {
        }

        public int Count() => 0;

        public void DeleteAll()
        {
        }

        public IEnumerable<TModel> GetAll() => Enumerable.Empty<TModel>();
    }
}
