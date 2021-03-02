using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow;

namespace xgCore.xgGamePlan.AutomationTests.GlobalSteps
{
    [Binding]
    internal class StepsTransforms
    {
        /// <summary>
        /// Transformation rule that uses SpecFlow to make comma separated string with values transformed to List type when passed as arguments to Steps
        /// </summary>
        /// <param name="commaSeparatedList"></param>
        /// <returns></returns>
        [StepArgumentTransformation]
        public static List<string> TransformToListOfString(string commaSeparatedList)
        {
            return commaSeparatedList.Split(',').ToList();
        }
    }
}
