namespace xggameplan.core.Hubs
{
    public interface IClientContract<in TModel> where TModel : class
    {
        void Notify(TModel notification);
    }
}
