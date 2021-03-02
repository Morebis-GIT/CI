using System;

namespace xggameplan.core.Landmark
{
    public class LandmarkHttpPolicyOptions
    {
        public const string CustomRequestUriActionName = "RequestUriUpdateAction";
        public const string CustomRequestBodyActionName = "RequestBodyUpdateAction";

        public TimeSpan TimeoutPerRequest { get; set; }
        public int MaxAttemptsPerInstance { get; set; }
        public TimeSpan CircuitBreakDuration { get; set; }
    }
}
