using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Recommendations.Objects;
using ImagineCommunications.GamePlan.Domain.SmoothFailures;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers
{
    public class SmoothTestResults
    {
        internal object Sender { get; set; }
        internal DateTime StartDate { get; set; }
        internal DateTime EndDate { get; set; }
        internal IReadOnlyList<Recommendation> Recommendations { get; set; }
        internal IReadOnlyList<SmoothFailure> SmoothFailures { get; set; }
    }
}
