using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http.Results;
using AutoFixture;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Autopilot.FlexibilityLevels;
using ImagineCommunications.GamePlan.Domain.Autopilot.Rules;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Rules;
using ImagineCommunications.GamePlan.Domain.BusinessRules.RuleTypes;
using Moq;
using NUnit.Framework;
using xggameplan.Configuration;
using xggameplan.Controllers;
using xggameplan.core.Configuration;
using xggameplan.Model;

namespace xggameplan.tests.ControllerTests
{
    [TestFixture(Category = "Controllers :: RuleTypes")]
    public class RuleTypesControllerTests : IDisposable
    {
        private Mock<IRuleRepository> _fakeRuleRepository;
        private Mock<IRuleTypeRepository> _fakeRuleTypeRepository;
        private Mock<IAutopilotRuleRepository> _fakeAutopilotRuleRepository;
        private Mock<IFlexibilityLevelRepository> _fakeFlexibilityLevelRepository;
        private Fixture _fixture;
        private IMapper _mapper;
        private int _ruleTypeId;
        private int _unknownRuleTypeId;

        private RuleTypesController _controller;

        [SetUp]
        public void Init()
        {
            _fakeRuleRepository = new Mock<IRuleRepository>();
            _fakeRuleTypeRepository = new Mock<IRuleTypeRepository>();
            _fakeAutopilotRuleRepository = new Mock<IAutopilotRuleRepository>();
            _fakeFlexibilityLevelRepository = new Mock<IFlexibilityLevelRepository>();
            _ruleTypeId = 1;
            _unknownRuleTypeId = 99;

            _fixture = new Fixture();

            _mapper = AutoMapperInitializer.Initialize(AutoMapperConfig.SetupAutoMapper);

            var ruleTypes = new List<RuleType>
            {
                new RuleType
                {
                    Id = 1,
                    Name = "RT 1-1",
                    IsCustom = false,
                    AllowedForAutopilot = true
                },
                new RuleType
                {
                    Id = 2,
                    Name = "RT 2-1",
                    IsCustom = false,
                    AllowedForAutopilot = false
                },
                new RuleType
                {
                    Id = 3,
                    Name = "RT 3-1",
                    IsCustom = false,
                    AllowedForAutopilot = true
                }
            };

            _ = _fakeRuleTypeRepository
                .Setup(m => m.GetAll(false))
                .Returns(ruleTypes);

            _ = _fakeRuleTypeRepository
                .Setup(m => m.Get(_ruleTypeId))
                .Returns(ruleTypes.FirstOrDefault(rt => rt.Id == _ruleTypeId));

            _ = _fakeRuleTypeRepository
                .Setup(m => m.GetAll(true))
                .Returns(ruleTypes.Where(rt => rt.AllowedForAutopilot));

            _ = _fakeRuleTypeRepository
                .Setup(m => m.Delete(_ruleTypeId));

            _ = _fakeRuleTypeRepository
                .Setup(m => m.Update(new RuleType
                {
                    Id = 1,
                    Name = "RT 1-1",
                    IsCustom = false,
                    AllowedForAutopilot = true
                }));

            _ = _fakeRuleRepository.Setup(m => m.GetAll()).Returns(
                new List<Rule>
                {
                    new Rule
                    {
                        Id = 1,
                        RuleTypeId = 1,
                        Description = "R 1-1",
                        InternalType = "internal",
                        Type = "rule"
                    },
                    new Rule
                    {
                        Id = 2,
                        RuleTypeId = 1,
                        Description = "R 2-1",
                        InternalType = "internal",
                        Type = "rule"
                    }
                }
            );

            _controller = new RuleTypesController(
                _fakeRuleTypeRepository.Object,
                _fakeRuleRepository.Object,
                _fakeAutopilotRuleRepository.Object,
                _fakeFlexibilityLevelRepository.Object,
                _mapper);
        }

        [Test]
        [Description("When getting all rule types then first element must have id equal to 1")]
        public void GetAllWhenCalledThenShouldReturnRuleTypeModels()
        {
            var result = _controller.GetAll();
            Assert.AreEqual(result.First().Id, 1);
            Assert.AreEqual(result.Count(), 3);
        }

        [Test]
        [Description("Given existing id when getting rule type then model must be returned")]
        public void GetAllWhenCalledWithExistingIdThenShouldReturnOk()
        {
            var result = _controller.Get(_ruleTypeId);
            var contentResult = result as OkNegotiatedContentResult<RuleTypeModel>;

            Assert.AreEqual(contentResult.Content.Id, _ruleTypeId);
        }

        [Test]
        [Description("Given nonexistent id when getting rule type then NotFound status must be returned")]
        public void GetAllWhenCalledWithNonexistentIdThenShouldReturnNotFound()
        {
            Assert.IsInstanceOf<NotFoundResult>(_controller.Get(_unknownRuleTypeId));
        }

        [Test]
        [Description("Given existing id when deleting rule type then model must be deleted")]
        public void DeleteWhenCalledWithExistingIdThenShouldReturnNoContent()
        {
            var result = _controller.Delete(_ruleTypeId);
            var statusCodeResult = result as StatusCodeResult;

            Assert.AreEqual(statusCodeResult.StatusCode, HttpStatusCode.NoContent);
        }

        [Test]
        [Description("Given nonexistent id when deleting rule type then NotFound status must be returned")]
        public void DeleteWhenCalledWithNonexistentIdThenShouldReturnNotFound()
        {
            Assert.IsInstanceOf<NotFoundResult>(_controller.Delete(_unknownRuleTypeId));
        }

        [Test]
        [Description("Given valid model when updating rule type then model property must be updated")]
        public void PutWhenCalledWithValidModelThenShouldReturnOk()
        {
            var model = GetRuleTypeModel(_ruleTypeId);
            model.Name = "New name";

            var result = _controller.Put(model.Id, model) as OkNegotiatedContentResult<RuleTypeModel>;

            Assert.AreEqual(result.Content.Name, model.Name);
        }

        [Test]
        [Description("Given not valid id when updating rule type then correct validation message must be returned")]
        public void PutWhenCalledWithNotValidIdThenShouldReturnCorrectValidationMessage()
        {
            var result = _controller.Put(_ruleTypeId, GetRuleTypeModel(_unknownRuleTypeId)) as BadRequestErrorMessageResult;

            Assert.AreEqual(result.Message, "Model id does not match");
        }

        [Test]
        [Description("Given nonexistent id when updating rule type then NotFound status must be returned")]
        public void PutWhenCalledWithNonexistentIdThenShouldReturnNotFound()
        {
            var model = GetRuleTypeModel(_unknownRuleTypeId);

            Assert.IsInstanceOf<NotFoundResult>(_controller.Put(model.Id, model));
        }

        private RuleTypeModel GetRuleTypeModel(int ruleTypeId)
        {
            var rules = _fixture.Build<RuleModel>()
                .With(r => r.Id, 1)
                .With(r => r.RuleTypeId, ruleTypeId)
                .With(r => r.Description, "R 1-1")
                .With(r => r.InternalType, "internal")
                .CreateMany(1)
                .ToList();

            return _fixture.Build<RuleTypeModel>()
                .With(r => r.Id, ruleTypeId)
                .With(r => r.Name, "Test name")
                .With(r => r.IsCustom, false)
                .With(r => r.AllowedForAutopilot, true)
                .With(r => r.Rules, rules)
                .Create();
        }

        [TearDown]
        public void Cleanup()
        {
            _controller = null;
            _mapper = null;
        }

        public void Dispose()
        {
            _controller = null;
            _mapper = null;
        }
    }
}
