using NUnit.Framework;
using TechTalk.SpecFlow;

namespace xggameplan.specification.tests.Extensions
{
    public static class FeatureContextExtension
    {
        public static void IgnoreFeature(this FeatureContext featureContext, string comment)
        {
            Assert.Ignore(comment);
        }
    }
}
