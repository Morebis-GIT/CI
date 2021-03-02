using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using TechTalk.SpecFlow;
using xgCore.xgGamePlan.ApiEndPoints.Models.Sponsorships;

namespace xgCore.xgGamePlan.AutomationTests.Coordinators
{
    [Binding]
    public class SponsorshipsCoordinator
    {
        private readonly IFixture _fixture;

        public SponsorshipsCoordinator(IFixture fixture)
        {
            _fixture = fixture;
        }

        public IEnumerable<CreateSponsorshipModel> CreateSponsorshipModel(int count, IEnumerable<string> productExtIdentifiers, string salesAreaName,
            string advertiserIdentifier, string сlashExternalref)
        {
            CusomizeFixture(productExtIdentifiers, salesAreaName, advertiserIdentifier, сlashExternalref);

            var createSponsorshipModels = _fixture.CreateMany<CreateSponsorshipModel>(count);
            foreach (var createSponsorshipModel in createSponsorshipModels)
            {
                createSponsorshipModel.ExternalReferenceId = Guid.NewGuid().ToString();
            }

            return createSponsorshipModels;   
        }

        public CreateSponsoredItemModel CreateSponsoredItemModels(IEnumerable<string> productExtIdentifiers, string salesAreaName, string advertiserIdentifier, string сlashExternalref)
        {
            CusomizeFixture(productExtIdentifiers, salesAreaName, advertiserIdentifier, сlashExternalref);
            return _fixture.Create<CreateSponsoredItemModel>();
        }

        private void CusomizeFixture(IEnumerable<string> productExtIdentifiers, string salesAreaName, string advertiserIdentifier, string сlashExternalref)
        {
            _fixture.Customize<CreateClashExclusivityModel>(ob => ob
                .With(x => x.ClashExternalRef, сlashExternalref)
                .Without(x => x.RestrictionType)
                .Without(x => x.RestrictionValue)
            );

            _fixture.Customize<CreateAdvertiserExclusivityModel>(ob => ob
                .With(x => x.AdvertiserIdentifier, advertiserIdentifier)
                .Without(x => x.RestrictionType)
                .Without(x => x.RestrictionValue)
            );

            _fixture.Customize<CreateSponsoredDayPartModel>(ob => ob
                .With(x => x.DaysOfWeek,
                    () => { return _fixture.Create<IEnumerable<DayOfWeek>>().Select(x => x.ToString()).Take(7).ToArray(); })
            );

            _fixture.Customize<CreateSponsorshipItemModel>(ob => ob
                .With(x => x.SalesAreas, new[] {salesAreaName})
                .With(x => x.StartDate, DateTime.Today.AddDays(3))
                .With(x => x.EndDate, DateTime.Today.AddDays(6))
                .Without(x => x.ProgrammeName)
            );

            _fixture.Customize<CreateSponsoredItemModel>(ob => ob
                .With(x => x.Products, productExtIdentifiers)
                .Without(x => x.RestrictionType)
                .Without(x => x.RestrictionValue)
                .With(x => x.CalculationType, SponsorshipCalculationType.None)
                .Without(x => x.Applicability)
            );

            _fixture.Customize<CreateSponsorshipModel>(b => b
                .With(a => a.ExternalReferenceId, _fixture.Create<string>)
                .With(a => a.RestrictionLevel, SponsorshipRestrictionLevel.TimeBand)
            );
        }
    }
}
