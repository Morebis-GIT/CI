using System;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes
{
    public class CampaignWithProduct : AbstractMultiMapIndexCreationTask<CampaignWithProduct.Result>
    {
        public class Result
        {
            public string Id { get; set; }
            public string Status { get; set; }
            public string Name { get; set; }
            public string ExternalId { get; set; }
            public string CampaignGroup { get; set; }
            public DateTime? StartDateTime { get; set; }
            public DateTime? EndDateTime { get; set; }
            public string ProductExternalRef { get; set; }
            public string ProductName { get; set; }
            public string AdvertiserName { get; set; }
            public string AgencyName { get; set; }
            public string BusinessType { get; set; }
            public string Demographic { get; set; }
            public double? RevenueBudget { get; set; }
            public decimal? TargetRatings { get; set; }
            public decimal? ActualRatings { get; set; }
            public bool? IsPercentage { get; set; }
            public bool? IncludeOptimisation { get; set; }
            public bool? TargetZeroRatedBreaks { get; set; }
            public bool? InefficientSpotRemoval { get; set; }
            public bool? IncludeRightSizer { get; set; }
            public string RightSizerLevel { get; set; }
            public string[] SearchFields { get; set; }
            public int? DefaultCampaignPassPriority { get; set; }
            public string ClashCode { get; set; }
        }

        public static string DefaultIndexName => "CampaignWithProduct";

        public CampaignWithProduct()
        {
            AddMap<Campaign>(campaigns => from c in campaigns
                                          select new Result()
                                          {
                                              ActualRatings = c.ActualRatings,
                                              BusinessType = c.BusinessType,
                                              CampaignGroup = c.CampaignGroup,
                                              Demographic = c.DemoGraphic,
                                              EndDateTime = c.EndDateTime,
                                              ExternalId = c.ExternalId,
                                              IncludeOptimisation = c.IncludeOptimisation,
                                              TargetZeroRatedBreaks = c.TargetZeroRatedBreaks,
                                              IncludeRightSizer = c.IncludeRightSizer,
                                              RightSizerLevel = c.RightSizerLevel.HasValue ? c.RightSizerLevel.Value.ToString() : null,
                                              InefficientSpotRemoval = c.InefficientSpotRemoval,
                                              IsPercentage = c.IsPercentage,
                                              Name = c.Name,
                                              ProductExternalRef = c.Product,
                                              RevenueBudget = c.RevenueBudget,
                                              StartDateTime = c.StartDateTime,
                                              Status = c.Status,
                                              TargetRatings = c.TargetRatings,
                                              Id = c.Id.ToString(),
                                              AdvertiserName = null,
                                              AgencyName = null,
                                              ProductName = null,
                                              SearchFields = null,
                                              DefaultCampaignPassPriority = c.CampaignPassPriority,
                                              ClashCode = null
                                          });

            AddMap<Product>(products => from p in products
                                        select new Result()
                                        {
                                            ActualRatings = null,
                                            BusinessType = null,
                                            CampaignGroup = null,
                                            Demographic = null,
                                            EndDateTime = (DateTime?)null,
                                            ExternalId = null,
                                            IncludeOptimisation = (bool?)null,
                                            TargetZeroRatedBreaks = (bool?)null,
                                            IncludeRightSizer = (bool?)null,
                                            RightSizerLevel = null,
                                            InefficientSpotRemoval = (bool?)null,
                                            IsPercentage = (bool?)null,
                                            Name = null,
                                            ProductExternalRef = p.Externalidentifier,
                                            RevenueBudget = (int?)null,
                                            StartDateTime = (DateTime?)null,
                                            Status = null,
                                            TargetRatings = (int?)null,
                                            Id = null,
                                            AdvertiserName = p.AdvertiserName,
                                            AgencyName = p.AgencyName,
                                            ProductName = p.Name,
                                            SearchFields = null,
                                            DefaultCampaignPassPriority = null,
                                            ClashCode = p.ClashCode
                                        });

            Reduce = results => from result in results
                                group result by result.ProductExternalRef
                                into g
                                select new Result()
                                {
                                    ActualRatings = g.Select(x => x.ActualRatings).FirstOrDefault(x => x != null),
                                    AdvertiserName = g.Select(x => x.AdvertiserName).FirstOrDefault(x => x != null),
                                    AgencyName = g.Select(x => x.AgencyName).FirstOrDefault(x => x != null),
                                    BusinessType = g.Select(x => x.BusinessType).FirstOrDefault(x => x != null),
                                    CampaignGroup = g.Select(x => x.CampaignGroup).FirstOrDefault(x => x != null),
                                    Demographic = g.Select(x => x.Demographic).FirstOrDefault(x => x != null),
                                    EndDateTime = g.Select(x => x.EndDateTime).FirstOrDefault(x => x != null),
                                    ExternalId = g.Select(x => x.ExternalId).FirstOrDefault(x => x != null),
                                    IncludeOptimisation = g.Select(x => x.IncludeOptimisation).FirstOrDefault(x => x != null),
                                    TargetZeroRatedBreaks = g.Select(x => x.TargetZeroRatedBreaks).FirstOrDefault(x => x != null),
                                    IncludeRightSizer = g.Select(x => x.IncludeRightSizer).FirstOrDefault(x => x != null),
                                    RightSizerLevel = g.Select(x => x.RightSizerLevel).FirstOrDefault(x => x != null),
                                    InefficientSpotRemoval = g.Select(x => x.InefficientSpotRemoval).FirstOrDefault(x => x != null),
                                    IsPercentage = g.Select(x => x.IsPercentage).FirstOrDefault(x => x != null),
                                    Name = g.Select(x => x.Name).FirstOrDefault(x => x != null),
                                    ProductExternalRef = g.Key,
                                    ProductName = g.Select(x => x.ProductName).FirstOrDefault(x => x != null),
                                    RevenueBudget = g.Select(x => x.RevenueBudget).FirstOrDefault(x => x != null),
                                    StartDateTime = g.Select(x => x.StartDateTime).FirstOrDefault(x => x != null),
                                    Status = g.Select(x => x.Status).FirstOrDefault(x => x != null),
                                    TargetRatings = g.Select(x => x.TargetRatings).FirstOrDefault(x => x != null),
                                    Id = g.Select(x => x.Id).FirstOrDefault(x => x != null),
                                    SearchFields = new[] {
                                        g.Select(x => x.CampaignGroup).FirstOrDefault(x => x != null) ,
                                        g.Select(x => x.Name).FirstOrDefault(x => x != null),
                                        g.Select(x => x.ExternalId).FirstOrDefault(x => x != null),
                                        g.Select(x => x.BusinessType).FirstOrDefault(x => x != null),
                                        g.Select(x => x.AgencyName).FirstOrDefault(x => x != null),
                                        g.Select(x => x.AdvertiserName).FirstOrDefault(x => x != null)
                                    },
                                    DefaultCampaignPassPriority = g.Select(x => x.DefaultCampaignPassPriority).FirstOrDefault(x => x != null),
                                    ClashCode = g.Select(x => x.ClashCode).FirstOrDefault(x => x != null)
                                };

            Index(c => c.SearchFields, FieldIndexing.Analyzed);
            Index(c => c.Demographic, FieldIndexing.Analyzed);
            Index(c => c.ProductExternalRef, FieldIndexing.Analyzed);
            Index(c => c.ClashCode, FieldIndexing.Analyzed);
            Index(c => c.Status, FieldIndexing.Analyzed);
            Index(c => c.StartDateTime, FieldIndexing.Analyzed);
            Index(c => c.EndDateTime, FieldIndexing.Analyzed);
        }
    }
}
