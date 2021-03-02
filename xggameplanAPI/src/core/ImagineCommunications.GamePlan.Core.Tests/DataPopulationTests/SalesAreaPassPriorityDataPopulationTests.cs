using System;
using System.Linq;
using AutoFixture;
using AutoMapper;
using FluentAssertions;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.System.Models;
using xggameplan.AutoGen.AgDataPopulation;
using xggameplan.Profile;
using Xunit;

namespace ImagineCommunications.GamePlan.Core.Tests
{
    public class SalesAreaPassPriorityDataPopulationTests : DataPopulationTestBase
    {
        public SalesAreaPassPriorityDataPopulationTests()
        {
            _mapper = new Mapper(new MapperConfiguration(expression => expression.AddProfile<ClashProfile>()));
            _random = new Random();

            _salesAreas = _fixture.CreateMany<SalesArea>(10).ToList();
            _salesAreas.ForEach(s => s.Name = _salesAreaArr[_random.Next(0, _salesAreaArr.Length)]);

            var passReferences = _fixture.CreateMany<PassReference>(2).ToList();

            _scenario = _fixture.Build<Scenario>().With(a => a.Passes, passReferences).Create();

            _random = new Random();
            var salesAreaPassPriorities = _fixture.CreateMany<SalesAreaPriority>(2).ToList();
            salesAreaPassPriorities.ForEach(s => s.SalesArea = _salesAreaArr[_random.Next(0, _salesAreaArr.Length)]);

            _run = _fixture.Build<Run>().With(r => r.SalesAreaPriorities, salesAreaPassPriorities).Create();

            _random = new Random();
            var salesAreaPriorityModels = _fixture.CreateMany<SalesAreaPriority>(4).ToList();
            salesAreaPriorityModels.ForEach(s => s.SalesArea = _salesAreaArr[_random.Next(0, _salesAreaArr.Length)]);

            var passSalesAreaPriorities1 = _fixture.Build<PassSalesAreaPriority>().With(a => a.SalesAreaPriorities, salesAreaPriorityModels.Take(2).ToList()).Create();
            var passSalesAreaPriorities2 = _fixture.Build<PassSalesAreaPriority>().With(a => a.SalesAreaPriorities, salesAreaPriorityModels.Skip(2).Take(2).ToList()).Create();

            _passes = _fixture.CreateMany<Pass>(2).ToList();

            _passes[0].PassSalesAreaPriorities = passSalesAreaPriorities1;
            _passes[0].Id = passReferences[0].Id;
            _passes[1].PassSalesAreaPriorities = passSalesAreaPriorities2;
            _passes[1].Id = passReferences[1].Id;
        }

        [Fact(DisplayName = "Perform Sales Area Pass Priority Data Population Should Return Empty Object When Scenario-Passes Is Null.")]
        public void PerformSalesAreaPassPriorityDataPopulation_ShouldReturnEmptyObject_WhenScenarioPassesIsNull()
        {
            //Arrange
            const int expected = 0;
            _scenario.Passes = null;

            //Act
            var result = _passes.ToAgSalesAreaPassPriority(_scenario, _salesAreas, _run, _mapper);

            //Assert
            _ = result.Size.Should().Be(expected, null);
        }

        [Fact(DisplayName = "Perform sales area pass priority data population, should return empty object, When Pass parameter is null.")]
        public void PerformSalesAreaPassPriorityDataPopulation_ShouldReturnEmptyObject_WhenPassIsNull()
        {
            //Arrange
            const int expected = 0;
            _passes = null;

            //Act
            var result = _passes.ToAgSalesAreaPassPriority(_scenario, _salesAreas, _run, _mapper);

            //Assert
            _ = result.Size.Should().Be(expected, null);
        }

        [Fact(DisplayName = "Perform sales area pass priority data population, should return full object when It is in Run Level.")]
        public void PerformSalesAreaPassPriorityDataPopulation_ShouldReturnFullObject_WhenInRunLevel()
        {
            //Arrange
            _passes.ForEach(p => p.PassSalesAreaPriorities = null);
            var expected = 0;
            foreach (var passReference in _scenario.Passes)
            {
                expected += _run.SalesAreaPriorities
                    .FindAll(sap =>
                        _salesAreas.Find(sa => sa.Name == sap.SalesArea)?.CustomId > 0 &&
                        sap.Priority > 0)
                    .Count;
            }

            //Act
            var result = _passes.ToAgSalesAreaPassPriority(_scenario, _salesAreas, _run, _mapper);

            //Assert
            _ = result.Size.Should().Be(expected, null);
        }

        [Fact(DisplayName = "Perform sales area pass priority data population, should return full object when it is in Pass level.")]
        public void PerformSalesAreaPassPriorityDataPopulation_ShouldReturnFullObject_WhenInPassLevel()
        {
            //Arrange
            var expected = 0;
            foreach (var passReference in _scenario.Passes)
            {
                expected += _passes
                    .Find(p => p.Id == passReference.Id)
                    .PassSalesAreaPriorities.SalesAreaPriorities
                    .FindAll(sap =>
                        _salesAreas.Find(sa =>
                            sa.Name == sap.SalesArea
                            )?.CustomId > 0 && sap.Priority > 0)
                    .Count;
            }

            //Act
            var result = _passes.ToAgSalesAreaPassPriority(_scenario, _salesAreas, _run, _mapper);

            //Assert
            _ = result.Size.Should().Be(expected, null);
        }
    }
}
