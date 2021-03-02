using AutoFixture;
using NodaTime;

namespace ImagineCommunications.GamePlan.CommonTestsHelpers
{
    /// <summary>
    /// NodaTime's Duration some times blows up with an arithmatic overflow exception when used
    /// with AutoFixture. I've no idea how an Int64 does that but this version stops it happening.
    /// </summary>
    public class SafeFixture
        : Fixture
    {
        public SafeFixture()
            : base() => this.Register(() => Duration.Epsilon);
    }
}
