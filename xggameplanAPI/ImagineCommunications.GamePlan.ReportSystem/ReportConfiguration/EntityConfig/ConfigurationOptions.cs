namespace ImagineCommunications.GamePlan.ReportSystem.ReportConfiguration.EntityConfig
{
    public class ConfigurationOptions: IConfigurationOptions
    {
        public bool IsHideHeader { get; set; }
        public bool UseHeaderHumanizer { get; set; } = true;
    }
}
