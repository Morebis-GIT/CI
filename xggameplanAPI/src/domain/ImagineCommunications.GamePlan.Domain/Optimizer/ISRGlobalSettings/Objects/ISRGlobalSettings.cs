namespace ImagineCommunications.GamePlan.Domain.Optimizer.ISRGlobalSettings.Objects
{
    public class ISRGlobalSettings
    {
        public int Id { get; set; }
        public bool ExcludeSpotsBookedByProgrammeRequirements { get; set; }

        public ISRGlobalSettings FulfillFrom(ISRGlobalSettings settings)
        {
            ExcludeSpotsBookedByProgrammeRequirements = settings.ExcludeSpotsBookedByProgrammeRequirements;
            return this;
        }
    }
}
