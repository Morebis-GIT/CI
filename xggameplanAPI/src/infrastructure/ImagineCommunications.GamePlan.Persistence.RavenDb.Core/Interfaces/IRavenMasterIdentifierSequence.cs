﻿using ImagineCommunications.GamePlan.Domain.Generic.DbSequence;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Core.Interfaces
{
    public interface IRavenMasterIdentifierSequence : IRavenIdentifierSequence, IMasterIdentifierSequence
    {
    }
}
