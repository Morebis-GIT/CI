using System.Collections.Generic;
using System.Text;
using ImagineCommunications.GamePlan.ReportSystem.HeaderHumanizer.Rules;

namespace ImagineCommunications.GamePlan.ReportSystem.HeaderHumanizer
{
    public static class HeaderHumanizer
    {
        private static IEnumerable<HumanizeRule> HumanizeRules { get; } = new List<HumanizeRule>
        {
            new DigitalHumanizeRule(),
            new UpperHumanizeRule(),
            new LetterHumanizeRule()
        };

        public static string Humanize(string header)
        {
            var humanText = new StringBuilder();
            for (var i = 0; i < header.Length; i++)
            {
                var current = header[i];
                char? next = null;
                char? previous = null;

                if (i + 1 < header.Length)
                {
                    next = header[i + 1];
                }

                if (i > 0)
                {
                    previous = header[i - 1];
                }

                foreach (var rule in HumanizeRules)
                {
                    if (rule.IsBreak(current, next, previous))
                    {
                        humanText.Append(" ");
                        break;
                    }
                }

                humanText.Append(current);
            }

            return humanText.ToString();
        }
    }
}
