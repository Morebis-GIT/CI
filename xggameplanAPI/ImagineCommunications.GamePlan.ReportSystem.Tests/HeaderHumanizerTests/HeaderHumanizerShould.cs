using System.Collections.Generic;
using Xunit;

namespace ImagineCommunications.GamePlan.ReportSystem.Tests.HeaderHumanizerTests
{
    public class HeaderHumanizerShould
    {
        public static IEnumerable<object[]> GetHeaders()
        {
            yield return new object[] { "lowercase", "lowercase" };
            yield return new object[] { "Class", "Class" };
            yield return new object[] { "MyClass", "My Class" };
            yield return new object[] { "HTML", "HTML" };
            yield return new object[] { "PDFLoader", "PDF Loader" };
            yield return new object[] { "AString", "A String" };
            yield return new object[] { "SimpleXMLParser", "Simple XML Parser" };
            yield return new object[] { "GL11Version", "GL 11 Version" };
            yield return new object[] { "99Bottles", "99 Bottles" };
            yield return new object[] { "May5", "May 5" };
            yield return new object[] { "BFG9000", "BFG 9000" };
        }

        [Theory]
        [MemberData(nameof(GetHeaders))]
        public void HeaderHumanize(string headerText, string expectedText)
        {
            var actualText = HeaderHumanizer.HeaderHumanizer.Humanize(headerText);
            Assert.Equal(expectedText, actualText);
        }
    }
}
