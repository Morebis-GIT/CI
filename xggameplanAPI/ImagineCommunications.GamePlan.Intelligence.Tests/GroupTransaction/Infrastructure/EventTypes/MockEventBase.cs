using ImagineCommunications.GamePlan.Intelligence.Tests.GroupTransaction.Infrastructure.EventTypes.Interfaces;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.GroupTransaction.Infrastructure.EventTypes
{
    public class MockEventBase
    {
        public int Id { get; set; }
        public bool IsModelValid { get; set; }
        public bool BusinessValidationPassed { get; set; }
    }

    public class MockEventOne : MockEventBase, IMockEventOne { }

    public class MockEventTwo : MockEventBase, IMockEventTwo { }

    public class MockEventThree : MockEventBase, IMockEventThree { }

    public class MockEventFour : MockEventBase, IMockEventFour { }
}
