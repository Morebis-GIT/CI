namespace ImagineCommunications.GamePlan.Domain.EfficiencySettings
{
    public interface IEfficiencySettingsRepository
    {
        EfficiencySettings GetDefault();

        EfficiencySettings UpdateDefault(EfficiencySettings settings);
    }
}
