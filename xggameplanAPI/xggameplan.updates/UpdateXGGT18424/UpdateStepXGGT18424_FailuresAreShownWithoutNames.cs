using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.FunctionalAreas.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Raven.Client;
using xggameplan.Updates;

namespace xggameplan.updates.UpdateXGGT18424
{
    internal class UpdateStepXGGT18424_FailuresAreShownWithoutNames : PatchUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;

        public UpdateStepXGGT18424_FailuresAreShownWithoutNames(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            _tenantConnectionStrings = tenantConnectionStrings;
            var rollBackFolder = Path.Combine(updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(rollBackFolder);
        }

        public Guid Id => new Guid("9f8b0dc1-cbdf-4c13-8a5a-49140618a2e9");

        public void Apply()
        {
            foreach (string tenantConnectionString in _tenantConnectionStrings)
            {
                using (IDocumentStore documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString))
                {
                    using (IDocumentSession session = documentStore.OpenSession())
                    {
                        var functionalAreas = session
                            .GetAll<FunctionalArea>()
                            .ToList();

                        if (functionalAreas != null)
                        {
                            foreach (var functionalArea in functionalAreas)
                            {
                                var faultType = functionalArea.FaultTypes
                                    .Where(a => a.ShortName is null || !a.ShortName.Any())
                                    .Select(c =>
                                    {
                                        switch (c.Id)
                                        {
                                            case 1:
                                                c.ShortName["ARA"] = "Out Camp Strk Wght";
                                                c.ShortName["ENG"] = "Out Camp Strk Wght";
                                                break;

                                            case 2:
                                                c.ShortName["ARA"] = "Prgm not in Req";
                                                c.ShortName["ENG"] = "Prgm not in Req";
                                                break;

                                            case 3:
                                                c.ShortName["ARA"] = "Prgm Cat not in Req";
                                                c.ShortName["ENG"] = "Prgm Cat not in Req";
                                                break;

                                            case 4:
                                                c.ShortName["ARA"] = "Prgm Req Incl Mismatch";
                                                c.ShortName["ENG"] = "Prgm Req Incl Mismatch";
                                                break;

                                            case 5:
                                                c.ShortName["ARA"] = "Spot Lgth Inval for Prd";
                                                c.ShortName["ENG"] = "Spot Lgth Inval for Prd";
                                                break;

                                            case 6:
                                                c.ShortName["ARA"] = "Pos Req not avlbl";
                                                c.ShortName["ENG"] = "Pos Req not avlbl";
                                                break;

                                            case 8:
                                                c.ShortName["ARA"] = "0 req for Lgth";
                                                c.ShortName["ENG"] = "0 req for Lgth";
                                                break;

                                            case 9:
                                                c.ShortName["ARA"] = "Out Camp Dprt";
                                                c.ShortName["ENG"] = "Out Camp Dprt";
                                                break;

                                            case 10:
                                                c.ShortName["ARA"] = "Not Enough Avail in Brk";
                                                c.ShortName["ENG"] = "Not Enough Avail in Brk";
                                                break;

                                            case 11:
                                                c.ShortName["ARA"] = "Cntr/End Req";
                                                c.ShortName["ENG"] = "Cntr/End Req";
                                                break;

                                            case 12:
                                                c.ShortName["ARA"] = "Time not in Req";
                                                c.ShortName["ENG"] = "Time not in Req";
                                                break;

                                            case 13:
                                                c.ShortName["ARA"] = "Brk Type";
                                                c.ShortName["ENG"] = "Brk Type";
                                                break;

                                            case 14:
                                                c.ShortName["ARA"] = "Clash det";
                                                c.ShortName["ENG"] = "Clash det";
                                                break;

                                            case 15:
                                                c.ShortName["ARA"] = "Restrictions";
                                                c.ShortName["ENG"] = "Restrictions";
                                                break;

                                            case 16:
                                                c.ShortName["ARA"] = "Clash w/self det";
                                                c.ShortName["ENG"] = "Clash w/self det";
                                                break;

                                            case 19:
                                                c.ShortName["ARA"] = "Max 0 Rtgs";
                                                c.ShortName["ENG"] = "Max 0 Rtgs";
                                                break;

                                            case 20:
                                                c.ShortName["ARA"] = "Effcy/Rank Lim";
                                                c.ShortName["ENG"] = "Effcy/Rank Lim";
                                                break;

                                            case 21:
                                                c.ShortName["ARA"] = "Camp Req (Ach/Ovrsp)";
                                                c.ShortName["ENG"] = "Camp Req (Ach/Ovrsp)";
                                                break;

                                            case 22:
                                                c.ShortName["ARA"] = "SA Req (Ach/Ovrsp)";
                                                c.ShortName["ENG"] = "SA Req (Ach/Ovrsp)";
                                                break;

                                            case 23:
                                                c.ShortName["ARA"] = "Cntr/End Req (Ach/Ovrsp)";
                                                c.ShortName["ENG"] = "Cntr/End Req (Ach/Ovrsp)";
                                                break;

                                            case 24:
                                                c.ShortName["ARA"] = "Strk Wght Req (Ach/Ovrsp)";
                                                c.ShortName["ENG"] = "Strk Wght Req (Ach/Ovrsp)";
                                                break;

                                            case 25:
                                                c.ShortName["ARA"] = "Strk Wght Len Req (Ach/Ovrsp)";
                                                c.ShortName["ENG"] = "Strk Wght Len Req (Ach/Ovrsp)";
                                                break;

                                            case 26:
                                                c.ShortName["ARA"] = "Strk Wght/Dprt Req (Ach/Ovrsp)";
                                                c.ShortName["ENG"] = "Strk Wght/Dprt Req (Ach/Ovrsp)";
                                                break;

                                            case 27:
                                                c.ShortName["ARA"] = "Dprt Req (Ach/Ovrsp)";
                                                c.ShortName["ENG"] = "Dprt Req (Ach/Ovrsp)";
                                                break;

                                            case 28:
                                                c.ShortName["ARA"] = "Spot Len Req (Ach/Ovrsp)";
                                                c.ShortName["ENG"] = "Spot Len Req (Ach/Ovrsp)";
                                                break;

                                            case 29:
                                                c.ShortName["ARA"] = "Book Pos Req (Ach/Ovrsp)";
                                                c.ShortName["ENG"] = "Book Pos Req (Ach/Ovrsp)";
                                                break;

                                            case 30:
                                                c.ShortName["ARA"] = "Max Spots/Day";
                                                c.ShortName["ENG"] = "Max Spots/Day";
                                                break;

                                            case 31:
                                                c.ShortName["ARA"] = "Max Spots/Hr";
                                                c.ShortName["ENG"] = "Max Spots/Hr";
                                                break;

                                            case 32:
                                                c.ShortName["ARA"] = "Max Spots/2 Hrs";
                                                c.ShortName["ENG"] = "Max Spots/2 Hrs";
                                                break;

                                            case 33:
                                                c.ShortName["ARA"] = "Min Brk Btw Spots";
                                                c.ShortName["ENG"] = "Min Brk Btw Spots";
                                                break;

                                            case 34:
                                                c.ShortName["ARA"] = "Max Spots/Prgm/Day";
                                                c.ShortName["ENG"] = "Max Spots/Prgm/Day";
                                                break;

                                            case 35:
                                                c.ShortName["ARA"] = "Max Spots/Prgm/Wk";
                                                c.ShortName["ENG"] = "Max Spots/Prgm/Wk";
                                                break;

                                            case 36:
                                                c.ShortName["ARA"] = "Max Spots/Prgm/100 rtg";
                                                c.ShortName["ENG"] = "Max Spots/Prgm/100 rtg";
                                                break;

                                            case 37:
                                                c.ShortName["ARA"] = "Min Wks Btw Prgm";
                                                c.ShortName["ENG"] = "Min Wks Btw Prgm";
                                                break;

                                            case 38:
                                                c.ShortName["ARA"] = "Max Rtg for Spot Camp";
                                                c.ShortName["ENG"] = "Max Rtg for Spot Camp";
                                                break;

                                            case 39:
                                                c.ShortName["ARA"] = "Max Spots/Prgm/Time";
                                                c.ShortName["ENG"] = "Max Spots/Prgm/Time";
                                                break;

                                            case 40:
                                                c.ShortName["ARA"] = "Min Days Btw Prgm/Time";
                                                c.ShortName["ENG"] = "Min Days Btw Prgm/Time";
                                                break;

                                            case 41:
                                                c.ShortName["ARA"] = "Min Wks Btw Prgm/Time";
                                                c.ShortName["ENG"] = "Min Wks Btw Prgm/Time";
                                                break;

                                            case 42:
                                                c.ShortName["ARA"] = "Min Hrs Btw Spots";
                                                c.ShortName["ENG"] = "Min Hrs Btw Spots";
                                                break;

                                            case 43:
                                                c.ShortName["ARA"] = "Min Brk Avail";
                                                c.ShortName["ENG"] = "Min Brk Avail";
                                                break;

                                            case 44:
                                                c.ShortName["ARA"] = "Split Book Not Alwd";
                                                c.ShortName["ENG"] = "Split Book Not Alwd";
                                                break;

                                            case 45:
                                                c.ShortName["ARA"] = "Spnsp Clash det";
                                                c.ShortName["ENG"] = "Spnsp Clash det";
                                                break;

                                            case 47:
                                                c.ShortName["ARA"] = "Rgnl Brk book";
                                                c.ShortName["ENG"] = "Rgnl Brk book";
                                                break;

                                            case 48:
                                                c.ShortName["ARA"] = "Excl Camp Pkgs";
                                                c.ShortName["ENG"] = "Excl Camp Pkgs";
                                                break;

                                            case 49:
                                                c.ShortName["ARA"] = "SA Req (Tgt Ach/Ovrsp)";
                                                c.ShortName["ENG"] = "SA Req (Tgt Ach/Ovrsp)";
                                                break;

                                            case 50:
                                                c.ShortName["ARA"] = "Strk Wght Req (Tgt Ach/Ovrsp)";
                                                c.ShortName["ENG"] = "Strk Wght Req (Tgt Ach/Ovrsp)";
                                                break;

                                            case 51:
                                                c.ShortName["ARA"] = "Strk Wght Len Req (Tgt Ach/Ovrsp)";
                                                c.ShortName["ENG"] = "Strk Wght Len Req (Tgt Ach/Ovrsp)";
                                                break;

                                            case 52:
                                                c.ShortName["ARA"] = "Strk Wght/Dprt Req (Ach/Ovrsp)";
                                                c.ShortName["ENG"] = "Strk Wght/Dprt Req (Ach/Ovrsp)";
                                                break;

                                            case 53:
                                                c.ShortName["ARA"] = "Dprt Req (Tgt Ach/Ovrsp)";
                                                c.ShortName["ENG"] = "Dprt Req (Tgt Ach/Ovrsp)";
                                                break;

                                            case 54:
                                                c.ShortName["ARA"] = "Spot Len Req (Tgt Ach/Ovrsp)";
                                                c.ShortName["ENG"] = "Spot Len Req (Tgt Ach/Ovrsp)";
                                                break;

                                            case 55:
                                                c.ShortName["ARA"] = "Existing Spot";
                                                c.ShortName["ENG"] = "Existing Spot";
                                                break;

                                            case 56:
                                                c.ShortName["ARA"] = "No avlbl Spot Tps";
                                                c.ShortName["ENG"] = "No avlbl Spot Tps";
                                                break;

                                            case 57:
                                                c.ShortName["ARA"] = "Camp Rev Req (Ach/Ovrsp)";
                                                c.ShortName["ENG"] = "Camp Rev Req (Ach/Ovrsp)";
                                                break;

                                            case 58:
                                                c.ShortName["ARA"] = "SA Rev Req (Ach/Oversp)";
                                                c.ShortName["ENG"] = "SA Rev Req (Ach/Oversp)";
                                                break;

                                            case 59:
                                                c.ShortName["ARA"] = "Camp Rtcd Restr";
                                                c.ShortName["ENG"] = "Camp Rtcd Restr";
                                                break;

                                            case 60:
                                                c.ShortName["ARA"] = "Bonus Book Req";
                                                c.ShortName["ENG"] = "Bonus Book Req";
                                                break;

                                            case 61:
                                                c.ShortName["ARA"] = "Strk Wght/Dprt Lght Req (Ach/Ovrsp)";
                                                c.ShortName["ENG"] = "Strk Wght/Dprt Lght Req (Ach/Ovrsp)";
                                                break;

                                            case 62:
                                                c.ShortName["ARA"] = "Dprt Len Req (Ach/Ovrsp)";
                                                c.ShortName["ENG"] = "Dprt Len Req (Ach/Ovrsp)";
                                                break;

                                            case 63:
                                                c.ShortName["ARA"] = "Strk Wght Len Req (Ach/Ovrsp)";
                                                c.ShortName["ENG"] = "Strk Wght Len Req (Ach/Ovrsp)";
                                                break;

                                            case 64:
                                                c.ShortName["ARA"] = "Strk Wght/Dprt. Len Req (Tgt Ach/Ovrsp)";
                                                c.ShortName["ENG"] = "Strk Wght/Dprt. Len Req (Tgt Ach/Ovrsp)";
                                                break;

                                            case 65:
                                                c.ShortName["ARA"] = "Dprt Len Req (Tgt. Ach/Ovrsp)";
                                                c.ShortName["ENG"] = "Dprt Len Req (Tgt. Ach/Ovrsp)";
                                                break;

                                            case 66:
                                                c.ShortName["ARA"] = "Strk Wght Len Req (Tgt Ach/Ovrsp)";
                                                c.ShortName["ENG"] = "Strk Wght Len Req (Tgt Ach/Ovrsp)";
                                                break;

                                            case 67:
                                                c.ShortName["ARA"] = "No avlbl Sched Pmts";
                                                c.ShortName["ENG"] = "No avlbl Sched Pmts";
                                                break;

                                            case 68:
                                                c.ShortName["ARA"] = "Floor Rate";
                                                c.ShortName["ENG"] = "Floor Rate";
                                                break;

                                            case 69:
                                                c.ShortName["ARA"] = "Macro Book";
                                                c.ShortName["ENG"] = "Macro Book";
                                                break;

                                            case 70:
                                                c.ShortName["ARA"] = "Brk Demo Restr";
                                                c.ShortName["ENG"] = "Brk Demo Restr";
                                                break;

                                            case 71:
                                                c.ShortName["ARA"] = "0 Rate Brks";
                                                c.ShortName["ENG"] = "0 Rate Brks";
                                                break;

                                            case 72:
                                                c.ShortName["ARA"] = "Spot Len Book Rules";
                                                c.ShortName["ENG"] = "Spot Len Book Rules";
                                                break;

                                            case 73:
                                                c.ShortName["ARA"] = "Max Rtgs for Rtg Camp";
                                                c.ShortName["ENG"] = "Max Rtgs for Rtg Camp";
                                                break;

                                            case 78:
                                                c.ShortName["ARA"] = "Prgm Req (Ach/Ovrsp)";
                                                c.ShortName["ENG"] = "Prgm Req (Ach/Ovrsp)";
                                                break;

                                            case 79:
                                                c.ShortName["ARA"] = "Sponsorship Restr…<Clash>";
                                                c.ShortName["ENG"] = "Sponsorship Restr…<Clash>";
                                                break;

                                            case 80:
                                                c.ShortName["ARA"] = "Sponsorship Restr…<Advertiser>";
                                                c.ShortName["ENG"] = "Sponsorship Restr…<Advertiser>";
                                                break;

                                            case 82:
                                                c.ShortName["ARA"] = "Min TARP`s not met";
                                                c.ShortName["ENG"] = "Min TARP`s not met";
                                                break;
                                        }
                                        return c;
                                    }).ToList();
                            }

                            session.SaveChanges();
                        }
                    }
                }
            }
        }

        public void RollBack() => throw new NotImplementedException();

        public int Sequence => 1;

        public string Name => "XGGT-18424";

        public bool SupportsRollback => false;
    }
}
