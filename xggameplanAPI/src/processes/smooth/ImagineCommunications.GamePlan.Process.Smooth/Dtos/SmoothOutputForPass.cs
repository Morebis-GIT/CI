namespace ImagineCommunications.GamePlan.Process.Smooth.Dtos
{
    public class SmoothOutputForPass
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SmoothOutputForPass"/>
        /// class. Initialises the <see cref="CountSpotsSet"/> property to zero.
        /// </summary>
        /// <param name="passSequence">The pass sequence.</param>
        public SmoothOutputForPass(
            int passSequence
            )
        {
            PassSequence = passSequence;
            CountSpotsSet = 0;
        }

        public int PassSequence { get; set; }

        public int CountSpotsSet { get; set; }
    }
}
