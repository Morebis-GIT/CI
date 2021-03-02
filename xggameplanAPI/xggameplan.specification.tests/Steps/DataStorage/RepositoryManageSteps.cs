using System;
using System.Globalization;
using NUnit.Framework;
using Raven.Abstractions.Extensions;
using TechTalk.SpecFlow;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.Steps.DataStorage
{
    [Binding]
    public class RepositoryManageSteps
    {
        private readonly IRepositoryAdapter _repositoryAdapter;

        public RepositoryManageSteps(IRepositoryAdapter repositoryAdapter)
        {
            _repositoryAdapter = repositoryAdapter;
        }

        [When(@"I (try to |)create (\d+) documents")]
        public void WhenICreateDocuments(bool throwException, int count)
        {
            _ = _repositoryAdapter.ExecuteAutoAdd(count).HandleError(throwException);
        }

        [When(@"I (try to |)create a document with values:")]
        public void WhenICreateADocument(bool throwException, Table values)
        {
            _ = _repositoryAdapter.ExecuteAdd(values).HandleError(throwException);
        }

        [When("I (try to |)create a document")]
        public void WhenICreateNewDocument(bool throwException)
        {
            _ = _repositoryAdapter.ExecuteAutoAdd(1).HandleError(throwException);
        }

        [When(@"I (try to |)create the following documents:")]
        public void WhenICreateTheFollowingDocuments(bool throwException, Table entities)
        {
            _ = _repositoryAdapter.ExecuteAddRange(entities).HandleError(throwException);
        }

        [When(@"I (try to |)get all documents")]
        public void WhenIGetAll(bool throwException)
        {
            _ = _repositoryAdapter.ExecuteGetAll().HandleError(throwException);
        }

        [When(@"I (try to |)get document with id (\d+)")]
        [When(@"I (try to |)get document with id '(.*)'")]
        public void WhenIGetDocumentWithId(bool throwException, string id)
        {
            _ = _repositoryAdapter.ExecuteGetById(id).HandleError(throwException);
        }

        [When(@"I (try to |)delete document with id (\d+)")]
        [When(@"I (try to |)delete document with id '(.*)'")]
        public void WhenIDeleteDocumentWithId(bool throwException, string id)
        {
            _ = _repositoryAdapter.ExecuteDelete(id).HandleError(throwException);
        }

        [When("I (try to |)truncate documents")]
        public void WhenITruncateDocuments(bool throwException)
        {
            _ = _repositoryAdapter.ExecuteTruncate().HandleError(throwException);
        }

        [When("I (try to |)count the number of documents")]
        public void WhenICountTheNumberOfDocuments(bool throwException)
        {
            _ = _repositoryAdapter.ExecuteCount().HandleError(throwException);
        }

        [When("I (try to |)call (.*) method")]
        public void WhenICallMethod(bool throwException, string methodName)
        {
            _ = _repositoryAdapter.ExecuteMethod(methodName).HandleError(throwException);
        }

        [When("I (try to |)call (.*) method with parameters:")]
        public void WhenICallMethodWithParameters(bool throwException, string methodName, Table parameters)
        {
            _ = _repositoryAdapter.ExecuteMethod(methodName, parameters).HandleError(throwException);
        }

        [When(@"I (try to |)update received document by values:")]
        public void WhenIUpdateReceivedDocument(bool throwException, Table values)
        {
            _ = _repositoryAdapter.ExecuteUpdate(values).HandleError(throwException);
        }

        [Then(@"there should be (\d+) documents returned")]
        [Then(@"there should be (\d+) documents counted")]
        public void ThenThereShouldBeNDocumentsReturned(int count)
        {
            var testContext = _repositoryAdapter.GetTestContext();
            Assert.AreEqual(count, testContext.LastOperationCount);
        }

        [Then(@"no documents should be returned")]
        public void ThenNoDocumentShouldBeReturned()
        {
            var testContext = _repositoryAdapter.GetTestContext();
            Assert.AreEqual(0, testContext.LastOperationCount);
        }

        [Then(@"the received document should contain the following values:")]
        [Then(@"the received result should contain the following values:")]
        public void ThenTheReceivedDocumentShouldContainTheFollowingValues(Table values)
        {
            _repositoryAdapter.CheckReceivedResult(values);
        }

        [Then(@"the exception is thrown")]
        public void ThenTheExceptionIsThrown()
        {
            var testContext = _repositoryAdapter.GetTestContext();
            Assert.NotNull(testContext.LastException, "The exception is expected but it hasn't been thrown.");
        }

        [Then(@"the exception type is (.*)")]
        public void ThenTheExceptionTypeIs(string exceptionType)
        {
            var testContext = _repositoryAdapter.GetTestContext();
            Assert.NotNull(testContext.LastException, "The exception is expected but it hasn't been thrown.");
            Assert.AreEqual(exceptionType, testContext.LastException.GetType().Name);
        }

        [Then(@"result should be (.*)")]
        public void ThenResultShouldBe(object value)
        {
            var testContext = _repositoryAdapter.GetTestContext();
            var destinationType = testContext.LastSingleResult.GetType();
            object convertedValue = destinationType == typeof(Guid)
                ? Guid.Parse(value.ToInvariantString())
                : Convert.ChangeType(value, testContext.LastSingleResult.GetType(), CultureInfo.InvariantCulture);
            Assert.AreEqual(convertedValue, testContext.LastSingleResult);
        }
    }
}
