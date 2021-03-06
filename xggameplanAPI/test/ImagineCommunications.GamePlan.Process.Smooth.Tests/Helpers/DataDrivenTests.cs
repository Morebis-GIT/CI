﻿using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.CommonTestsHelpers;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Domain.Recommendations.Objects;
using ImagineCommunications.GamePlan.Domain.SmoothFailures;
using ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers;
using ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers.CollectionFixtures;
using Xunit.Abstractions;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests
{
    public abstract class DataDrivenTests
        : IDisposable
    {
        protected readonly ITestOutputHelper _output;
        private readonly SmoothFixture _smoothFixture;

        protected SmoothTest DataLoader { get; private set; }

        /// <summary>
        /// Base class for tests that require data loaded from on-disk JSON files.
        /// </summary>
        /// <param name="loadAllCoreData">
        /// When <see langword="true"/>, load all core data, otherwise only load
        /// settings and metadata.
        /// </param>
        protected DataDrivenTests(
            SmoothFixture smoothFixture,
            ITestOutputHelper output,
            bool loadAllCoreData = true)
        {
            _output = output;
            _smoothFixture = smoothFixture;

            DataLoader = new SmoothTest(
                _smoothFixture.RepositorySegmentationSalt,
                _smoothFixture.ObjectTTL,
                _output,
                loadAllCoreData);

            if (_smoothFixture is null)
            {
                return;
            }

            output.WriteLine(
                "Repository segmentation salt => " +
                _smoothFixture.RepositorySegmentationSalt.ToString()
                );
        }

        protected virtual string SalesArea => "NWS91";

        protected virtual DateTimeRange SmoothPeriod => (
            start: new DateTime(2018, 7, 30, 0, 0, 0),
            end: new DateTime(2018, 7, 30, 23, 59, 59)
        );

        protected IRepositoryFactory RepositoryFactory => DataLoader.RepositoryFactory;

        protected void ActThen(Action<SmoothTestResults> checkTestResults)
        {
            void PackageResults(
                object sender,
                DateTime startDate,
                DateTime endDate,
                IReadOnlyList<Recommendation> recommendations,
                IReadOnlyList<SmoothFailure> smoothFailures
                )
            {
                checkTestResults(new SmoothTestResults()
                {
                    Sender = sender,
                    StartDate = startDate,
                    EndDate = endDate,
                    Recommendations = recommendations,
                    SmoothFailures = smoothFailures
                });
            }

            try
            {
                DataLoader.OnSmoothBatchComplete += PackageResults;

                using var timer = new OperationTimer(_output);
                _ = DataLoader.Execute(SalesArea, SmoothPeriod);
            }
            finally
            {
                DataLoader.OnSmoothBatchComplete -= PackageResults;
            }
        }

        protected void WriteTestCode(string testCode)
        {
            _output.WriteLine("======================");
            _output.WriteLine($"Test case: {testCode}");
            _output.WriteLine("======================");
        }

        protected void DumpRecommentationsToDebug(
            IReadOnlyCollection<Recommendation> recommendations
            )
        {
            _output.WriteLine("--------------------");
            _output.WriteLine("ExtBreakNo | ExternalSpotRef | Actual position in break | Product | Sponsored | Pass Iteration Sequence | Preempt level");

            foreach (var item in recommendations
                .OrderBy(r => r.ExternalBreakNo)
                .ThenBy(r => r.ActualPositionInBreak)
                .Select(r => new
                {
                    ExternalBreakNo = r.ExternalBreakNo ?? "<NotPlaced>",
                    r.ExternalSpotRef,
                    r.ActualPositionInBreak,
                    r.Product,
                    Sponsored = r.Sponsored.ToString(),
                    PassIterationSequence = r.PassIterationSequence.ToString(),
                    Preemptlevel = r.Preemptlevel.ToString()
                })
                )
            {
                _output.WriteLine($"{item.ExternalBreakNo} | {item.ExternalSpotRef} | {item.ActualPositionInBreak} | {item.Product} | {item.Sponsored} |  {item.PassIterationSequence} | {item.Preemptlevel}");
            }

            _output.WriteLine("--------------------");
        }

        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    DataLoader.Dispose();
                    DataLoader = null;
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
