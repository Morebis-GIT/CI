using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Caches;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using xggameplan.specification.tests.Interfaces;
using ProgrammeEpisodeEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Programmes.ProgrammeEpisode;

namespace xggameplan.specification.tests.Infrastructure.SqlServer.DomainModelHandlers
{
    /// <summary>
    /// ProgrammeEpisode entity has a reference to ProgrammeDictionary, ProgrammeDictionaryCache is used to create episodes.
    /// </summary>
    public class
        ProgrammeEpisodeDomainModelHandler : SimpleDomainModelMappingHandler<ProgrammeEpisodeEntity, ProgrammeEpisode>
    {
        private readonly IMapper _mapper;
        private readonly ProgrammeDictionaryCache _programmeDictionaryCache;

        public ProgrammeEpisodeDomainModelHandler(
            ISqlServerTestDbContext dbContext,
            IMapper mapper)
            : base(dbContext, mapper)
        {
            _mapper = mapper;
            _programmeDictionaryCache = new ProgrammeDictionaryCache(dbContext);
        }

        public override ProgrammeEpisode Add(ProgrammeEpisode model)
        {
            var programmeDictionary = _programmeDictionaryCache.GetOrAdd(model.ProgrammeExternalReference,
                key => new ProgrammeDictionary
                {
                    ExternalReference = key,
                    Name = $"{nameof(ProgrammeDictionary)}_{key}",
                });

            var episode = new ProgrammeEpisodeEntity
            {
                Name = model.Name,
                Number = model.Number,
                ProgrammeDictionary = programmeDictionary
            };

            programmeDictionary.ProgrammeEpisodes.Add(episode);

            return model;
        }

        public override void AddRange(params ProgrammeEpisode[] models)
        {
            foreach (var model in models)
            {
                _ = Add(model);
            }
        }
    }
}
