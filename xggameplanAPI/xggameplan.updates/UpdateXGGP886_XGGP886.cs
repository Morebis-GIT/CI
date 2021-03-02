using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using Raven.Abstractions.Data;
using Raven.Client;
using xggameplan.AutoGen;

namespace xggameplan.Updates
{
    /// <summary>
    /// Update step XGGP-886. Move defaults type rules to the general list and reset ids.
    /// </summary>
    internal class UpdateStepXGGP886_XGGP886 : CodeUpdateStepBase, IUpdateStep
    {
        private List<string> _tenantConnectionStrings;
        private string _updatesFolder;
        private string _rollBackFolder;

        public UpdateStepXGGP886_XGGP886(List<string> tenantConnectionStrings, string updatesFolder)
        {
            _tenantConnectionStrings = tenantConnectionStrings;
            _updatesFolder = updatesFolder;
            _rollBackFolder = Path.Combine(_updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(_rollBackFolder);
        }

        public Guid Id
        {
            get { return new Guid("f4126039-89bd-435a-9200-202f107f2381"); }
        }

        public int Sequence
        {
            get { return 1; }
        }

        public string Name
        {
            get { return "XGGP-886"; }
        }

        /// <summary>
        /// Converts pass
        /// </summary>
        /// <param name="pass"></param>
        /// <param name="passes"></param>
        private void ConvertPass(Pass pass, List<Pass> passes)
        {
            List<General> generalRules = new List<General>()
            {
                new General()
                {
                    Description = "Minimum Efficiency",
                    InternalType = "Defaults",
                    RuleId = RuleIDs.MinimumEfficiency,
                    Type = "rules",
                    Value = pass.Rules.Where(r => r.RuleId == 31).First().Value
                },
                new General()
                {
                    Description = "Maximum Rank",
                    InternalType = "Defaults",
                    RuleId = RuleIDs.MaximumRank,
                    Type = "rules",
                    Value = pass.Rules.Where(r => r.RuleId == 32).First().Value
                },
                new General()
                {
                    Description = "Demograph Banding Tolerance",
                    InternalType = "Defaults",
                    RuleId = RuleIDs.DemographBandingTolerance,
                    Type = "rules",
                    Value = pass.Rules.Where(r => r.RuleId == 33).First().Value
                },
                new General()
                {
                    Description = "Default Centre Break Ratio",
                    InternalType = "Defaults",
                    RuleId = RuleIDs.DefaultCentreBreakRatio,
                    Type = "rules",
                    Value = "50"
                }
            };
            pass.General = generalRules;
            List<PassRule> rules = pass.Rules.Where(r => r.InternalType != "Defaults").ToList();
            pass.Rules = rules;
            passes.Add(pass);
        }

        /// <summary>
        /// We do the following for each pass.
        /// 1) Move the defaults type rules to general list. 
        /// 2) Reset ids. 
        /// 3) Add default centre break ratio rule to general list.
        /// </summary>
        public void Apply()
        {
            foreach (string tenantConnectionString in _tenantConnectionStrings)
            {
                using (IDocumentStore documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString))
                {
                    using (IDocumentSession session = documentStore.OpenSession())
                    {
                        var passDocuments = session.Advanced.DocumentStore.DatabaseCommands.Query("Raven/DocumentsByEntityName",
                                                                 new IndexQuery
                                                                 {
                                                                     Query = "Tag:[[Passes]]",
                                                                     PageSize = 1024
                                                                 });
                        List<Pass> passes = new List<Pass>();
                        foreach (var passDocument in passDocuments.Results)
                        {
                            string passJson = passDocument.ToString();
                            Pass pass = Newtonsoft.Json.JsonConvert.DeserializeObject<Pass>(passJson);

                            var metadata = passDocument["@metadata"];
                            var idToken = metadata.SelectToken("@id");
                            pass.Id = Convert.ToInt32(idToken.ToObject<string>().Split('/')[1]);

                            if (!String.IsNullOrEmpty(_rollBackFolder))
                            {
                                string passFile = Path.Combine(_rollBackFolder, string.Format("Pass.{0}.json", pass.Id));
                                if (File.Exists(passFile))
                                {
                                    File.Delete(passFile);
                                }
                                File.WriteAllText(passFile, passJson);
                            }
                            ConvertPass(pass, passes);
                        }
                        using (IDocumentStore targetDocumentStore = DocumentStoreFactory.CreateStore(tenantConnectionString))
                        {
                            using (IDocumentSession session1 = targetDocumentStore.OpenSession())
                            {
                                passes.ForEach(pass => session1.Store(pass));
                                session1.SaveChanges();
                            }
                        }
                    }
                }
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
