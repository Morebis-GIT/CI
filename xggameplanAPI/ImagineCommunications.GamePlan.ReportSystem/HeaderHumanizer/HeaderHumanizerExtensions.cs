using ImagineCommunications.GamePlan.ReportSystem.ReportConfiguration.MemberConfig;

namespace ImagineCommunications.GamePlan.ReportSystem.HeaderHumanizer
{
    public static class HeaderHumanizerExtensions
    {
        public static string GetHeader(this IMemberOptions options, bool useHeaderHumanizer)
        {
            if (options.IsHeader)
            {
                return options.Header;
            }

            if (useHeaderHumanizer)
            {
                return HeaderHumanizer.Humanize(options.MemberName);
            }

            return options.MemberName;
        }
    }
}
