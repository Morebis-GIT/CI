using System;
using NodaTime;

namespace xggameplan.specification.tests.Interfaces
{
    public interface ITestClock : IClock
    {
        void Freeze(DateTime freezeTime);
    }
}
