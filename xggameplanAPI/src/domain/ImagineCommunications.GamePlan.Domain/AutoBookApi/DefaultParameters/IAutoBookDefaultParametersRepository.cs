namespace ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters
{
    public interface IAutoBookDefaultParametersRepository
    {
        IAutoBookDefaultParameters Get();

        void AddOrUpdate(IAutoBookDefaultParameters autoBookDefaultParameters);

        void SaveChanges();
    }
}
