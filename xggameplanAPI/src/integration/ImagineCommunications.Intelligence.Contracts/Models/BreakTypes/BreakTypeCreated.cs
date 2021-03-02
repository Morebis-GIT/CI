using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.BreakTypes;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.BreakTypes
{
    public class BreakTypeCreated : IBreakTypeCreated
    {
        public string Name { get; set; }

        public BreakTypeCreated(string name)
        {
            Name = name;
        }
    }
}
