using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using Newtonsoft.Json;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Json.Linq;

namespace xggameplan.Updates
{
    internal class UpdateStepXGGT7600_MapCampaignPassPriority : PatchUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;
        private readonly string _updatesFolder;
        private readonly string _rollBackFolder;
        private static readonly JsonSerializerSettings _settings = GetSerializerSettings();

        public UpdateStepXGGT7600_MapCampaignPassPriority(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            ValidateParametersBeforeUse(tenantConnectionStrings, updatesFolder);

            _tenantConnectionStrings = tenantConnectionStrings;
            _updatesFolder = updatesFolder;
            _rollBackFolder = Path.Combine(_updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(_rollBackFolder);
        }

        public Guid Id => new Guid("29885a80-1086-44ef-9b54-7319bd35adcd");

        public void Apply()
        {
            foreach (string tenantConnectionString in _tenantConnectionStrings)
            {
                using (IDocumentStore documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString))
                using (IDocumentSession session = documentStore.OpenSession())
                {
                    var scenarioDocuments = session.Advanced.DocumentStore.DatabaseCommands.Query(
                        "Raven/DocumentsByEntityName",
                        new IndexQuery
                        {
                            Query = "Tag:Scenarios*",
                            PageSize = 1024
                        });

                    UpdateCampaignPassPriorities(scenarioDocuments.Results, session);
                }
            }
        }

        public void RollBack()
        {
            throw new NotImplementedException();
        }

        public int Sequence => 1;

        public string Name => "XGGP-7600 : Map Campaign Pass Priority";

        public bool SupportsRollback => false;

        private static void ValidateParametersBeforeUse(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            _ = UpdateValidator.ValidateTenantConnectionString(tenantConnectionStrings, true);
            _ = UpdateValidator.ValidateUpdateFolderPath(updatesFolder, true);
        }

        private void UpdateCampaignPassPriorities(List<RavenJObject> currentScenarios, IDocumentSession session)
        {
            foreach (var scenario in currentScenarios)
            {
                var campaignPassPriorities = scenario.Value<RavenJArray>("CampaignPassPriorities");

                if (campaignPassPriorities.Length > 0)
                {
                    var parsedScenario = JsonConvert.DeserializeObject<Scenario>(scenario.ToString(), _settings);
                    parsedScenario.Id = GetId(scenario);

                    foreach (var campaignPassPriority in campaignPassPriorities)
                    {
                        var campaign = campaignPassPriority.Value<RavenJObject>("Campaign");
                        var campaignId = campaign.Value<Guid>("Uid");
                        var campaignDefaultCampaignPassPriority = campaign.Value<string>("DefaultCampaignPassPriority");

                        var passPriorities = campaignPassPriority.Value<RavenJArray>("PassPriorities");
                        var newCampaignPassPriorities =
                            parsedScenario.CampaignPassPriorities.FirstOrDefault(pp => pp.Campaign.Uid == campaignId);

                        newCampaignPassPriorities.Campaign.DefaultCampaignPassPriority =
                            ConvertPassPriority(campaignDefaultCampaignPassPriority);

                        foreach (var passPriority in passPriorities)
                        {
                            var passId = passPriority.Value<int>("PassId");
                            var priority = passPriority.Value<string>("Priority");

                            newCampaignPassPriorities
                                .PassPriorities.FirstOrDefault(p => p.PassId == passId).Priority = ConvertPassPriority(priority);
                        }
                    }

                    session.Store(parsedScenario);
                }
            }
            session.SaveChanges();
        }

        private static int ConvertPassPriority(string passPriority)
        {
            switch (passPriority)
            {
                case "Exclude":
                   return (int)PassPriorityType.Exclude;
                case "Priority1":
                    return 1;
                case "Priority2":
                    return 2;
                case "Priority3":
                    return 3;
                default:
                    return (int)PassPriorityType.Include;
            }
        }

        private static JsonSerializerSettings GetSerializerSettings()
        {
            return new JsonSerializerSettings
            {
                Error = (sender, args) =>
                {
                    if (Equals(args.ErrorContext.Member, "DefaultCampaignPassPriority") && args.ErrorContext.OriginalObject.GetType() == typeof(CompactCampaign)
                        || Equals(args.ErrorContext.Member, "Priority") && args.ErrorContext.OriginalObject.GetType() == typeof(PassPriority))
                    {
                        args.ErrorContext.Handled = true;
                    }
                }
            };
        }

        private static Guid GetId(RavenJObject obj)
        {
            var metadata = obj["@metadata"];
            var idToken = metadata.SelectToken("@id");
            return new Guid(idToken.ToObject<string>().Split('/')[1]);
        }
    }
}
