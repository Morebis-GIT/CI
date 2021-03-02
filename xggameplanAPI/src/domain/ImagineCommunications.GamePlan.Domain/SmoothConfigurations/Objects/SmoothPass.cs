namespace ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects
{
    public abstract class SmoothPass
    {
        /// <summary>
        /// Sequence for processing in correct order
        /// </summary>
        public int Sequence { get; set; }

        public SmoothPass()
        {

        }
    }
}
