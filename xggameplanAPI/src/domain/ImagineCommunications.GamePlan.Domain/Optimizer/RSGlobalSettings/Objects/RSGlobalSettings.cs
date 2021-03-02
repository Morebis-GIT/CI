namespace ImagineCommunications.GamePlan.Domain.Optimizer.RSGlobalSettings.Objects
{
    public class RSGlobalSettings
    {
        public int Id { get; set; }
        public bool ExcludeSpotsBookedByProgrammeRequirements { get; set; }

        public RSGlobalSettings FulfillFrom(RSGlobalSettings settings)
        {
            ExcludeSpotsBookedByProgrammeRequirements = settings.ExcludeSpotsBookedByProgrammeRequirements;
            return this;
        }
    }
}
