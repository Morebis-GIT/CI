using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext
{
    public interface IDomainModelHandler<TModel>
        where TModel : class
    {
        TModel Add(TModel model);

        void AddRange(params TModel[] models);

        int Count();

        void DeleteAll();

        IEnumerable<TModel> GetAll();
    }
}
