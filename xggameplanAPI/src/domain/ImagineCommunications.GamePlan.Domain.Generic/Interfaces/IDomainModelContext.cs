using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.Generic.Interfaces
{
    public interface IDomainModelContext
    {
        TModel Add<TModel>(TModel model) where TModel : class;

        object Add(object model);

        void AddRange<TModel>(params TModel[] models) where TModel : class;

        void AddRange(params object[] models);

        void AddRange(IEnumerable<object> models);

        int Count<TModel>() where TModel : class;

        void DeleteAll<TModel>() where TModel : class;

        IEnumerable<TModel> GetAll<TModel>() where TModel : class;
    }
}
