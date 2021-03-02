using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Optimizer.Facilities;
using ImagineCommunications.GamePlan.Persistence.RavenDb;

namespace xggameplan.Updates.UpdateXGGT2035
{
    internal class UpdateStepXGGT2035_AddDefaultFacilities : PatchUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;

        public UpdateStepXGGT2035_AddDefaultFacilities(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            var connectionStrings = tenantConnectionStrings.ToList();
            ValidateParametersBeforeUse(connectionStrings, updatesFolder);

            _tenantConnectionStrings = connectionStrings;
            var rollBackFolder = Path.Combine(updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(rollBackFolder);
        }

        public Guid Id => new Guid("E6A2814E-3049-4AD9-8963-4F2ED6BFE927");

        public int Sequence => 1;

        public string Name => "XGGT-2035";

        public bool SupportsRollback => false;

        public void Apply()
        {
            foreach (var tenantConnectionString in _tenantConnectionStrings)
            {
                using (var documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString, null))
                using (var session = documentStore.OpenSession())
                {
                    var newFacilities = new List<Facility>();

                    // Default facilities (Nine)
                    newFacilities.Add(new Facility {Code = "ABAREA", Description = "Allow split/joint spots in Automated Booking", Enabled = false});
                    newFacilities.Add(new Facility {Code = "ABBNEV", Description = "Automated Booking Include Bonus Spot Delivery", Enabled = false});
                    newFacilities.Add(new Facility {Code = "ABBONS", Description = "Autobook Bonus Value", Enabled = false});
                    newFacilities.Add(new Facility {Code = "ABBTRQ", Description = "Automated Booking Book to Target Requirement", Enabled = false});
                    newFacilities.Add(new Facility {Code = "ABDBUG", Description = "Automated Booking Debug File Generation", Enabled = true});
                    newFacilities.Add(new Facility {Code = "ABDEMO", Description = "Automated Booking Process Active Demographics Only", Enabled = false});
                    newFacilities.Add(new Facility {Code = "ABDPYT", Description = "Use Campaign Daypart in Addition to AB Daypart", Enabled = false});
                    newFacilities.Add(new Facility {Code = "ABEVTH", Description = "Automated Booking - Evaluation Threading", Enabled = false});
                    newFacilities.Add(new Facility {Code = "ABFAIL", Description = "Automated Booking - All Pass Failure Reporting", Enabled = true});
                    newFacilities.Add(new Facility {Code = "ABPRC2", Description = "Automated Booking Pricing Spot Based Campaign Only", Enabled = false});
                    newFacilities.Add(new Facility {Code = "ABPRCE", Description = "Autoboook Spot Pricing", Enabled = false});
                    newFacilities.Add(new Facility {Code = "ABPREQ", Description = "Automated Booking - Programme Inclusion with Slotting Controls", Enabled = true});
                    newFacilities.Add(new Facility {Code = "ABSPLG", Description = "Autobook by Length by Daypart by Strike Weight", Enabled = false});
                    newFacilities.Add(new Facility {Code = "ABSTAT", Description = "Store Run Statistics for Request", Enabled = false});
                    newFacilities.Add(new Facility {Code = "ABTZRB", Description = "Autobook Target Zero Rated Break", Enabled = true});
                    newFacilities.Add(new Facility {Code = "ABWGHT", Description = "Random Automated Booking Weightings", Enabled = false});
                    newFacilities.Add(new Facility {Code = "ABZBON", Description = "Automated Booking - Zero Price Bonus Spots", Enabled = true});
                    newFacilities.Add(new Facility {Code = "ABZERO", Description = "Book Campaigns with Zero Requirement", Enabled = true});
                    newFacilities.Add(new Facility {Code = "ABZRTG", Description = "Zero Rated Breaks Count for Rating Delivery", Enabled = true});
                    newFacilities.Add(new Facility {Code = "ADCLSH", Description = "Advertiser Clash", Enabled = false});
                    newFacilities.Add(new Facility {Code = "AGREG", Description = "Autogeneration Nat/Reg Booking", Enabled = false});
                    newFacilities.Add(new Facility {Code = "AUTSAG", Description = "Autobooking by Sales Area group", Enabled = false});
                    newFacilities.Add(new Facility {Code = "AUTSPN", Description = "Autobook Sponsorship campaigns by programme", Enabled = false});
                    newFacilities.Add(new Facility {Code = "B2BCLH", Description = "Back To Back Clash Available", Enabled = false});
                    newFacilities.Add(new Facility {Code = "BRKBON", Description = "Bonus Restriction on Break", Enabled = false});
                    newFacilities.Add(new Facility {Code = "BRKPGR", Description = "Break Position Groups", Enabled = false});
                    newFacilities.Add(new Facility {Code = "BTYPLN", Description = "Book spots by campaign Break Type and Length", Enabled = false});
                    newFacilities.Add(new Facility {Code = "CPPBKP", Description = "CPP Break Pricing", Enabled = false});
                    newFacilities.Add(new Facility {Code = "CURFIL", Description = "Earlier Automated Filling date", Enabled = false});
                    newFacilities.Add(new Facility {Code = "DECARE", Description = "Deal Campaign Regional Exclusions", Enabled = false});
                    newFacilities.Add(new Facility {Code = "DEMRST", Description = "Demographic Booking Restrictions", Enabled = false});
                    newFacilities.Add(new Facility {Code = "DISSUR", Description = "Campaign Discount and Surcharge", Enabled = false});
                    newFacilities.Add(new Facility {Code = "DLBOTY", Description = "Deal Bonus Types", Enabled = false});
                    newFacilities.Add(new Facility {Code = "DPSTWT", Description = "Define Dayparts by Deal and Campaign period", Enabled = true});
                    newFacilities.Add(new Facility {Code = "EFFFAC", Description = "Efficiency Factors", Enabled = false});
                    newFacilities.Add(new Facility {Code = "EFFRAN", Description = "Autogen Random Efficiency at 100eff", Enabled = true});
                    newFacilities.Add(new Facility {Code = "EPGTGT", Description = "EPG (Target) Sales Area Processing", Enabled = false});
                    newFacilities.Add(new Facility {Code = "EXCLSH", Description = "EXTEND CLASH", Enabled = true});
                    newFacilities.Add(new Facility {Code = "EXPDIS", Description = "Expected Expenditure Discounts", Enabled = false});
                    newFacilities.Add(new Facility {Code = "FLRPRC", Description = "Floor Pricing", Enabled = false});
                    newFacilities.Add(new Facility {Code = "HFSSIX", Description = "Enable Index Restrictions for HFSS or Age Group", Enabled = true});
                    newFacilities.Add(new Facility {Code = "LCKPRC", Description = "Lock Spot Price", Enabled = false});
                    newFacilities.Add(new Facility {Code = "MCNPSD", Description = "Update Spot from Nominal Prices", Enabled = false});
                    newFacilities.Add(new Facility {Code = "MNPRDC", Description = "Apply Discounts to Manually Priced Spots - Bulk", Enabled = false});
                    newFacilities.Add(new Facility {Code = "NATREG", Description = "National Regional Breaks", Enabled = false});
                    newFacilities.Add(new Facility {Code = "NODISC", Description = "Discount Processing Not Required", Enabled = false});
                    newFacilities.Add(new Facility {Code = "PGRCLA", Description = "Programme Classification", Enabled = true});
                    newFacilities.Add(new Facility {Code = "RATING", Description = "Hide Ratings", Enabled = false});
                    newFacilities.Add(new Facility {Code = "REGBRK", Description = "Regional Break - Store Reduced Break Information", Enabled = false});
                    newFacilities.Add(new Facility {Code = "RNDPRC", Description = "Spot Price - Round to Sales Area Ratecard Rounding", Enabled = false});
                    newFacilities.Add(new Facility {Code = "RTCTCA", Description = "Rate Card Tariff Category", Enabled = false});
                    newFacilities.Add(new Facility {Code = "SALREG", Description = "Regional Sales Availability", Enabled = false});
                    newFacilities.Add(new Facility {Code = "SANOEX", Description = "No of clash exposures defined by sales area", Enabled = true});
                    newFacilities.Add(new Facility {Code = "SBSPBP", Description = "Spot Book Starting Price from Break Price", Enabled = false});
                    newFacilities.Add(new Facility {Code = "SPNADV", Description = "Sponsorship Entitlement Group By Advertiser", Enabled = false});
                    newFacilities.Add(new Facility {Code = "SPNCHK", Description = "Sponsership Checking Required (y/n)", Enabled = false});
                    newFacilities.Add(new Facility {Code = "SPNENT", Description = "Sponsorship entitlement", Enabled = false});
                    newFacilities.Add(new Facility {Code = "SPTMSD", Description = "Spots map to scheduled payments", Enabled = false});
                    newFacilities.Add(new Facility {Code = "TOPRTG", Description = "Tot Spot Ratings Only", Enabled = false});
                    newFacilities.Add(new Facility {Code = "TOTEFF", Description = "Total efficiency for all Sales Areas within Break", Enabled = false});
                    newFacilities.Add(new Facility {Code = "XGCSVO", Description = "xG Gameplan - Produce Output Files with Header information", Enabled = true});
                    newFacilities.Add(new Facility {Code = "XGRTIM", Description = "xG Gameplan - Receive Ratings as Impressions", Enabled = true});

                    foreach (var facility in newFacilities)
                    {
                        session.Store(facility);
                    }

                    session.SaveChanges();
                }
            }
        }

        private static void ValidateParametersBeforeUse(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            _ = UpdateValidator.ValidateTenantConnectionString(tenantConnectionStrings, throwOnInvalid: true);
            _ = UpdateValidator.ValidateUpdateFolderPath(updatesFolder, throwOnInvalid: true);
        }

        public void RollBack() => throw new NotImplementedException();
    }
}
