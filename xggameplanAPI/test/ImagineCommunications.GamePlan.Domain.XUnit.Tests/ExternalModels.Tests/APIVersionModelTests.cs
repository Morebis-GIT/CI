using FluentAssertions;
using xggameplan.Model;
using Xunit;

namespace ImagineCommunications.GamePlan.Domain.XUnit.Tests.ExternalModels.Tests
{
    [Trait("Domain (external) models", nameof(APIVersionModel))]
    public class APIVersionModelTests
    {
        private const string MasterBuildVersionString = "3.88.0-master.372.20116+ci.0adbd77.master";
        private const string BugfixBranchVersionString = "3.99.0-bugfix.380.20127+ci.562ce5a.bugfix-xggt-15608-master-embed-correct-versiontxt";

        [Fact(DisplayName = "Missing version data returns empty version")]
        public void MissingVersionData()
        {
            // Arrange
            string noVersionData = string.Empty;

            // Act
            var result = APIVersionModel.Create(noVersionData);

            // Assert
            _ = result.Version.Should().BeEmpty(null);
        }

        [Fact(DisplayName = "Missing branch name returns empty version")]
        public void MissingBranchName()
        {
            // Arrange
            const string noVersionData = "3.11.0-master.372.20116+ci.0adbd77";

            // Act
            var result = APIVersionModel.Create(noVersionData);

            // Assert
            _ = result.Version.Should().BeEmpty(null);
        }

        [Fact(DisplayName = "If only the GitVersion and a dash is present then return empty version")]
        public void IfOnlyTheGitVersionAndDashReturnsDataMissing()
        {
            // Arrange
            const string missingData = "3.11.0-";

            // Act
            var result = APIVersionModel.Create(missingData);

            // Assert
            _ = result.Version.Should().BeEmpty(null);
        }

        [Fact(DisplayName = "If only the GitVersion is present then return empty version")]
        public void IfOnlyTheGitVersionReturnsDataMissing()
        {
            // Arrange
            const string missingData = "3.11.0";

            // Act
            var result = APIVersionModel.Create(missingData);

            // Assert
            _ = result.Version.Should().BeEmpty(null);
        }

        [Fact(DisplayName = "Master build should have GitVersion build version")]
        public void MasterBuildShouldHaveGitVersionBuildVersion()
        {
            // Arrange

            // Act
            var result = APIVersionModel.Create(MasterBuildVersionString);

            // Assert
            _ = result.Version.Should().Be("3.88.0", null);
        }

        [Fact(DisplayName = "Master build should have master branch name")]
        public void MasterBuildShouldHaveMasterBranchName()
        {
            // Arrange

            // Act
            var result = APIVersionModel.Create(MasterBuildVersionString);

            // Assert
            _ = result.Branch.Should().Be("master", null);
        }

        [Fact(DisplayName = "Master build should have package build number")]
        public void MasterBuildShouldHavePackageBuildNumber()
        {
            // Arrange

            // Act
            var result = APIVersionModel.Create(MasterBuildVersionString);

            // Assert
            _ = result.Build.Should().Be("20116+ci", null);
        }

        [Fact(DisplayName = "Master build should have master branch git hash")]
        public void MasterBuildShouldHaveMasterGitHash()
        {
            // Arrange

            // Act
            var result = APIVersionModel.Create(MasterBuildVersionString);

            // Assert
            _ = result.Hash.Should().Be("0adbd77", null);
        }

        [Fact(DisplayName = "Bugfix build should have GitVersion build version")]
        public void BugfixBuildShouldHaveGitVersionBuildVersion()
        {
            // Arrange

            // Act
            var result = APIVersionModel.Create(BugfixBranchVersionString);

            // Assert
            _ = result.Version.Should().Be("3.99.0", null);
        }

        [Fact(DisplayName = "Bugfix build should have bugfix branch name")]
        public void BugfixBuildShouldHaveBugfixBranchName()
        {
            // Arrange

            // Act
            var result = APIVersionModel.Create(BugfixBranchVersionString);

            // Assert
            _ = result.Branch.Should().Be("bugfix-xggt-15608-master-embed-correct-versiontxt", null);
        }

        [Fact(DisplayName = "Bugfix build should have package build number")]
        public void BugfixBuildShouldHavePackageBuildNumber()
        {
            // Arrange

            // Act
            var result = APIVersionModel.Create(BugfixBranchVersionString);

            // Assert
            _ = result.Build.Should().Be("20127+ci", null);
        }

        [Fact(DisplayName = "Bugfix build should have bugfix branch git hash")]
        public void BugfixBuildShouldHaveBugfixGitHash()
        {
            // Arrange

            // Act
            var result = APIVersionModel.Create(BugfixBranchVersionString);

            // Assert
            _ = result.Hash.Should().Be("562ce5a", null);
        }

        [Fact(DisplayName = "ToString with Bugfix build should return a sensible string")]
        public void BugfixBuildToStringShouldReturnFullVersion()
        {
            // Arrange

            // Act
            var result = APIVersionModel.Create(BugfixBranchVersionString);

            // Assert
            _ = result.ToString().Should().Be("3.99.0-20127+ci.562ce5a.bugfix-xggt-15608-master-embed-correct-versiontxt", null);
        }

        [Fact(DisplayName = "GetHash with two Bugfix builds should match")]
        public void SameTwoStringShouldHaveSameGitHash()
        {
            // Arrange
            var result1 = APIVersionModel.Create(BugfixBranchVersionString);
            var result2 = APIVersionModel.Create(BugfixBranchVersionString);

            // Act
            var hash1 = result1.GetHashCode();
            var hash2 = result2.GetHashCode();

            // Assert
            _ = hash1.Should().Be(hash2, null);
        }

        [Fact(DisplayName = "Two Bugfix builds should match")]
        public void SameTwoStringShouldMatch()
        {
            // Arrange
            var result1 = APIVersionModel.Create(BugfixBranchVersionString);
            var result2 = APIVersionModel.Create(BugfixBranchVersionString);

            // Act
            var result = result1 == result2;

            // Assert
            _ = result.Should().BeTrue(null);
        }

        [Fact(DisplayName = "Two Bugfix builds should be equal")]
        public void SameTwoTypesShouldBeEqual()
        {
            // Arrange
            var result1 = APIVersionModel.Create(BugfixBranchVersionString);
            object result2 = APIVersionModel.Create(BugfixBranchVersionString);

            // Act
            var result = result1.Equals(result2);

            // Assert
            _ = result.Should().BeTrue(null);
        }

        [Fact(DisplayName = "Different objects should not be equal")]
        public void DifferentObjectTypesShouldNotBeEqual()
        {
            // Arrange
            var result1 = APIVersionModel.Create(BugfixBranchVersionString);
            object result2 = BugfixBranchVersionString;

            // Act
            var result = result1.Equals(result2);

            // Assert
            _ = result.Should().BeFalse(null);
        }

        [Fact(DisplayName = "Master and Bugfix builds should not match")]
        public void MasterAndBugfixShouldNotMatch()
        {
            // Arrange
            var result1 = APIVersionModel.Create(MasterBuildVersionString);
            var result2 = APIVersionModel.Create(BugfixBranchVersionString);

            // Act
            var result = result1 == result2;

            // Assert
            _ = result.Should().BeFalse(null);
        }
    }
}
