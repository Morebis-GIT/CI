using System;
using NodaTime;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.Infrastructure
{
    public class TestClock : ITestClock
    {
        private DateTime? _freezeTime;

        public Instant GetCurrentInstant() =>
            NodaConstants.BclEpoch.PlusTicks(_freezeTime?.Ticks ?? DateTime.UtcNow.Ticks);

        public void Freeze(DateTime freezeTime) => _freezeTime = freezeTime;
    }
}
