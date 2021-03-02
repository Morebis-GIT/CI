using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions.Objects;
using ImagineCommunications.GamePlan.Process.Smooth.Dtos;
using ImagineCommunications.GamePlan.Process.Smooth.Types;

namespace ImagineCommunications.GamePlan.Process.Smooth.Models
{
    /// <summary>
    /// A collection of Smooth failures for a spot.
    /// </summary>
    internal class SmoothFailureMessagesForSpotsCollection
        : IEnumerable<KeyValuePair<Guid, SmoothFailuresForSpotCollection>>
    {
        private readonly IDictionary<Guid, SmoothFailuresForSpotCollection> _results
            = new Dictionary<Guid, SmoothFailuresForSpotCollection>();

        public SmoothFailureMessagesForSpotsCollection() { }

        internal SmoothFailureMessagesForSpotsCollection(SmoothBreak smoothBreak)
            : this() => SmoothBreak = smoothBreak;

        internal SmoothBreak SmoothBreak { get; }

        /// <summary>
        /// Add a <see cref="SmoothFailureMessages"/> for a Spot to the result.
        /// </summary>
        internal void Add(
            Guid spotUid,
            SmoothFailureMessages failureMessage)
        {
            InitialiseForSpot(spotUid);

            var failuresCollection = _results[spotUid].Failures;
            failuresCollection.Add(
                new SmoothFailureAndReasonForFailure(failureMessage)
                );
        }

        /// <summary>
        /// Add a <see cref="SmoothFailureMessages"/> for a Spot to the result.
        /// </summary>
        internal void Add(
            Guid spotUid,
            SmoothFailureMessages failureMessage,
            Restriction restriction)
        {
            InitialiseForSpot(spotUid);

            var failuresCollection = _results[spotUid].Failures;
            failuresCollection.Add(
                new SmoothFailureAndReasonForFailure(failureMessage, restriction)
                );
        }

        [Pure]
        internal SmoothFailuresForSpotCollection this[Guid spotUid] => _results[spotUid];

        /// <summary>
        /// Initialise a failures collection for a spot.
        /// </summary>
        /// <param name="spotUid"></param>
        internal void InitialiseForSpot(Guid spotUid)
        {
            if (_results.ContainsKey(spotUid))
            {
                return;
            }

            _results.Add(
                spotUid,
                new SmoothFailuresForSpotCollection(spotUid));
        }

        /// <summary>
        /// So we can use this collection in a <see langword="foreach"/> statement.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<Guid, SmoothFailuresForSpotCollection>> GetEnumerator() =>
            ((IReadOnlyDictionary<Guid, SmoothFailuresForSpotCollection>)_results).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            ((IEnumerable)_results).GetEnumerator();
    }
}
