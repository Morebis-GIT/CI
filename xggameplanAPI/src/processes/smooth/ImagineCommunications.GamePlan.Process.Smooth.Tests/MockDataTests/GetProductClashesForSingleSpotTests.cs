using System;
using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using ImagineCommunications.GamePlan.CommonTestsHelpers;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Models;
using ImagineCommunications.GamePlan.Process.Smooth.Services;
using ImagineCommunications.GamePlan.Process.Smooth.Types;
using Xunit;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests
{
    /// <summary>
    /// Unit tests for the ProductClashChecker.GetProductClashesForSingleSpot() method.
    /// </summary>
    [Collection("Smooth")]
    [Trait("Smooth", "Product clash checker for single spot")]
    public static class GetProductClashesForSingleSpotTests
    {
        [Fact(DisplayName = "Given an invalid clash code level when checking for a product clash then an exception is thrown.")]
        public static void
        Given_an_invalid_clash_code_level_when_checking_for_a_product_clash_then_an_exception_is_thrown()
        {
            const ClashCodeLevel invalidValue = (ClashCodeLevel)int.MinValue;

            // Assert
            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                // Arrange
                var fixture = new SafeFixture();
                var spotToCheckForClashes = fixture.Create<Spot>();

                var infoForAllSpots = new Dictionary<Guid, SpotInfo>
                {
                    [spotToCheckForClashes.Uid] = new SpotInfo { Uid = spotToCheckForClashes.Uid }
                };

                var labrat = new ProductClashChecker(enabled: true);

                // Act
                IEnumerable<Spot> result = labrat.GetProductClashesForSingleSpot(
                spotToCheckForClashes,
                new List<Spot>(0),
                infoForAllSpots,
                invalidValue
                );
            });

            _ = ex.Message.Should().Contain(invalidValue.ToString(), becauseArgs: null);
        }

        [Fact(DisplayName = "Given product clash checking is not enabled when checking for product clashes then none are found.")]
        public static void
        Given_product_clash_checking_is_not_enabled_when_checking_for_product_clashes_then_none_are_found()
        {
            // Arrange
            var labrat = new ProductClashChecker(enabled: false);

            var fixture = new SafeFixture();

            // Act
            IEnumerable<Spot> result = labrat.GetProductClashesForSingleSpot(
                fixture.Create<Spot>(),
                new List<Spot>(0),
                new Dictionary<Guid, SpotInfo>(0),
                ClashCodeLevel.Child
                );

            // Assert
            _ = result.Should().BeEmpty(becauseArgs: null);
        }

        [Fact(DisplayName = "Given no products currently being used when checking for product clashes then none are found.")]
        public static void
        Given_no_products_currently_being_used_when_checking_for_product_clashes_then_none_are_found()
        {
            // Arrange
            var fixture = new SafeFixture();

            var spotToCheckForClashes = fixture.Create<Spot>();

            var infoForAllSpots = new Dictionary<Guid, SpotInfo>
            {
                [spotToCheckForClashes.Uid] = new SpotInfo { Uid = spotToCheckForClashes.Uid }
            };

            var labrat = new ProductClashChecker(enabled: true);

            // Act
            IEnumerable<Spot> result = labrat.GetProductClashesForSingleSpot(
                spotToCheckForClashes,
                new List<Spot>(0),
                infoForAllSpots,
                ClashCodeLevel.Child
                );

            // Assert
            _ = result.Should().BeEmpty(becauseArgs: null);
        }

        [Fact(DisplayName = "Given a similar child product is already used when searching for a child clash then a clash is found.")]
        public static void
        Given_a_similar_child_product_is_already_used_when_searching_for_a_child_clash_then_a_clash_is_found()
        {
            // Arrange
            var fixture = new SafeFixture();

            var clashingSpot = fixture.Create<Spot>();
            var nonClashingSpot = fixture.Create<Spot>();

            var productCodeThatWillClash = fixture.Create<string>();

            var infoForAllSpots = new Dictionary<Guid, SpotInfo>
            {
                [clashingSpot.Uid] = new SpotInfo
                {
                    Uid = clashingSpot.Uid,
                    ProductClashCode = productCodeThatWillClash
                },
                [nonClashingSpot.Uid] = new SpotInfo
                {
                    Uid = nonClashingSpot.Uid
                }
            };

            var labrat = new ProductClashChecker(enabled: true);

            // Act
            IEnumerable<Spot> result = labrat.GetProductClashesForSingleSpot(
                clashingSpot,
                new List<Spot> { clashingSpot, nonClashingSpot },
                infoForAllSpots,
                ClashCodeLevel.Child
                );

            // Assert
            _ = result.Should().NotBeEmpty(becauseArgs: null);
        }

        [Fact(DisplayName = "Given a similar parent product is already used when searching for a parent clash then a clash is found.")]
        public static void
        Given_a_similar_parent_product_is_already_used_when_searching_for_a_parent_clash_then_a_clash_is_found()
        {
            // Arrange
            var fixture = new SafeFixture();

            var clashingSpot = fixture.Create<Spot>();
            var nonClashingSpot = fixture.Create<Spot>();

            var productCodeThatWillClash = fixture.Create<string>();

            var infoForAllSpots = new Dictionary<Guid, SpotInfo>
            {
                [clashingSpot.Uid] = new SpotInfo
                {
                    Uid = clashingSpot.Uid,
                    ParentProductClashCode = productCodeThatWillClash
                },
                [nonClashingSpot.Uid] = new SpotInfo
                {
                    Uid = nonClashingSpot.Uid
                }
            };

            var labrat = new ProductClashChecker(enabled: true);

            // Act
            IEnumerable<Spot> result = labrat.GetProductClashesForSingleSpot(
                clashingSpot,
                new List<Spot> { clashingSpot, nonClashingSpot },
                infoForAllSpots,
                ClashCodeLevel.Parent
                );

            // Assert
            _ = result.Should().NotBeEmpty(becauseArgs: null);
        }

        [Fact(DisplayName = "Given a similar child product is already used when searching for a parent clash then a clash is not found.")]
        public static void
        Given_a_similar_child_product_is_already_used_when_searching_for_a_parent_clash_then_a_clash_is_not_found()
        {
            // Arrange
            var fixture = new SafeFixture();

            var clashingSpot = fixture.Create<Spot>();
            var nonClashingSpot = fixture.Create<Spot>();

            var productCodeThatWillClash = fixture.Create<string>();

            var infoForAllSpots = new Dictionary<Guid, SpotInfo>
            {
                [clashingSpot.Uid] = new SpotInfo
                {
                    Uid = clashingSpot.Uid,
                    ProductClashCode = productCodeThatWillClash
                },
                [nonClashingSpot.Uid] = new SpotInfo
                {
                    Uid = nonClashingSpot.Uid
                }
            };

            var labrat = new ProductClashChecker(enabled: true);

            // Act
            IEnumerable<Spot> result = labrat.GetProductClashesForSingleSpot(
                clashingSpot,
                new List<Spot> { clashingSpot, nonClashingSpot },
                infoForAllSpots,
                ClashCodeLevel.Parent
                );

            // Assert
            _ = result.Should().BeEmpty(becauseArgs: null);
        }

        [Fact(DisplayName = "Given a similar parent product is already used when searching for a child clash then a clash is not found.")]
        public static void
        Given_a_similar_parent_product_is_already_used_when_searching_for_a_child_clash_then_a_clash_is_not_found()
        {
            // Arrange
            var fixture = new SafeFixture();

            var clashingSpot = fixture.Create<Spot>();
            var nonClashingSpot = fixture.Create<Spot>();

            var productCodeThatWillClash = fixture.Create<string>();

            var infoForAllSpots = new Dictionary<Guid, SpotInfo>
            {
                [clashingSpot.Uid] = new SpotInfo
                {
                    Uid = clashingSpot.Uid,
                    ParentProductClashCode = productCodeThatWillClash
                },
                [nonClashingSpot.Uid] = new SpotInfo
                {
                    Uid = nonClashingSpot.Uid
                }
            };

            var labrat = new ProductClashChecker(enabled: true);

            // Act
            IEnumerable<Spot> result = labrat.GetProductClashesForSingleSpot(
                clashingSpot,
                new List<Spot> { clashingSpot, nonClashingSpot },
                infoForAllSpots,
                ClashCodeLevel.Child
                );

            // Assert
            _ = result.Should().BeEmpty(becauseArgs: null);
        }
    }
}
