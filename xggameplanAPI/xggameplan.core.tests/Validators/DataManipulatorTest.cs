using System;
using System.Collections.Generic;
using NUnit.Framework;
using xggameplan.core.Validators;
using xggameplan.Model;

namespace xggameplan.core.tests.Validators
{
    [TestFixture(Category = "Validations :: DataManipulator")]
    public class DataManipulatorTest : IDisposable
    {
        private IDataManipulator _dataManipulator;

        [SetUp]
        public void Init()
        {
            _dataManipulator = new DataManipulator();
        }

        [TearDown]
        public void Cleanup()
        {
            _dataManipulator = null;
        }

        [Test]
        public void ManipulateCreatePassModelWithNullValueDoesNotFail()
        {
            CreatePassModel model = null;

            model = _dataManipulator.Manipulate(model);

            Assert.That(model, Is.Null);
        }

        [Test]
        public void ManipulateCreatePassModelPassSalesAreaPrioritiesWithNullValueDoesNotFail()
        {
            var model = new CreatePassModel
            {
                PassSalesAreaPriorities = null
            };

            model = _dataManipulator.Manipulate(model);

            Assert.That(model.PassSalesAreaPriorities, Is.Null);
        }

        [Test]
        public void ManipulatePassSalesAreaPrioritiesWithStartDateValueSetsTheStartDateToNull()
        {
            var model = new CreatePassModel
            {
                PassSalesAreaPriorities = new PassSalesAreaPriorityModel() { StartDate = DateTime.Now }
            };

            model = _dataManipulator.Manipulate(model);

            Assert.That(model.PassSalesAreaPriorities.StartDate, Is.Null);
        }

        [Test]
        public void ManipulatePassSalesAreaPrioritiesWithEndDateValueSetsTheEndtDateToNull()
        {
            var model = new CreatePassModel
            {
                PassSalesAreaPriorities = new PassSalesAreaPriorityModel()
                {
                    EndDate = DateTime.Now
                }
            };

            model = _dataManipulator.Manipulate(model);

            Assert.That(model.PassSalesAreaPriorities.EndDate, Is.Null);
        }

        [Test]
        public void ManipulateCreateRunModelDropsTimeFromStartDate()
        {
            CreateRunModel model = getCreateRunModel();

            model = _dataManipulator.Manipulate(model);

            Assert.AreEqual(model.StartDate, model.StartDate.Date);
        }

        [Test]
        public void ManipulateCreateRunModelDropsTimeFromEndDate()
        {
            CreateRunModel model = getCreateRunModel();

            model = _dataManipulator.Manipulate(model);

            Assert.AreEqual(model.EndDate, model.EndDate.Date);
        }

        [Test]
        public void ManipulateCreateRunModelDropsTimeFromStartDateOfPassSalesAreaPriorities()
        {
            CreateRunModel model = getCreateRunModel();

            model = _dataManipulator.Manipulate(model);

            Assert.AreEqual(model.Scenarios[0].Passes[0].PassSalesAreaPriorities.StartDate, model.Scenarios[0].Passes[0].PassSalesAreaPriorities.StartDate.Value.Date);
        }

        [Test]
        public void ManipulateCreateRunModelDropsTimeFromEndDateOfPassSalesAreaPriorities()
        {
            CreateRunModel model = getCreateRunModel();

            model = _dataManipulator.Manipulate(model);

            Assert.AreEqual(model.Scenarios[0].Passes[0].PassSalesAreaPriorities.EndDate, model.Scenarios[0].Passes[0].PassSalesAreaPriorities.EndDate.Value.Date);
        }

        public void Dispose()
        {
            _dataManipulator = null;
        }

        private CreateRunModel getCreateRunModel()
        {
            return new CreateRunModel()
            {
                StartDate = new DateTime(1, 1, 1, 1, 1, 1)
                ,
                EndDate = new DateTime(1, 1, 1, 1, 1, 1)
                ,
                Scenarios = new List<CreateRunScenarioModel>
                {
                    new CreateRunScenarioModel()
                    {
                        Passes = new List<PassModel>
                        {
                            new PassModel()
                            {
                                PassSalesAreaPriorities = new PassSalesAreaPriorityModel()
                                {
                                    StartDate = new DateTime(1, 1, 1, 1, 1, 1)
                                    ,EndDate = new DateTime(1, 1, 1, 1, 1, 1)
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}
