namespace ImagineCommunications.GamePlan.ReportSystem.HeaderHumanizer.Rules
{
    internal class DigitalHumanizeRule : HumanizeRule
    {
        internal override bool IsBreak(char current, char? next, char? previous)
        {
            if (previous == null)
            {
                return false;
            }

            if (char.IsDigit(current))
            {
                if (!char.IsDigit(previous.Value))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
