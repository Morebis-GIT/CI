using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using Raven.Abstractions.Data;
using xggameplan.updates.UpdateXGGT13649;

namespace xggameplan.Updates
{
    internal class UpdateStepXGGT13649_ProductChanges : PatchUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;
        private readonly string _rollBackFolder;

        public UpdateStepXGGT13649_ProductChanges(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            _tenantConnectionStrings = tenantConnectionStrings;
            _rollBackFolder = Path.Combine(updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(_rollBackFolder);
        }

        public Guid Id => new Guid("6b1bb3d9-9f7b-4c81-9e11-bbd49f4a8601");

        public void Apply()
        {
            var idx = 0;
            //apply update to each tenant db
            foreach (var cs in _tenantConnectionStrings)
            {
                idx++;
                //Generating unique backup file name based on connection string parameters
                //The file name pattern is "product_{hostname}_{databasename}.json"
                var csBuilder = new DbConnectionStringBuilder {ConnectionString = cs};
                if (csBuilder.TryGetValue("Url", out var host))
                {
                    host = new Uri(host.ToString()).Host;
                }
                else
                {
                    host = $"host[{idx}]";
                }
                if (!csBuilder.TryGetValue("Database", out var database))
                {
                    database = $"db[{idx}]";
                }

                //if file name exists the current iteration index will be added to the end of file name
                //For instance, "product_localhost_tenantdb_1.json"
                var fileName = Path.Combine(_rollBackFolder, $"product_{host}_{database}.json");
                while (File.Exists(fileName))
                {
                    fileName = Path.Combine(_rollBackFolder, $"{Path.GetFileNameWithoutExtension(fileName)}_{idx}.json");
                }

                using (var documentStore = DocumentStoreFactory.CreateStore(cs))
                {
                    using (var session = documentStore.OpenSession())
                    {
                        //load all existing product data into memory collection using the product structure which is actual before this update
                        var prevProducts = new List<ProductPrev>();
                        using (var enumerator =
                            session.Advanced.DocumentStore.DatabaseCommands.StreamDocs(null, "products/"))
                        {
                            while (enumerator.MoveNext())
                            {
                                prevProducts.Add(
                                    Newtonsoft.Json.JsonConvert.DeserializeObject<ProductPrev>(
                                        enumerator.Current?.ToString() ?? string.Empty));
                            }
                        }

                        //save product collection into file
                        File.WriteAllText(Path.Combine(_rollBackFolder, fileName),
                            Newtonsoft.Json.JsonConvert.SerializeObject(prevProducts));

                        // Delete old products
                        _ = session.Advanced.DocumentStore.DatabaseCommands
                            .DeleteByIndex("Raven/DocumentsByEntityName", new IndexQuery { Query = "Tag:[[Products]]" })
                            .WaitForCompletion();

                        using (var targetSession = documentStore.OpenSession())
                        {
                            //save product collection using new product structure
                            prevProducts.ForEach(p =>
                            {
                                var product = new Product
                                {
                                    Uid = p.Uid,
                                    Name = p.Name,
                                    Externalidentifier = p.Externalidentifier,
                                    ParentExternalidentifier = p.ParentExternalidentifier,
                                    EffectiveStartDate = p.EffectiveStartDate,
                                    EffectiveEndDate = p.EffectiveEndDate,
                                    ClashCode = p.ClashCode,
                                    AdvertiserIdentifier = p.AdvertiserIdentifier,
                                    AdvertiserName = p.AdvertiserName,
                                    AdvertiserLinkStartDate = p.AdvertiserLinkStartDate,
                                    AdvertiserLinkEndDate = p.AdvertiserLinkEndDate,
                                    AgencyIdentifier = p.AgencyIdentifier,
                                    AgencyName = p.AgencyName,
                                    AgencyStartDate = p.AgencyStartDate,
                                    AgencyLinkEndDate = p.AgencyLinkEndDate,
                                    //new fields
                                    AgencyGroup = null,
                                    SalesExecutive = null,
                                    ReportingCategory = null
                                };
                                targetSession.Store(product);
                            });
                            targetSession.SaveChanges();
                        }
                    }
                }
            }
        }

        public void RollBack() => throw new NotImplementedException();

        public int Sequence => 1;

        public string Name => "XGGT-14246";

        public bool SupportsRollback => false;
    }
}
