using ImagineCommunications.GamePlan.Process.Smooth.Types;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services
{
    public static class ContainerReferenceService
    {
        public static bool IsContainerReference(BreakExternalReference breakExternalReference) =>
            ContainerReference.TryParse(breakExternalReference, out var _);
    }
}
