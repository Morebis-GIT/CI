using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Results;
using AutoFixture;
using FluentAssertions;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using Moq;
using NUnit.Framework;
using xggameplan.Model;

namespace xggameplan.tests.ControllerTests
{
    public partial class BreaksControllerTests : IDisposable
    {
        private const string ValidBreakType = "VA-ValidBreakType";
        private const string InvalidBreakType_TooShort = "X";
        private const string InvalidBreakType_WhiteSpaceSecondCharacter = "X - Invalid Because of 2nd character";

        [Test(Description = "BreakType field::Null break type is invalid")]
        public void NullBreakTypeIsInvalid()
        {
            // Arrange
            var fakeSalesAreaName = _fixture.Create<string>();
            var fakeSalesAreaList = _fixture
                .Build<SalesArea>()
                .With(p => p.Name, fakeSalesAreaName)
                .CreateMany(1)
                .ToList();

            _ = _fakeSalesAreaRepository
                .Setup(m => m.FindByNames(It.IsAny<List<string>>()))
                .Returns(fakeSalesAreaList);

            var sampleToTest = _fixture
                .Build<CreateBreak>()
                .Without(p => p.BreakType)
                .Without(p => p.ClockHour)
                .With(p => p.SalesArea, fakeSalesAreaName)
                .CreateMany(1)
                .ToList();

            // Act
            var result = _controller.Post(sampleToTest);

            // Assert
            _ = result.Should().BeOfType<InvalidModelStateResult>(null);

            var modelState = ((InvalidModelStateResult)result).ModelState;

            _ = modelState
                    .IsValidField(nameof(CreateBreak.BreakType))
                    .Should().BeFalse(null);

            _ = modelState[nameof(CreateBreak.BreakType)].Errors[0].ErrorMessage
                    .Should().StartWith("The BreakType value cannot be null.", null);

            _ = modelState[nameof(CreateBreak.BreakType)].Errors[0].ErrorMessage
                    .Should().EndWith(sampleToTest[0].ExternalBreakRef, null);
        }

        [Test(Description = "BreakType field::Valid Break type has a length greater than or equal to two")]
        public void ValidBreakTypeHasLengthGreaterThanOrEqualToTwo()
        {
            // Arrange
            var fakeSalesAreaName = _fixture.Create<string>();
            var fakeSalesAreaList = _fixture
                .Build<SalesArea>()
                .With(p => p.Name, fakeSalesAreaName)
                .CreateMany(1)
                .ToList();

            _ = _fakeSalesAreaRepository
                .Setup(m => m.FindByNames(It.IsAny<List<string>>()))
                .Returns(fakeSalesAreaList);

            var sampleToTest = _fixture
                .Build<CreateBreak>()
                .With(p => p.BreakType, ValidBreakType)
                .Without(p => p.ClockHour)
                .With(p => p.SalesArea, fakeSalesAreaName)
                .CreateMany(1)
                .ToList();

            // Act
            var result = _controller.Post(sampleToTest);

            // Assert
            _ = result.Should().BeOfType<OkResult>(null);
        }

        [Test(Description = "BreakType field::Invalid Break type has a length less than two")]
        public void InvalidBreakTypeHasLengthLessThanTwo()
        {
            // Arrange
            var fakeSalesAreaName = _fixture.Create<string>();
            var fakeSalesAreaList = _fixture
                .Build<SalesArea>()
                .With(p => p.Name, fakeSalesAreaName)
                .CreateMany(1)
                .ToList();

            _ = _fakeSalesAreaRepository
                .Setup(m => m.FindByNames(It.IsAny<List<string>>()))
                .Returns(fakeSalesAreaList);

            var sampleToTest = _fixture
                .Build<CreateBreak>()
                .With(p => p.BreakType, InvalidBreakType_TooShort)
                .Without(p => p.ClockHour)
                .With(p => p.SalesArea, fakeSalesAreaName)
                .CreateMany(1)
                .ToList();

            // Act
            var result = _controller.Post(sampleToTest);

            // Assert
            _ = result.Should().BeOfType<InvalidModelStateResult>(null);

            var modelState = ((InvalidModelStateResult)result).ModelState;

            _ = modelState
                    .IsValidField(nameof(CreateBreak.BreakType))
                    .Should().BeFalse(null);

            _ = modelState[nameof(CreateBreak.BreakType)].Errors[0].ErrorMessage
                    .Should().StartWith("The BreakType value is too short.", null);

            _ = modelState[nameof(CreateBreak.BreakType)].Errors[0].ErrorMessage
                    .Should().EndWith(sampleToTest[0].ExternalBreakRef, null);
        }

        [Test(Description = "BreakType field::Two Invalid breaks with Break type with length less than two")]
        public void InvalidTwoBreaksWithBreakTypeWithLengthLessThanTwo()
        {
            // Arrange
            var fakeSalesAreaName = _fixture.Create<string>();
            var fakeSalesAreaList = _fixture
                .Build<SalesArea>()
                .With(p => p.Name, fakeSalesAreaName)
                .CreateMany(1)
                .ToList();

            _ = _fakeSalesAreaRepository
                .Setup(m => m.FindByNames(It.IsAny<List<string>>()))
                .Returns(fakeSalesAreaList);

            var sampleToTest = _fixture
                .Build<CreateBreak>()
                .With(p => p.BreakType, InvalidBreakType_TooShort)
                .Without(p => p.ClockHour)
                .With(p => p.SalesArea, fakeSalesAreaName)
                .CreateMany(2)
                .ToList();

            // Act
            var result = _controller.Post(sampleToTest);

            // Assert
            _ = result.Should().BeOfType<InvalidModelStateResult>(null);

            var modelState = ((InvalidModelStateResult)result).ModelState;

            _ = modelState
                    .IsValidField(nameof(CreateBreak.BreakType))
                    .Should().BeFalse(null);

            _ = modelState[nameof(CreateBreak.BreakType)].Errors
                    .Should().HaveCount(2, null);

            _ = modelState[nameof(CreateBreak.BreakType)].Errors[0].ErrorMessage
                    .Should().StartWith("The BreakType value is too short.", null);

            _ = modelState[nameof(CreateBreak.BreakType)].Errors[0].ErrorMessage
                    .Should().EndWith(sampleToTest[0].ExternalBreakRef, null);

            _ = modelState[nameof(CreateBreak.BreakType)].Errors[1].ErrorMessage
                    .Should().StartWith("The BreakType value is too short.", null);

            _ = modelState[nameof(CreateBreak.BreakType)].Errors[1].ErrorMessage
                    .Should().EndWith(sampleToTest[1].ExternalBreakRef, null);
        }

        [Test(Description = "BreakType field::Invalid Break type has a length greater than or equal to two but the 2nd character is whitespace")]
        public void InvalidBreakTypeHasLengthGreaterThanOrEqualToTwoButSecondCharacterIsWhiteSpace()
        {
            // Arrange
            var fakeSalesAreaName = _fixture.Create<string>();
            var fakeSalesAreaList = _fixture
                .Build<SalesArea>()
                .With(p => p.Name, fakeSalesAreaName)
                .CreateMany(1)
                .ToList();

            _ = _fakeSalesAreaRepository
                .Setup(m => m.FindByNames(It.IsAny<List<string>>()))
                .Returns(fakeSalesAreaList);

            var sampleToTest = _fixture
                .Build<CreateBreak>()
                .With(p => p.BreakType, InvalidBreakType_WhiteSpaceSecondCharacter)
                .Without(p => p.ClockHour)
                .With(p => p.SalesArea, fakeSalesAreaName)
                .CreateMany(1)
                .ToList();

            // Act
            var result = _controller.Post(sampleToTest);

            // Assert
            _ = result.Should().BeOfType<InvalidModelStateResult>(null);

            var modelState = ((InvalidModelStateResult)result).ModelState;

            _ = modelState
                    .IsValidField(nameof(CreateBreak.BreakType))
                    .Should().BeFalse(null);

            _ = modelState[nameof(CreateBreak.BreakType)].Errors[0].ErrorMessage
                    .Should().StartWith("The BreakType prefix value is too short.", null);

            _ = modelState[nameof(CreateBreak.BreakType)].Errors[0].ErrorMessage
                    .Should().EndWith(sampleToTest[0].ExternalBreakRef, null);
        }
    }
}
