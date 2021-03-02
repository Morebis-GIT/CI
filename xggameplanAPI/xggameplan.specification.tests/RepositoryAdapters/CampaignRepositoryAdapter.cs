using System;
using System.Collections.Generic;
using System.Globalization;
using AutoFixture;
using AutoFixture.Dsl;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Campaigns.Queries;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using xggameplan.specification.tests.Infrastructure.RepositoryMethod;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class CampaignRepositoryAdapter : RepositoryTestAdapter<Campaign, ICampaignRepository, Guid>
    {
        private static readonly string[] CampaignStatuses = { "A", "C", "N" };
        private static readonly string[] CategoryOrProgrammeValues = { "C", "P" };
        private static readonly string[] IncludeOrExcludeValues = { "I", "E" };

        private readonly Random _randomizer = new Random((int)DateTime.UtcNow.Ticks);

        public CampaignRepositoryAdapter(IScenarioDbContext dbContext, ICampaignRepository repository) : base(dbContext, repository)
        {
            Fixture.Customize<CampaignProgramme>(composer => composer
                    .With(p => p.CategoryOrProgramme, Fixture.CreateMany<string>(3))
                    .With(p => p.SalesAreas, Fixture.CreateMany<string>(3))
                );

            Fixture.Customize<ProgrammeRestriction>(composer => composer
                .With(p => p.IsCategoryOrProgramme, CategoryOrProgrammeValues[_randomizer.Next(0, 2)])
                .With(p => p.IsIncludeOrExclude, IncludeOrExcludeValues[_randomizer.Next(0, 2)]));

            Fixture.Customize<TimeRestriction>(composer => composer
                .With(p => p.IsIncludeOrExclude, IncludeOrExcludeValues[_randomizer.Next(0, 2)])
                .With(p => p.DowPattern, new List<string> { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" }));

            Fixture.Customize<Timeslice>(composer => composer
                .With(p => p.FromTime, DateTime.UtcNow.ToString("HH:mm", CultureInfo.InvariantCulture))
                .With(p => p.ToTime, DateTime.UtcNow.ToString("HH:mm", CultureInfo.InvariantCulture))
                .With(p => p.DowPattern, new List<string> { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" }));
        }

        protected override Campaign Add(Campaign model)
        {
            _ = Repository.Add(model);
            return model;
        }

        protected override IEnumerable<Campaign> AddRange(params Campaign[] models)
        {
            Repository.Add(models);
            return models;
        }

        protected override Campaign Update(Campaign model)
        {
            Repository.Update(model);
            return model;
        }

        protected override Campaign GetById(Guid id)
        {
            return Repository.Get(id);
        }

        protected override IEnumerable<Campaign> GetAll()
        {
            return Repository.GetAll();
        }

        protected override void Delete(Guid id)
        {
            Repository.Delete(id);
        }

        protected override void Truncate()
        {
            Repository.TruncateAsync().Wait();
        }

        protected override int Count()
        {
            return Repository.CountAll;
        }

        protected override IPostprocessComposer<Campaign> GetAutoModelComposer()
        {
            return base.GetAutoModelComposer()
                .With(p => p.Status, CampaignStatuses[_randomizer.Next(0, 2)]);
        }

        [RepositoryMethod]
        protected CallMethodResult GetWithProduct(CampaignStatus status, DateTime? startDate, DateTime? endDate, string description)
        {
            var queryModel = new CampaignSearchQueryModel
            {
                Status = status,
                StartDate = startDate ?? default,
                EndDate = endDate ?? default,
                Description = description
            };
            var res = Repository.GetWithProduct(queryModel);

            TestContext.LastOperationCount = res?.Items?.Count ?? 0;
            TestContext.LastCollectionResult = res?.Items;
            TestContext.LastSingleResult = null;

            return CallMethodResult.CreateHandled();
        }
    }
}
