namespace ImagineCommunications.GamePlan.Domain.Optimizer.ISRGlobalSettings
{
    public interface IISRGlobalSettingsRepository
    {
        Objects.ISRGlobalSettings Get();
        Objects.ISRGlobalSettings Update(Objects.ISRGlobalSettings settings);
    }
}
