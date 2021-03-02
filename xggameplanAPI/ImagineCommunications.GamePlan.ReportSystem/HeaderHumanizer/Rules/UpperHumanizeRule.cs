namespace ImagineCommunications.GamePlan.ReportSystem.HeaderHumanizer.Rules
{
    internal class UpperHumanizeRule : HumanizeRule
    {
        internal override bool IsBreak(char current, char? next, char? previous)
        {
            if (previous == null)
            {
                return false;
            }

            if (char.IsUpper(current))
            {
                if (next != null && char.IsLower(next.Value)
                    || !char.IsUpper(previous.Value))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
