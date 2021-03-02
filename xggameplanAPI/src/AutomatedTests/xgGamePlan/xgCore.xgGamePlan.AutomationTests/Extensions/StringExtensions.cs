namespace xgCore.xgGamePlan.AutomationTests.Extensions
{
    public static class StringExtensions
    {
        public static bool TryParseToBoolean(this string value, out bool result)
        {
            return bool.TryParse(value, out result);
        }
    }
}
