using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using NodaTime;
using Raven.Client.Listeners;
using Raven.Json.Linq;
using xggameplan.Updates;

namespace xggameplan.updates.UpdateXGGT13239
{
    internal class UpdateStepXGGT13239_Change_Multipart_length : PatchUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;

        public UpdateStepXGGT13239_Change_Multipart_length(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            _tenantConnectionStrings = tenantConnectionStrings;

            var rollBackFolder = Path.Combine(updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(rollBackFolder);
        }

        public Guid Id => new Guid("E9E7FCD7-73C9-40CA-8230-0427A6FB5129");

        public int Sequence => 1;

        public string Name => "XGGT-13239";

        public bool SupportsRollback => false;

        public void Apply()
        {
            foreach (var connectionString in _tenantConnectionStrings)
            {
                using (var documentStore = DocumentStoreFactory.CreateStore(connectionString, null))
                {
                    documentStore.Listeners.RegisterListener(new MultipartLengthVersion1ToVersion2Converter());
                    using (var session = documentStore.OpenSession())
                    {
                        var campaigns = session.GetAll<Campaign>().ToList();

                        foreach (var x in campaigns)
                        {
                            session.Store(x);
                        }

                        session.SaveChanges();
                    }
                }
               
            }
        }

        public void RollBack() => throw new NotImplementedException();
    }

    internal class MultipartLengthVersion1ToVersion2Converter : IDocumentConversionListener
    {
        public void BeforeConversionToDocument(string key, object entity, RavenJObject metadata)
        {
        }

        public void AfterConversionToDocument(string key, object entity, RavenJObject document, RavenJObject metadata)
        {
        }

        public void BeforeConversionToEntity(string key, RavenJObject document, RavenJObject metadata)
        {
        }

        public void AfterConversionToEntity(string key, RavenJObject document, RavenJObject metadata, object entity)
        {
            var campaign = entity as Campaign;

            var i = 0;
            foreach (var ct in campaign.SalesAreaCampaignTarget)
            {
                var j = 0;
                foreach (var m in ct.Multiparts)
                {
                    var k = 0;
                    foreach (var l in m.Lengths)
                    {
                        var ticks = document["SalesAreaCampaignTarget"]
                            .Values().ToArray()[i]
                            .SelectToken("Multiparts")
                                .Values().ToArray()[j]
                                .SelectToken("Lengths")
                                    .Values().ToArray()[k]
                                    .SelectToken("ticks")
                                        .Value<int>();

                        l.Length = Duration.FromTicks(ticks);
                        k++;
                    }
                    j++;
                }
                i++;
            }
        }
    }
}
