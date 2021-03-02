using System;
using ImagineCommunications.GamePlan.Process.Smooth.Services;
using Xunit;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests.MockDataTests
{
    [Trait("Smooth", "Diagnostics log filename factory")]
    public class LogFilenameFactoryTests
    {
        [Theory(DisplayName = "Log file type gets the correct filename")]
        [InlineData(LogFilenameFactory.LogFileType.BestBreak, "MyRootFolder\\Logs\\25-12-2030.MySalesArea.smooth_best_break.txt")]
        [InlineData(LogFilenameFactory.LogFileType.Programmes, "MyRootFolder\\Logs\\25-12-2030.MySalesArea.smooth_programmes.txt")]
        [InlineData(LogFilenameFactory.LogFileType.SpotActions, "MyRootFolder\\Logs\\25-12-2030.MySalesArea.smooth_spot_actions.txt")]
        [InlineData(LogFilenameFactory.LogFileType.Spots, "MyRootFolder\\Logs\\25-12-2030.MySalesArea.smooth.txt")]
        public void LogFileTypeGetsTheCorrectFilename(
            LogFilenameFactory.LogFileType type,
            string expectedFilename
            )
        {
            // Arrange
            var factoryUnderTest = new LogFilenameFactory(
                "MySalesArea",
                "MyRootFolder",
                new DateTime(2030, 12, 25)
                );

            // Act
            string result = factoryUnderTest.FilenameFor(type);

            // Assert
            Assert.Equal(result, expectedFilename);
        }

        [Fact(DisplayName = "Log file type gets the correct filename for smooth configuration")]
        public void LogFileTypeGetsTheCorrectFilenameForSmoothConfiguration()
        {
            // Arrange
            var runId = Guid.NewGuid();

            var factoryUnderTest = new LogFilenameFactory(
                runId,
                "MyRootFolder",
                new DateTime(2030, 12, 25)
                );

            // Act
            string result = factoryUnderTest.FilenameFor(
                LogFilenameFactory.LogFileType.SmoothConfiguration);

            // Assert
            Assert.Equal(
                result,
                $"MyRootFolder\\Logs\\25-12-2030.{runId.ToString()}.smooth_configuration.json"
                );
        }
    }
}
