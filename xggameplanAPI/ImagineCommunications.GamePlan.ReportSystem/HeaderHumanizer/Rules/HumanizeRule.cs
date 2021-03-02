namespace ImagineCommunications.GamePlan.ReportSystem.HeaderHumanizer.Rules
{
    internal abstract class HumanizeRule
    {
        internal abstract bool IsBreak(char current, char? next, char? previous);
    }
}
