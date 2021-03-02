using System;
using System.Collections.Generic;
using System.Linq;
using xggameplan.Common;
using xggameplan.core.Interfaces;
using SpotDbObject = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Spot;
using System.Threading;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using AutoMapper.QueryableExtensions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Dto;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Services
{
    public class SpotCleaner : ISpotCleaner
    {
        private const int _spotBatchSize = 10000;
        private const int _spotDeleteBatchSize = 10000;
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public SpotCleaner(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task ExecuteAsync(IReadOnlyCollection<string> externalRefs, Action<string> logDebug, DateTime? thresholdDate = default, CancellationToken cancellationToken = default)
        {
            var dataCount = externalRefs.Count;

            var spots = new List<ReducedSpotDTO>();

            for (int i = 0; i <= dataCount / _spotBatchSize; i++)
            {
                var externalRefsBatch = externalRefs.Skip(i * _spotBatchSize).Take(_spotBatchSize).ToList();
                var spotsBatch = await _dbContext.Query<SpotDbObject>()
                    .Where(e => externalRefsBatch.Contains(e.ExternalSpotRef))
                    .ProjectTo<ReducedSpotDTO>(_mapper.ConfigurationProvider).ToArrayAsync(cancellationToken);
                spots.AddRange(spotsBatch);
            }

            logDebug?.Invoke("Start attaching Multipart Spots");

            var multipartSpots = (await _dbContext.Query<SpotDbObject>()
                .Where(e => MultipartSpotTypes.All.Contains(e.MultipartSpot))
                .ProjectTo<ReducedSpotDTO>(_mapper.ConfigurationProvider)
                .ToArrayAsync(cancellationToken))
                .Where(x => externalRefs.All(r => r != x.ExternalSpotRef) || x.MultipartSpotPosition == MultipartSpotPositions.End)
                .ToArray();

            var spotsToDelete = new List<int>();

            foreach (var spot in spots)
            {
                if (!IsMultipartSpot(spot))
                {
                    spotsToDelete.Add(spot.Id);
                    continue;
                }

                if (!IsTopMultipart(spot))
                {
                    continue;
                }

                switch (spot.MultipartSpot)
                {
                    case MultipartSpotTypes.TopTail:
                        var topTailDeletes = multipartSpots
                            .Where(x => IsMultipartSpot(x) && x.MultipartSpot == spot.MultipartSpot)
                            .Where(x => x.MultipartSpotRef == spot.MultipartSpotRef || x.ExternalSpotRef == spot.MultipartSpotRef)
                            .ToArray();

                        if (!thresholdDate.HasValue || topTailDeletes.All(e => e.StartDateTime < thresholdDate))
                        {
                            spotsToDelete.AddRange(topTailDeletes.Select(e => e.Id));
                            spotsToDelete.Add(spot.Id);
                        }

                        break;

                    case MultipartSpotTypes.SameBreak:
                        var sameBreakDeletes = multipartSpots
                            .Where(x => IsMultipartSpot(x) && x.MultipartSpot == spot.MultipartSpot)
                            .Where(x => MultipartSpotRefs(x).Contains(spot.ExternalSpotRef) ||
                                MultipartSpotRefs(spot).Contains(x.ExternalSpotRef) ||
                                MultipartSpotRefs(spot).Intersect(MultipartSpotRefs(x)).Any())
                            .ToArray();

                        if (!thresholdDate.HasValue || sameBreakDeletes.All(e => e.StartDateTime < thresholdDate))
                        {
                            spotsToDelete.AddRange(sameBreakDeletes.Select(e => e.Id));
                            spotsToDelete.Add(spot.Id);
                        }

                        break;
                }
            }

            cancellationToken.ThrowIfCancellationRequested();

            var distinctSpotsToDelete = spotsToDelete.Distinct().ToArray();

            logDebug?.Invoke($"Spots count after attaching Multipart Spots {distinctSpotsToDelete.Length}");

            Delete(distinctSpotsToDelete);
            await _dbContext.SaveChangesAsync(cancellationToken);

        }

        public void Delete(IEnumerable<int> identifiers)
        {
            if (!identifiers.Any())
            {
                return;
            }

            var loadSize = identifiers.Count();

            using (var transaction = _dbContext.Specific.Database.BeginTransaction())
            {
                for (int i = 0, page = 0; i < loadSize; i += _spotDeleteBatchSize, page++)
                {
                    var idsToDelete = identifiers.Skip(_spotDeleteBatchSize * page).Take(_spotDeleteBatchSize).ToArray();

                    _ = _dbContext.Specific.RemoveByIdentityIds<SpotDbObject>(idsToDelete);
                }

                transaction.Commit();
            }
        }

        private bool IsTopMultipart(ReducedSpotDTO spot) =>
            spot.MultipartSpot == MultipartSpotTypes.TopTail && spot.MultipartSpotPosition == MultipartSpotPositions.TopTail_Top ||
            spot.MultipartSpot == MultipartSpotTypes.SameBreak && (spot.MultipartSpotPosition == MultipartSpotPositions.SameBreak_Top ||
                                                                   spot.MultipartSpotPosition == MultipartSpotPositions.SameBreak_Any);

        private bool IsMultipartSpot(ReducedSpotDTO spot) => !String.IsNullOrWhiteSpace(spot.MultipartSpot);

        private static char[] MultipartSpotReferenceSeparator => new char[] { ',' };

        public List<string> MultipartSpotRefs(ReducedSpotDTO spot) => String.IsNullOrEmpty(spot.MultipartSpotRef)
            ? null
            : spot.MultipartSpotRef.Split(MultipartSpotReferenceSeparator).ToList();
    }
}
