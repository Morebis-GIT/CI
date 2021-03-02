using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using xggameplan.Updates;

namespace xggameplan.updates.UpdateXGGT13203
{
    internal class UpdateStepXGGT13203_Delivery_Currency : PatchUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionString;

        public UpdateStepXGGT13203_Delivery_Currency(IEnumerable<string> tenantConnectionString, string updatesFolder)
        {
            _tenantConnectionString = tenantConnectionString;

            var rollBackFolder = Path.Combine(updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(rollBackFolder);
        }

        public Guid Id => new Guid("FC4FE679-F43D-4C9C-862D-0904DC2C0B8F");

        public int Sequence => 1;

        public string Name => "XGGT-13203";

        public bool SupportsRollback => false;

        public void Apply()
        {
            foreach (var connectionString in _tenantConnectionString)
            {
                using (var documentStore = DocumentStoreFactory.CreateStore(connectionString, null))
                using (var session = documentStore.OpenSession())
                {
                    var campaigns = session.GetAll<Campaign>().ToList();

                    foreach (var x in campaigns)
                    {
                        x.DeliveryCurrency = x.DeliveryType == CampaignDeliveryType.Rating
                            ? DeliveryCurrency.FixedRating
                            : DeliveryCurrency.FixedSchedule;
                        session.Store(x);
                    }

                    session.SaveChanges();
                }
            }
        }

        public void RollBack() => throw new NotImplementedException();
    }
}
