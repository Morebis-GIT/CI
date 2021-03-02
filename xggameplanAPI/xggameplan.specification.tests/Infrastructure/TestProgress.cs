using NUnit.Framework;

namespace xggameplan.specification.tests.Infrastructure
{
    public static class TestProgress
    {
        public static void WriteLine(string message)
        {
            TestContext.Progress.WriteLine(message);
        }
    }
}
