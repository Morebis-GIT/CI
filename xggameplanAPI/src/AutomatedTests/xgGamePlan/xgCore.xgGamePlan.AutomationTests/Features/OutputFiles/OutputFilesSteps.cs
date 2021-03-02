using System.Threading.Tasks;
using NUnit.Framework;
using TechTalk.SpecFlow;
using xgCore.xgGamePlan.ApiEndPoints.Routes;
using xgCore.xgGamePlan.AutomationTests.Contexts;

namespace xgCore.xgGamePlan.AutomationTests.Features.OutputFiles
{
    [Binding]
    public class OutputFilesSteps
    {
        private readonly IOutputFilesApi _outputFilesApi;
        private readonly OutputFilesContext _outputFilesContext;

        public OutputFilesSteps(
            IOutputFilesApi outputFilesApi,
            OutputFilesContext outputFilesContext)
        {
            _outputFilesApi = outputFilesApi;
            _outputFilesContext = outputFilesContext;
        }

        [When(@"I get all OutputFiles")]
        public async Task WhenIGetAllOutputFiles()
        {
            _outputFilesContext.OutputFileModels = await _outputFilesApi
                .GetAll()
                .ConfigureAwait(false);
        }

        [Then(@"not empty collection is returned")]
        public void ThenNotEmptyCollectionIsReturned()
        {
            Assert.IsNotEmpty(_outputFilesContext.OutputFileModels);
        }
    }
}
