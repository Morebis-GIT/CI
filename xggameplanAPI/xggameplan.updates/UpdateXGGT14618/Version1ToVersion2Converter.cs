using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects;
using Raven.Client.Listeners;
using Raven.Json.Linq;

namespace xggameplan.updates.UpdateXGGT14618
{
    internal class Version1ToVersion2Converter : IDocumentConversionListener
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
            var parameters = entity as AutoBookDefaultParameters;

            var demoDb = document["AgHfssDemo"];

            parameters.AgHfssDemos = new List<AgHfssDemo>
                {
                    new AgHfssDemo
                    {
                        SalesAreaNo = demoDb.SelectToken("SalesAreaNo").Value<int>(),
                        IndexType = demoDb.SelectToken("IndexType").Value<int>(),
                        BaseDemoNo = demoDb.SelectToken("BaseDemoNo").Value<int>(),
                        IndexDemoNo = demoDb.SelectToken("IndexDemoNo").Value<int>(),
                        BreakScheduledDate = demoDb.SelectToken("BreakScheduledDate").Value<string>()
                    }
                };
        }
    }
}
