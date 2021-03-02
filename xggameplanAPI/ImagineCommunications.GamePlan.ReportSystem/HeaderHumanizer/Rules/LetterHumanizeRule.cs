namespace ImagineCommunications.GamePlan.ReportSystem.HeaderHumanizer.Rules
{
    internal class LetterHumanizeRule : HumanizeRule
    {
        internal override bool IsBreak(char current, char? next, char? previous)
        {
            if (previous == null)
            {
                return false;
            }

            if (char.IsLetter(current))
            {
                if (!char.IsLetter(previous.Value))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
