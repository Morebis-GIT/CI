namespace ImagineCommunications.GamePlan.Domain.AutoBookApi.Settings
{
    public interface IAutoBookSettingsRepository
    {
#pragma warning disable CA1716 // Identifiers should not match keywords
        AutoBookSettings Get();
#pragma warning restore CA1716 // Identifiers should not match keywords

        void AddOrUpdate(AutoBookSettings autoBookSettings);

        void SaveChanges();
    }
}
