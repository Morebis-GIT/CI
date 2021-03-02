using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.ProgrammeClassification;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.ProgrammeClassification
{
    public class ProgrammeClassificationCreated : IProgrammeClassificationCreated
    {
        public ProgrammeClassificationCreated(int uid, string code, string description)
        {
            Uid = uid;
            Code = code;
            Description = description;
        }

        public int Uid { get; }

        public string Code { get; }

        public string Description { get; }
    }
}
