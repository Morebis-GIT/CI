using System;
using System.Collections.Generic;
using System.IO;
using ImagineCommunications.GamePlan.Domain.Generic;
using ImagineCommunications.GamePlan.Domain.Shared.FunctionalAreas.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using xggameplan.Updates;

namespace xggameplan.updates.UpdateXGGT12473
{
    internal class UpdateStepXGGT12473_AddShortNameForFaultTypes : PatchUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;
        private readonly string _updatesFolder;
        private readonly string _rollBackFolder;

        public UpdateStepXGGT12473_AddShortNameForFaultTypes(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            _tenantConnectionStrings = tenantConnectionStrings;
            _updatesFolder = updatesFolder;
            _rollBackFolder = Path.Combine(_updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(_rollBackFolder);
        }

        public Guid Id => new Guid("B93AF7B7-A4A9-4552-AC6D-9F2523B8358F");

        public void Apply()
        {
            foreach (var tenantConnectionString in _tenantConnectionStrings)
            {
                using (var documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString, null))
                using (var session = documentStore.OpenSession())
                {
                    var allFunctionalAreas = session.GetAll<FunctionalArea>();

                    foreach (var functionalArea in allFunctionalAreas)
                    {
                        foreach (var faultType in functionalArea.FaultTypes)
                        {
                            if (_shortNames.TryGetValue(faultType.Id, out var shortName))
                            {
                                foreach (var language in Globals.SupportedLanguages)
                                {
                                    faultType.ShortName.Add(language, shortName);
                                }
                            }
                        }

                        session.Store(functionalArea);
                    }

                    session.SaveChanges();
                }
            }
        }

        public void RollBack() => throw new NotImplementedException();

        public int Sequence => 1;

        public string Name => "XGGT-12473: Add short name for fault types";

        public bool SupportsRollback => false;

        private readonly Dictionary<int, string> _shortNames = new Dictionary<int, string>
        {
            { 1, "Out Camp Strk Wght" },
            { 2, "Prgm not in Req" },
            { 3, "Prgm Cat not in Req" },
            { 4, "Prgm Req Incl Mismatch" },
            { 5, "Spot Lgth Inval for Prd" },
            { 6, "Pos Req not avlbl" },
            { 8, "0 req for Lgth" },
            { 9, "Out Camp Dprt" },
            { 10, "Not Enough Avail in Brk" },
            { 11, "Cntr/End Req" },
            { 12, "Time not in Req" },
            { 13, "Brk Type" },
            { 14, "Clash det" },
            { 15, "Restrictions" },
            { 16, "Clash w/self det" },
            { 19, "Max 0 Rtgs" },
            { 20, "Effcy/Rank Lim" },
            { 21, "Camp Req (Ach/Ovrsp)" },
            { 22, "SA Req (Ach/Ovrsp)" },
            { 23, "Cntr/End Req (Ach/Ovrsp)" },
            { 24, "Strk Wght Req (Ach/Ovrsp)" },
            { 25, "Strk Wght Len Req (Ach/Ovrsp)" },
            { 26, "Strk Wght/Dprt Req (Ach/Ovrsp)" },
            { 27, "Dprt Req (Ach/Ovrsp)" },
            { 28, "Spot Len Req (Ach/Ovrsp)" },
            { 29, "Book Pos Req (Ach/Ovrsp)" },
            { 30, "Max Spots/Day" },
            { 31, "Max Spots/Hr" },
            { 32, "Max Spots/2 Hrs" },
            { 33, "Min Brk Btw Spots" },
            { 34, "Max Spots/Prgm/Day" },
            { 35, "Max Spots/Prgm/Wk" },
            { 36, "Max Spots/Prgm/100 rtg" },
            { 37, "Min Wks Btw Prgm" },
            { 38, "Max Rtg for Spot Camp" },
            { 39, "Max Spots/Prgm/Time" },
            { 40, "Min Days Btw Prgm/Time" },
            { 41, "Min Wks Btw Prgm/Time" },
            { 42, "Min Hrs Btw Spots" },
            { 43, "Min Brk Avail" },
            { 44, "Split Book Not Alwd" },
            { 45, "Spnsp Clash det" },
            { 47, "Rgnl Brk book" },
            { 48, "Excl Camp Pkgs" },
            { 49, "SA Req (Tgt Ach/Ovrsp)" },
            { 50, "Strk Wght Req (Tgt Ach/Ovrsp)" },
            { 51, "Strk Wght Len Req (Tgt Ach/Ovrsp)" },
            { 52, "Strk Wght/Dprt Req (Ach/Ovrsp)" },
            { 53, "Dprt Req (Tgt Ach/Ovrsp)" },
            { 54, "Spot Len Req (Tgt Ach/Ovrsp)" },
            { 55, "Existing Spot" },
            { 56, "No avlbl Spot Tps" },
            { 57, "Camp Rev Req (Ach/Ovrsp)" },
            { 58, "SA Rev Req (Ach/Oversp)" },
            { 59, "Camp Rtcd Restr" },
            { 60, "Bonus Book Req" },
            { 61, "Strk Wght/Dprt Lght Req (Ach/Ovrsp)" },
            { 62, "Dprt Len Req (Ach/Ovrsp)" },
            { 63, "Strk Wght Len Req (Ach/Ovrsp)" },
            { 64, "Strk Wght/Dprt. Len Req (Tgt Ach/Ovrsp)" },
            { 65, "Dprt Len Req (Tgt. Ach/Ovrsp)" },
            { 66, "Strk Wght Len Req (Tgt Ach/Ovrsp)" },
            { 67, "No avlbl Sched Pmts" },
            { 68, "Floor Rate" },
            { 69, "Macro Book" },
            { 70, "Brk Demo Restr" },
            { 71, "Zero Rate Brks" },
            { 72, "Spot Len Book Rules" },
            { 73, "Max Rtgs for Rtg Camp" },
            { 74, "Bus Type Rev Req (Ach/Ovrsp)" },
            { 75, "Brk Type Req (Ach/Ovrsp)" },
            { 76, "Brk Type Rev Req (Ach/Ovrsp)" },
            { 77, "Strk Wght Rev Req (Ach/Oversp)" },
            { 78, "Prgm Req (Ach/Ovrsp)" },
            { 79, "Sponsorship Restr…<Clash>" },
            { 80, "Sponsorship Restr…<Advertiser>" },
            { 81, "Mob Restr" },
            { 82, "Min TARP`s not met" }
        };
    }
}
