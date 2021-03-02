using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;

namespace ImagineCommunications.GamePlan.Domain.Shared.Programmes
{
    public interface IProgrammeEpisodeRepository
    {
        IEnumerable<ProgrammeEpisode> GetAll();
    }
}
