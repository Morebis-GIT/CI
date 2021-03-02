namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Shared
{
    public class ProgrammeEpisode
    {
        public ProgrammeEpisode(string name, int number)
        {
            Name = name;
            Number = number;
        }

        public string Name { get; }
        public int Number { get; }
    }
}
