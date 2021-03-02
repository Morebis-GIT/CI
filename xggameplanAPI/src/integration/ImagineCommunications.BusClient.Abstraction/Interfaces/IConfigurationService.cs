namespace ImagineCommunications.BusClient.Abstraction.Interfaces
{
    public interface IConfigurationService
    {
        T GetData<T>(string jsonString);
    }
}
