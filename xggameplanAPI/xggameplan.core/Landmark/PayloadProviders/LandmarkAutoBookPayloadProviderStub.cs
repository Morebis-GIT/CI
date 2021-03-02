using System;
using System.Collections.Generic;
using xggameplan.core.Landmark.Abstractions;
using xggameplan.Model;

namespace xggameplan.core.Landmark.PayloadProviders
{
    public class LandmarkAutoBookPayloadProviderStub : ILandmarkAutoBookPayloadProvider
    {
        public IEnumerable<LandmarkInputFilePayload> GetFiles(Guid runId, Guid scenarioId) => new List<LandmarkInputFilePayload>();
    }
}
