using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.ProgrammeDictionaries;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Raven.Client;

namespace xggameplan.Updates
{
    internal class UpdateXGGP10302_UpdateProgrammeDictionaryDocument : CodeUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;
        private readonly string _updatesFolder;
        private readonly string _rollBackFolder;
        private readonly IMapper _mapper;

        public UpdateXGGP10302_UpdateProgrammeDictionaryDocument(IEnumerable<string> tenantConnectionStrings, string updatesFolder, IMapper mapper)
        {
            _tenantConnectionStrings = tenantConnectionStrings;
            _updatesFolder = updatesFolder;
            _mapper = mapper;
            _rollBackFolder = Path.Combine(_updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(_rollBackFolder);
        }

        public Guid Id => new Guid("60701178-902A-45E0-B87B-239FC7B184EA");

        public int Sequence => 1;

        public string Name => "XGGP-10302";

        /// <summary>
        /// Add ProgrammeName to ProgrammeDictionary
        /// </summary>
        public void Apply()
        {
            foreach (string tenantConnectionString in _tenantConnectionStrings)
            {
                using (IDocumentStore documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString))
                {
                    using (IDocumentSession session = documentStore.OpenSession())
                    {
                        var programmeDictionaries = session.GetAll<ProgrammeDictionary>();
                        var allProgrammes = session.GetAll<Programme>();
                        programmeDictionaries
                            .ForEach(programmeDictionary =>
                            {
                                var programme = allProgrammes.FirstOrDefault(p => p.ExternalReference == programmeDictionary.ExternalReference);
                                if (programme != null)
                                {
                                    programmeDictionary.ProgrammeName = programme.ProgrammeName;
                                }
                            });
                        session.SaveChanges();
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
