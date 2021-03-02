namespace ImagineCommunications.Extensions.DependencyInjection
{
    public interface IIndex<T> where T : class
    {
        T Resolve(string key);
    }
}
