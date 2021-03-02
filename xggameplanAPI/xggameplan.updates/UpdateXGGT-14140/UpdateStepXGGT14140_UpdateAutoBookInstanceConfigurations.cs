using System;
using System.Collections.Generic;
using System.IO;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.InstanceConfiguration.Objects;
using ImagineCommunications.GamePlan.Domain.AWSInstanceConfigurations;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Raven.Client;

namespace xggameplan.Updates
{
    internal class UpdateStepXGGT14140_UpdateAutoBookInstanceConfigurations : PatchUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;
        private readonly string _updatesFolder;
        private readonly string _rollBackFolder;

        public UpdateStepXGGT14140_UpdateAutoBookInstanceConfigurations(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            ValidateParametersBeforeUse(tenantConnectionStrings, updatesFolder);

            _tenantConnectionStrings = tenantConnectionStrings;
            _updatesFolder = updatesFolder;
            _rollBackFolder = Path.Combine(_updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(_rollBackFolder);
        }

        public Guid Id => new Guid("e8e376f4-85bb-4f5d-b1e8-5e826403a2d5");

        public void Apply()
        {
            foreach (string tenantConnectionString in _tenantConnectionStrings)
            {
                using (IDocumentStore documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString))
                using (IDocumentSession session = documentStore.OpenSession())
                {
                    var awsconfigs = session.GetAll<AWSInstanceConfiguration>();
                    var autobookconfigs = session.GetAll<AutoBookInstanceConfiguration>();

                    foreach (AutoBookInstanceConfiguration abic in autobookconfigs)
                    {
                        var aws = awsconfigs.Find(x => x.Id == abic.Id);
                        if (aws != null)
                        {
                            abic.InstanceType = aws.InstanceType;
                            abic.StorageSizeGb = aws.StorageSizeGb;
                            abic.Cost = aws.Cost;
                        }
                    }

                    awsconfigs.ForEach(aws => session.Delete(aws));

                    session.SaveChanges();
                }
            }
        }

        public void RollBack()
        {
            throw new NotImplementedException();
        }

        public int Sequence => 1;

        public string Name => "XGGT-14140: Update AutoBookInstanceConfigurations from AWSInstanceConfiguration and delete AWSInstanceConfigurations";

        public bool SupportsRollback => false;

        private static void ValidateParametersBeforeUse(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            _ = UpdateValidator.ValidateTenantConnectionString(tenantConnectionStrings, true);
            _ = UpdateValidator.ValidateUpdateFolderPath(updatesFolder, true);
        }
    }
}
