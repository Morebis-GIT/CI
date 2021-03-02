namespace ImagineCommunications.BusClient.Abstraction.Interfaces
{
    public interface IContractValidatorService
    {
        void Validate<T>(T contract);
    }
}
