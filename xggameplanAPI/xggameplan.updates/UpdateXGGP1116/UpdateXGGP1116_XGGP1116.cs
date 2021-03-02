using System;
using System.Collections.Generic;
using System.IO;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories;

namespace xggameplan.Updates
{
    internal class UpdateXGGP1116_XGGP1116 : CodeUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;
        private readonly string _updatesFolder;
        private readonly string _rollBackFolder;
        private readonly IMapper _mapper;

        public UpdateXGGP1116_XGGP1116(IEnumerable<string> tenantConnectionStrings, string updatesFolder, IMapper mapper)
        {
            _tenantConnectionStrings = tenantConnectionStrings;
            _updatesFolder = updatesFolder;
            _mapper = mapper;
            _rollBackFolder = Path.Combine(_updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(_rollBackFolder);
        }

        public Guid Id => new Guid("1de0d1cb-abcc-4891-b630-8ecc1a28d379");

        public int Sequence => 1;

        public string Name => "XGGP-1116";

        /// <summary>
        /// Added run restrictions for tenant settings
        /// </summary>
        public void Apply()
        {
            foreach (var tenantConnectionString in _tenantConnectionStrings)
            {
                using (var documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString))
                using (var session = documentStore.OpenSession())
                {
                    var tenantSettingsRepository = new RavenTenantSettingsRepository(session, _mapper);

                    var tenantSettings = tenantSettingsRepository.Get();

                    if (tenantSettings is null)
                    {
                        throw new Exception("Tenant's settings cannot be empty");
                    }

                    if (tenantSettings.RunRestrictions == null)
                    {
                        tenantSettings.RunRestrictions = new RunRestrictions
                        {
                            MinDocRestriction = new MinimumDocumentRestriction()
                            {
                                Campaigns = 1,
                                Clashes = 1,
                                ClearanceCodes = 1,
                                Demographics = 1,
                                Products = 1
                            },
                            MinRunSizeDocRestriction = new MinimumRunSizeDocumentRestriction()
                            {
                                Breaks = 1,
                                Programmes = 1,
                                Spots = 1
                            }
                        };
                        tenantSettingsRepository.SaveChanges();
                    }
                }
            }
        }

        public bool SupportsRollback => false;

        public void RollBack()
        {
            throw new NotImplementedException();
        }
    }
}
