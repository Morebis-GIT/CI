using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using Raven.Client;

namespace xggameplan.Updates
{
    /// <summary>
    /// Updates for XGGP-791, applies patch to Pass documents to fix spelling mistakes
    /// </summary>
    internal class UpdateXGGP509_XGGP791 : PatchUpdateStepBase, IUpdateStep
    {
        private List<string> _tenantConnectionStrings;
        private string _updatesFolder;        

        public UpdateXGGP509_XGGP791(List<string> tenantConnectionStrings, string updatesFolder)
        {
            _tenantConnectionStrings = tenantConnectionStrings;
            _updatesFolder = updatesFolder;            
        }

        public Guid Id
        {
            get { return new Guid("2bbcb808-18be-402d-b636-f6fc9b51c4d3"); }
        }

        public int Sequence
        {
            get { return 1; }
        }

        public string Name
        {
            get { return "XGGP-791"; }
        }

        public void Apply()
        {
            foreach (string tenantConnectionString in _tenantConnectionStrings)
            {
                using (IDocumentStore documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString))
                {
                    using (IDocumentSession session = documentStore.OpenSession())
                    {
                        ExecutePatchScriptByIndexQuery(session, "Raven/DocumentsByEntityName", "Tag:[[Passes]]", ScriptToFixPasses);                        
                    }
                }
            }
        }

        /// <summary>
        /// Script to fix Pass documents, spelling mistakes in descriptions
        /// </summary>
        private static string ScriptToFixPasses
        {
            get
            {                
                return "for (var i = 0; i < this.Weightings.length; i++) " +
                    "{ " +
                     "   if (this.Weightings[i].Description == 'Fit to Spot Lengt') " +
                     "   { " +
                     "       this.Weightings[i].Description = 'Fit to Spot Length'; " +
                     "   } " +
                    "} " +
                    "for (var i = 0; i < this.Rules.length; i++) " +
                    "{ " +
                    "    if (this.Rules[i].Description == 'Miniumum Break Availability') " +
                    "    { " +
                    "        this.Rules[i].Description = 'Minimum Break Availability'; " +
                    "    } " +
                    "}";                
            }
        }

        public bool SupportsRollback
        {
            get { return false; }
        }

        public void RollBack()
        {
            throw new NotImplementedException();
        }
    }
}
