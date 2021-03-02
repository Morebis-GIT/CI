using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.ProgrammeCategory;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.ProgrammeCategory
{
    public class ProgrammeCategoryCreated : IProgrammeCategoryCreated
    {
        public string Name { get; }
        public string ExternalRef { get; }
        public string ParentExternalRef { get; }

        public ProgrammeCategoryCreated(string name, string externalRef, string parentExternalRef)
        {
            Name = name;
            ExternalRef = externalRef;
            ParentExternalRef = parentExternalRef;
        }
    }
}
