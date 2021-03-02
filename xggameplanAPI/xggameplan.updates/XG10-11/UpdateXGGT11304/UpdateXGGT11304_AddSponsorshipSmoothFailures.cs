using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.SmoothFailureMessages;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using ImagineCommunications.GamePlan.Process.Smooth.Types;
using Raven.Client;
using xggameplan.Updates;

namespace xggameplan.updates
{
    internal class UpdateXGGT11304_AddSponsorshipSmoothFailures
        : PatchUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;
        private readonly string _updatesFolder;

        public UpdateXGGT11304_AddSponsorshipSmoothFailures(
            IEnumerable<string> tenantConnectionStrings,
            string updatesFolder
            )
        {
            _tenantConnectionStrings = tenantConnectionStrings;
            _updatesFolder = updatesFolder;
        }

        public Guid Id => new Guid("87CD06BF-3C74-45C2-957B-B7C4985CFA64");

        public int Sequence => 1;

        public string Name => "XGGT 11304 Add Sponsorship Smooth Failures";

        public void Apply()
        {
            foreach (string tenantConnectionString in _tenantConnectionStrings)
            {
                using (IDocumentStore documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString))
                using (IDocumentSession session = documentStore.OpenSession())
                {
                    List<SmoothFailureMessage> smoothFailures = session.GetAll<SmoothFailureMessage>();

                    if (smoothFailures.Any(sf => sf.Id == (int)SmoothFailureMessages.T1_SponsorshipRestrictionAppliedForCompetitorSpotBasedOnCompetingClash))
                    {
                        continue;
                    }

                    var inserted = new List<SmoothFailureMessage>
                    {
                        new SmoothFailureMessage{
                            Id = (int)SmoothFailureMessages.T1_SponsorshipRestrictionAppliedForCompetitorSpotBasedOnCompetingClash,
                            Description = new Dictionary<string, string>
                            {
                                ["ARA"] =  "Sponsorship Restriction Applied For Competitor Spot Based On Competing Clash",
                                ["ENG"] =  "Sponsorship Restriction Applied For Competitor Spot Based On Competing Clash",
                            }
                        },
                        new SmoothFailureMessage{
                            Id = (int)SmoothFailureMessages.T1_SponsorshipRestrictionAppliedForCompetitorSpotBasedOnCompetingAdvertiser,
                            Description = new Dictionary<string, string>
                            {
                                ["ARA"] =  "Sponsorship Restriction Applied For Competitor Spot Based On Competing Advertiser",
                                ["ENG"] =  "Sponsorship Restriction Applied For Competitor Spot Based On Competing Advertiser",
                            }
                        }
                    };

                    foreach (var item in inserted)
                    {
                        session.Store(item, $"SmoothFailureMessages/{item.Id}");
                    }

                    session.SaveChanges();
                }
            }
        }

        public bool SupportsRollback => false;

        public void RollBack() => throw new NotImplementedException();
    }
}
