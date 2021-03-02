namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext
{
    public interface IDomainModelHandlerResolver
    {
        IDomainModelHandler<TModel> Resolve<TModel>() where TModel : class;
    }
}
