using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Campaigns.Queries;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using Microsoft.Ajax.Utilities;
using xggameplan.Model;

namespace xggameplan.Controllers
{
    public partial class CampaignsController
    {
        private string BreakTypeValueErrorMessage(Exception ex)
        {
            var errorMessage = new StringBuilder(200);

            errorMessage.Append(ex.Message);
            errorMessage.Append(" ");
            errorMessage.Append(nameof(CreateCampaign.ExternalId));
            errorMessage.Append(" :: ");
            errorMessage.Append(ex.Data[nameof(CreateCampaign.ExternalId)]);

            return errorMessage.ToString();
        }

        public CampaignFilterGroupModel GenerateCampaignFilterGroup(IEnumerable<CampaignReducedModel> campaigns, CampaignFilterGroupQueryModel query)
        {
            var campaignFilterGroupModel = new CampaignFilterGroupModel();
            if (campaigns is null || !campaigns.Any() || query is null)
            {
                return campaignFilterGroupModel;
            }

            var businessTypes = new List<string>();
            var clashExternalRefs = new List<string>();
            var productExternalIds = new List<string>();
            var reportingCategories = new List<string>();
            var campaignsOutput = new List<CampaignFilterGroupDataModel>();
            var clashCodes = new List<CampaignFilterGroupDataModel>();
            var productBasicModels = new List<CampaignFilterGroupDataModel>();
            var mediaSalesGroups = new List<CampaignFilterGroupDataModel>();
            var productAssignees = new List<CampaignFilterGroupDataModel>();
            var agencies = new List<CampaignFilterGroupDataModel>();
            var advertisers = new List<CampaignFilterGroupDataModel>();

            foreach (var campaign in campaigns)
            {
                businessTypes.Add(campaign.BusinessType);
                campaignsOutput.Add(new CampaignFilterGroupDataModel { Identifier = campaign.ExternalId, Text = campaign.Name });
                productExternalIds.Add(campaign.Product);
            }

            var productDictionary = _productRepository
                                         .FindByExternal(productExternalIds)
                                         .ToDictionary(r => r.Externalidentifier);

            foreach (var productExternalId in productExternalIds)
            {
                if (productDictionary.TryGetValue(productExternalId, out Product product))
                {
                    productBasicModels.Add(new CampaignFilterGroupDataModel
                    {
                        Identifier = productExternalId,
                        Text = product.Name
                    });

                    if (!string.IsNullOrEmpty(product.ClashCode)) { clashExternalRefs.Add(product.ClashCode); }

                    if (!string.IsNullOrEmpty(product.ReportingCategory)) { reportingCategories.Add(product.ReportingCategory); }

                    if (product.AgencyGroup != null && !string.IsNullOrEmpty(product.AgencyGroup.Code))
                    {
                        mediaSalesGroups.Add(new CampaignFilterGroupDataModel
                        {
                            Identifier = product.AgencyGroup.Code,
                            Text = string.IsNullOrEmpty(product.AgencyGroup.ShortName) ? product.AgencyGroup.Code : product.AgencyGroup.ShortName
                        });
                    }

                    if (product.SalesExecutive != null)
                    {
                        productAssignees.Add(new CampaignFilterGroupDataModel
                        {
                            Identifier = product.SalesExecutive.Identifier.ToString(),
                            Text = string.IsNullOrEmpty(product.SalesExecutive.Name) ? product.SalesExecutive.Identifier.ToString() : product.SalesExecutive.Name
                        });
                    }

                    if (!string.IsNullOrEmpty(product.AgencyIdentifier))
                    {
                        agencies.Add(new CampaignFilterGroupDataModel
                        {
                            Identifier = product.AgencyIdentifier,
                            Text = string.IsNullOrEmpty(product.AgencyName) ? product.AgencyIdentifier : product.AgencyName
                        });
                    }

                    if (!string.IsNullOrEmpty(product.AdvertiserIdentifier))
                    {
                        advertisers.Add(new CampaignFilterGroupDataModel
                        {
                            Identifier = product.AdvertiserIdentifier,
                            Text = string.IsNullOrEmpty(product.AdvertiserName) ? product.AdvertiserIdentifier : product.AdvertiserName
                        });
                    }
                }
            }

            if (query.IsProductFiltered)
            {
                campaignFilterGroupModel.Products = productBasicModels
                    .DistinctBy(o => new { o.Identifier, o.Text })
                    .OrderBy(o => o.Text); ;
            }

            if (query.IsClashCodeFiltered)
            {
                var clashes = _clashRepository.FindByExternal(clashExternalRefs.Distinct().ToList());
                foreach (var clash in clashes)
                {
                    clashCodes.Add(new CampaignFilterGroupDataModel { Identifier = clash.Externalref, Text = clash.Description });
                }

                campaignFilterGroupModel.ClashCodes = clashCodes
                    .DistinctBy(o => new { o.Identifier, o.Text })
                    .OrderBy(o => o.Text);
            }

            if (query.IsBusinessTypeFiltered)
            {
                campaignFilterGroupModel.BusinessTypes = businessTypes.Distinct().OrderBy(o => o);
            }

            if (query.IsCampaignIdentifierFiltered)
            {
                campaignFilterGroupModel.Campaigns = campaignsOutput
                    .DistinctBy(o => new { o.Identifier, o.Text })
                    .OrderBy(o => o.Text);
            }

            if (query.IsReportingCategoryFiltered)
            {
                campaignFilterGroupModel.ReportingCategories = reportingCategories.Distinct().OrderBy(o => o);
            }

            if (query.IsMediaSalesGroupFiltered)
            {
                campaignFilterGroupModel.MediaSalesGroups = mediaSalesGroups
                    .DistinctBy(o => new { o.Identifier, o.Text })
                    .OrderBy(o => o.Text);
            }

            if (query.IsProductAssigneeFiltered)
            {
                campaignFilterGroupModel.ProductAssignees = productAssignees
                    .DistinctBy(o => new { o.Identifier, o.Text })
                    .OrderBy(o => o.Text);
            }

            if (query.IsAgencyFiltered)
            {
                campaignFilterGroupModel.Agencies = agencies
                    .DistinctBy(o => new { o.Identifier, o.Text })
                    .OrderBy(o => o.Text);
            }

            if (query.IsAdvertiserFiltered)
            {
                campaignFilterGroupModel.Advertisers = advertisers
                    .DistinctBy(o => new { o.Identifier, o.Text })
                    .OrderBy(o => o.Text);
            }

            return campaignFilterGroupModel;
        }

        private void ValidateTargetRatings(IEnumerable<CreateCampaign> campaigns)
        {
            foreach (var campaign in campaigns)
            {
                var multipartTotalSum = campaign.SalesAreaCampaignTarget.Select(salesArea =>
                {
                    decimal strikeWeightsDesiredSum = 0;
                    decimal strikeWeightsCurrentSum = 0;

                    var strikeWeights = salesArea.CampaignTargets
                        .Where(s => s.StrikeWeights?.Count > 0)
                        .SelectMany(s => s.StrikeWeights)
                        .ToList();

                    if (strikeWeights.Count > 0)
                    {
                        var strikeWeightsSum = strikeWeights.Select(sw =>
                        {
                            var dayPartDesiredSum = sw.DayParts?.Select(dp =>
                            {
                                var desiredLengthSum = dp.Lengths?.Sum(l => l.DesiredPercentageSplit) ?? 0;

                                if (dp.Lengths != null &&
                                    dp.Lengths.Count > 0 &&
                                    !desiredLengthSum.Equals(dp.DesiredPercentageSplit)
                                    )
                                {
                                    throw new InvalidDataException(
                                        "Sum of length desired percentage should equal to day part desired percentage");
                                }

                                return dp.DesiredPercentageSplit;
                            }).Sum(ds => ds);

                            if (!dayPartDesiredSum.Equals(sw.DesiredPercentageSplit))
                            {
                                throw new InvalidDataException(
                                    "Sum of day part desired percentage should equal to strike weight desired percentage");
                            }

                            var dayPartCurrentSum = sw.DayParts?.Select(dp =>
                            {
                                var currentLengthSum = dp.Lengths?.Sum(l => l.CurrentPercentageSplit) ?? 0;
                                if (dp.Lengths != null &&
                                    dp.Lengths.Count > 0 &&
                                    !currentLengthSum.Equals(dp.CurrentPercentageSplit)
                                    )
                                {
                                    throw new InvalidDataException(
                                        "Sum of length desired percentage should equal to day part desired percentage");
                                }

                                return dp.CurrentPercentageSplit;
                            }).Sum(cs => cs);

                            if (!dayPartCurrentSum.Equals(sw.CurrentPercentageSplit))
                            {
                                throw new InvalidDataException(
                                    "Sum of day part current percentage should equal to strike weight current percentage");
                            }

                            var lengthDesiredSum = sw.Lengths?.Sum(l => l?.DesiredPercentageSplit ?? 0);
                            if (!lengthDesiredSum.Equals(sw.DesiredPercentageSplit))
                            {
                                throw new InvalidDataException(
                                    "Sum of length desired percentage should equal to strike weight desired percentage");
                            }

                            var lengthCurrentSum = sw.Lengths?.Sum(l => l?.CurrentPercentageSplit ?? 0);
                            if (!lengthCurrentSum.Equals(sw.CurrentPercentageSplit))
                            {
                                throw new InvalidDataException(
                                    "Sum of length current percentage should equal to strike weight current percentage");
                            }

                            return Tuple.Create(sw.DesiredPercentageSplit, sw.CurrentPercentageSplit);
                        })
                        .ToList();

                        strikeWeightsDesiredSum = strikeWeightsSum.Sum(t => t?.Item1 ?? 0);
                        strikeWeightsCurrentSum = strikeWeightsSum.Sum(t => t?.Item2 ?? 0);
                    }

                    var multipartDesiredSum = salesArea.Multiparts?.Sum(l => l?.DesiredPercentageSplit ?? 0) ?? 0;
                    if (!multipartDesiredSum.Equals(strikeWeightsDesiredSum))
                    {
                        throw new InvalidDataException(
                            "For each sales area campaign target, sum of multipart desired percentage split should equal to sum of desired percentage split in strike weight");
                    }

                    var multipartCurrentSum = salesArea.Multiparts?.Sum(l => l?.CurrentPercentageSplit ?? 0) ?? 0;
                    if (!multipartCurrentSum.Equals(strikeWeightsCurrentSum))
                    {
                        throw new InvalidDataException(
                            "For each sales area campaign target, sum of multipart current percentage split should equal to sum of current percentage split in strike weight");
                    }

                    return Tuple.Create(multipartDesiredSum, multipartCurrentSum);
                })
                .ToList();

                var multipartsDesiredSum = multipartTotalSum.Sum(t => t?.Item1 ?? 0);
                var multipartsCurrentSum = multipartTotalSum.Sum(t => t?.Item2 ?? 0);

                if (!multipartsDesiredSum.Equals(campaign.TargetRatings))
                {
                    throw new InvalidDataException(
                        "Sum of sales area campaign target multipart desired percentage split should equal to Campaign target ratings");
                }

                if (!multipartsCurrentSum.Equals(campaign.ActualRatings))
                {
                    throw new InvalidDataException(
                        "Sum of sales area campaign target multipart current percentage split should equal to Campaign current ratings");
                }
            }
        }
    }
}
