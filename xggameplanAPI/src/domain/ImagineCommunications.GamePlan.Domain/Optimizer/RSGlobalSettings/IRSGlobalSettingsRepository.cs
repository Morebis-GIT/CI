namespace ImagineCommunications.GamePlan.Domain.Optimizer.RSGlobalSettings
{
    public interface IRSGlobalSettingsRepository
    {
        Objects.RSGlobalSettings Get();
        Objects.RSGlobalSettings Update(Objects.RSGlobalSettings settings);
    }
}
