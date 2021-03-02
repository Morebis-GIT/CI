using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.BusinessTypes.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb;

namespace xggameplan.Updates
{
    internal class UpdateStepXGGT14974_AddBusinessTypes : PatchUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;

        public UpdateStepXGGT14974_AddBusinessTypes(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            var connectionStrings = tenantConnectionStrings.ToList();
            ValidateParametersBeforeUse(connectionStrings, updatesFolder);

            _tenantConnectionStrings = connectionStrings;
            var rollBackFolder = Path.Combine(updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(rollBackFolder);
        }

        public Guid Id => new Guid("9ed71c56-925e-4935-8e34-f304d2ff4a58");

        public int Sequence => 1;

        public string Name => "XGGT-14974: Add business types";

        public bool SupportsRollback => false;

        public void Apply()
        {
            foreach (var tenantConnectionString in _tenantConnectionStrings)
            {
                using (var documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString, null))
                using (var session = documentStore.OpenSession())
                {
                    _businessTypes.ForEach(b => session.Store(b));
                    session.SaveChanges();
                }
            }
        }

        private readonly List<BusinessType> _businessTypes = new List<BusinessType> {
            new BusinessType {
                Id = 1,
                Code = "STD",
                Name = "SK-CS-UK - STANDARD AIRTIME",
                IncludeConversionIndex = true
            },
            new BusinessType {
                Id = 2,
                Code = "BAR",
                Name = "SK-CS-UK - BARTER TRADE",
                IncludeConversionIndex = true
            },
            new BusinessType {
                Id = 3,
                Code = "BAT",
                Name = "SK-CS-UK - BARTER TRADE DISC",
                IncludeConversionIndex = true
            },
            new BusinessType {
                Id = 8,
                Code = "SPN",
                Name = "SP-NS-UK - SKY SPORTS SPONSORS",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 9,
                Code = "PRD",
                Name = "SP-NS-UK - SKY 1 SPONSORSHIP",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 10,
                Code = "TEL",
                Name = "DO-TS-UK - DOLPHIN TELESHOPPIN",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 11,
                Code = "IAS",
                Name = "IA-NS-UK - RED SURCHARGE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 12,
                Code = "DAL",
                Name = "IA-NS-UK - DAL",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 13,
                Code = "IMR",
                Name = "IA-NS-UK - IMPULSE RESPONSE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 14,
                Code = "LEF",
                Name = "IA-NS-UK - LEAD FEE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 15,
                Code = "MID",
                Name = "IA-NS-UK - MINI DAL",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 16,
                Code = "MIC",
                Name = "IA-NS-UK - MICROSITE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 17,
                Code = "IAB",
                Name = "IA-NS-UK - BANNER",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 19,
                Code = "CHT",
                Name = "CH-NM-UK - CHELSEA TV SALES HS",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 28,
                Code = "ONL",
                Name = "ON-NS-UK - ONLINE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 29,
                Code = "ETV",
                Name = "IA-NS-UK - SPONSORSHIP ETV",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 30,
                Code = "IAC",
                Name = "IA-NS-UK - SPONSORSHIP CREDIT",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 31,
                Code = "IAP",
                Name = "IA-NS-UK - SPONSORSHIP PROD",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 32,
                Code = "LON",
                Name = "IA-NS-UK - GREEN CONTENT",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 33,
                Code = "ANT",
                Name = "DG-NM-UK - ON DEMAND",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 34,
                Code = "TPF",
                Name = "IA-NS-UK - THIRD PTY LEAD FEE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 35,
                Code = "TPS",
                Name = "IA-NS-UK - THIRD PARTY SITES",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 36,
                Code = "TXR",
                Name = "TX-NS-UK - AT THE RACES",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 37,
                Code = "TXB",
                Name = "TX-NS-UK - BETTING",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 38,
                Code = "TEM",
                Name = "TX-NS-UK - EMAP",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 39,
                Code = "TEU",
                Name = "TX-NS-UK - EUROSPORT",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 40,
                Code = "TXF",
                Name = "TX-NS-UK - FINANCE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 41,
                Code = "TXG",
                Name = "TX-NS-UK - GENERAL DISPLAY",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 42,
                Code = "TXH",
                Name = "TX-NS-UK - HOLIDAYS",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 43,
                Code = "TXI",
                Name = "TX-NS-UK - INTERACTIVE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 44,
                Code = "TXL",
                Name = "TX-NS-UK - LIVE TV",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 45,
                Code = "TPR",
                Name = "TX-NS-UK - PREMIUM RATE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 46,
                Code = "XSP",
                Name = "TX-NS-UK - SPONSORSHIP",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 47,
                Code = "TXS",
                Name = "TX-NS-UK - SPORT",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 48,
                Code = "TXW",
                Name = "TX-NS-UK - WEB",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 49,
                Code = "DIS",
                Name = "DI-CS-UK - DISCOVERY SALES HSE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 50,
                Code = "HAL",
                Name = "HA-CS-UK - HALLM SH DO NOT USE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 51,
                Code = "EMA",
                Name = "EM-CS-UK - EMAP SALES HOUSE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 52,
                Code = "HIS",
                Name = "HI-CS-UK - HISTORY SALES HOUSE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 53,
                Code = "NGE",
                Name = "NG-CS-UK - NAT GEO SALES HOUSE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 54,
                Code = "ZON",
                Name = "ZO-CS-UK - ZONEMEDIA SALES HSE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 55,
                Code = "EXT",
                Name = "ES-CS-UK - ESPN SALES HOUSE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 56,
                Code = "FXU",
                Name = "FX-CS-UK - FXUK SALES HOUSE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 57,
                Code = "DRT",
                Name = "SK-CS-UK - DRTV AIRTIME",
                IncludeConversionIndex = true
            },
            new BusinessType {
                Id = 58,
                Code = "SPS",
                Name = "SK-CS-UK - SPONSORSHIP",
                IncludeConversionIndex = true
            },
            new BusinessType {
                Id = 59,
                Code = "IRE",
                Name = "SK-CS-IR - IRELAND AIRTIME",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 60,
                Code = "PUB",
                Name = "PU-NM-UK - PUB AIRTIME",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 61,
                Code = "PUC",
                Name = "PU-NM-UK - CHELSEA TV",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 62,
                Code = "ANI",
                Name = "DG-NM-IR - ON DEMAND",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 63,
                Code = "ETI",
                Name = "IA-NS-IR - SPONSORSHIP ETV",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 64,
                Code = "ICI",
                Name = "IA-NS-IR - SPONSORSHIP CREDIT",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 65,
                Code = "IDA",
                Name = "IA-NS-IR - DAL",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 66,
                Code = "IIB",
                Name = "IA-NS-IR - BANNER",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 67,
                Code = "IIM",
                Name = "IA-NS-IR - IMPULSE RESPONSE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 68,
                Code = "IIS",
                Name = "IA-NS-IR - RED SURCHARGE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 69,
                Code = "ILE",
                Name = "IA-NS-IR - LEAD FEE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 70,
                Code = "ILO",
                Name = "IA-NS-IR - GREEN CONTENT",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 71,
                Code = "IMI",
                Name = "IA-NS-IR - MICROSITE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 72,
                Code = "IPI",
                Name = "IA-NS-IR - SPONSORSHIP PROD",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 73,
                Code = "ITF",
                Name = "IA-NS-IR - THIRD PTY LEAD FEE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 74,
                Code = "ITS",
                Name = "IA-NS-IR - THIRD PARTY SITES",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 75,
                Code = "MII",
                Name = "IA-NS-IR - MINI DAL",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 76,
                Code = "NMS",
                Name = "SK-NM-UK - BOX OFFICE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 77,
                Code = "SHD",
                Name = "SK-CS-UK - SHARED REWARD",
                IncludeConversionIndex = true
            },
            new BusinessType {
                Id = 79,
                Code = "SNI",
                Name = "SK-NM-IN - SKY NEWS INTERNATIO",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 80,
                Code = "SNN",
                Name = "SK-NM-IN - NATGEO NEWS INTERNA",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 81,
                Code = "TEI",
                Name = "DO-TS-IR - DOLPHIN IRISH TELES",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 82,
                Code = "TES",
                Name = "SK-TS-UK - SKY TELESHOPPING",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 83,
                Code = "NGI",
                Name = "SK-CS-IR - NATGEO SKY NEWS",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 84,
                Code = "PBA",
                Name = "PU-NM-UK - BARTER TRADE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 85,
                Code = "PBT",
                Name = "PU-NM-UK - BARTER TRADE DISC",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 86,
                Code = "SPR",
                Name = "PR-CS-UK - SKY PROMO AIRTIME",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 87,
                Code = "CHS",
                Name = "CH-CS-UK - CHARTSHOW SALES HSE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 88,
                Code = "NSD",
                Name = "NS-NS-UK - PRIMARY GENERIC",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 89,
                Code = "SPI",
                Name = "SP-NS-IR - SPONSORSHIP",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 90,
                Code = "NSI",
                Name = "NS-NS-IR - PRIMARY GENERIC",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 91,
                Code = "ONI",
                Name = "ON-NS-IR - ONLINE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 92,
                Code = "IAV",
                Name = "IA-NS-UK - VAL",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 93,
                Code = "IVA",
                Name = "IA-NS-IR - VAL",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 94,
                Code = "SCI",
                Name = "NB-CS-UK - NBC SALES HOUSE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 95,
                Code = "IPR",
                Name = "SK-CS-UK - INTERNAL PROMOTIONS",
                IncludeConversionIndex = true
            },
            new BusinessType {
                Id = 96,
                Code = "IBO",
                Name = "IA-NM-UK - BOX OFFICE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 97,
                Code = "DSP",
                Name = "DI-NS-UK - DISCOVERY SPONSORSH",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 98,
                Code = "HSP",
                Name = "HI-NS-UK - HISTORY SPONSORSHIP",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 99,
                Code = "ZSP",
                Name = "ZO-NS-UK - ZONEMEDIA SPONSORSH",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 100,
                Code = "FSP",
                Name = "FX-NS-UK - FXUK SPONSORSHIP",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 101,
                Code = "ESP",
                Name = "ES-NS-UK - ESPN SPONSORSHIP",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 102,
                Code = "HAS",
                Name = "HA-NS-UK - HALLM SP DO NOT USE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 103,
                Code = "SSP",
                Name = "NB-NS-UK - NBC SPONSORSHIP",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 104,
                Code = "NSP",
                Name = "NG-NS-UK - NAT GEO SPONSORSHIP",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 105,
                Code = "IAG",
                Name = "IA-NS-UK - GREEN SURCHARGE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 106,
                Code = "IIG",
                Name = "IA-NS-IR - GREEN SURCHARGE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 107,
                Code = "IGV",
                Name = "IA-NS-IR - GO VIEW",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 108,
                Code = "IGI",
                Name = "IA-NS-UK - GO VIEW",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 109,
                Code = "PRI",
                Name = "SK-CS-IR - INTERNAL PROMOTIONS",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 110,
                Code = "SKP",
                Name = "DG-NS-UK - SKY GO VOD",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 111,
                Code = "VIA",
                Name = "VI-CS-UK - VIACOM SALES HOUSE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 114,
                Code = "KID",
                Name = "KI-CS-UK - KIDS STANDARD AIRTI",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 115,
                Code = "IRK",
                Name = "KI-CS-IR - IRELAND KIDS AIRTIM",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 116,
                Code = "VIR",
                Name = "VI-CS-IR - VIACOM SALESHSE IRE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 117,
                Code = "KIB",
                Name = "KI-CS-UK - KIDS BARTER TRADE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 118,
                Code = "IBA",
                Name = "SK-CS-IR - BARTER TRADE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 119,
                Code = "IKB",
                Name = "SK-CS-IR - IRE KIDS BARTER TRA",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 120,
                Code = "DIR",
                Name = "DI-CS-IR - DISCOV SALESHSE IRE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 121,
                Code = "PAR",
                Name = "SK-CS-UK - PARTNERSHIPS",
                IncludeConversionIndex = true
            },
            new BusinessType {
                Id = 122,
                Code = "KIP",
                Name = "KI-CS-UK - KIDS PARTNERSHIP",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 123,
                Code = "KDR",
                Name = "KI-CS-UK - KIDS DRTV",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 125,
                Code = "SPB",
                Name = "SP-NS-UK - SPONSORSHIP BARTER",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 126,
                Code = "VSP",
                Name = "VI-NS-UK - VIACOM SPONSORSHIP",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 127,
                Code = "BTX",
                Name = "TX-NS-UK - BARTER TRADE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 128,
                Code = "FUA",
                Name = "SK-CS-UK - FUTURES",
                IncludeConversionIndex = true
            },
            new BusinessType {
                Id = 129,
                Code = "SPM",
                Name = "SP-NS-UK - SKY MOVIES SPONSORS",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 130,
                Code = "SNE",
                Name = "SP-NS-UK - SKY NEWS SPONSORSHI",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 131,
                Code = "SPA",
                Name = "SP-NS-UK - SKY ARTS SPONSORSHI",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 132,
                Code = "SRE",
                Name = "SP-NS-UK - SKY REAL LIVES SPON",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 133,
                Code = "MME",
                Name = "MM-NS-UK - MULTIMEDIA (SPONS)",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 134,
                Code = "MMC",
                Name = "MM-NS-UK - MULTIMEDIA CREATIVE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 135,
                Code = "MAG",
                Name = "MZ-NS-UK - SKY PREVIEW MAGAZIN",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 136,
                Code = "MSP",
                Name = "MZ-NS-UK - SPORTS MAGAZINE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 137,
                Code = "MMO",
                Name = "MZ-NS-UK - MOVIES MAGAZINE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 138,
                Code = "MMS",
                Name = "MM-NS-UK - MULTIMEDIA (SPOT)",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 139,
                Code = "IBB",
                Name = "IA-NM-UK - BOX OFFICE BARTER",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 140,
                Code = "MMI",
                Name = "MZ-NS-IR - MOVIES MAGAZINE IRE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 141,
                Code = "MAI",
                Name = "MZ-NS-IR - SKY MAGAZINE IRE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 142,
                Code = "MSI",
                Name = "MZ-NS-IR - SPORTS MAGAZINE IRE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 143,
                Code = "PTD",
                Name = "PU-NM-UK - 3D TV AIRTIME",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 144,
                Code = "MGM",
                Name = "MG-CS-UK - MGM SALES HOUSE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 145,
                Code = "MOV",
                Name = "ON-NS-UK - MOBILE VOD",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 146,
                Code = "CRS",
                Name = "ON-NS-UK - CREATIVE SOLUTIONS",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 147,
                Code = "ONV",
                Name = "ON-NS-UK - ONLINE VOD",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 148,
                Code = "INB",
                Name = "IA-NM-UK - INTEGRATED BOX OFFI",
                IncludeConversionIndex = true
            },
            new BusinessType {
                Id = 149,
                Code = "INS",
                Name = "SP-NS-UK - INTEGRATED SPONSORS",
                IncludeConversionIndex = true
            },
            new BusinessType {
                Id = 150,
                Code = "INA",
                Name = "DG-NM-UK - INTEGRATED ON DEMAN",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 151,
                Code = "INP",
                Name = "PU-NM-UK - INTEGRATED PUB",
                IncludeConversionIndex = true
            },
            new BusinessType {
                Id = 152,
                Code = "INK",
                Name = "IA-NS-UK - INTEG SKY GO NON SP",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 153,
                Code = "SKB",
                Name = "DG-NS-UK - SKY GO VOD BARTER",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 154,
                Code = "VIG",
                Name = "SK-CS-UK - VIRGIN MEDIA",
                IncludeConversionIndex = true
            },
            new BusinessType {
                Id = 155,
                Code = "SAT",
                Name = "SP-NS-UK - SKY ATLANTIC SPONSO",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 156,
                Code = "SLI",
                Name = "SP-NS-UK - SKY LIVING SPONSORS",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 157,
                Code = "BMD",
                Name = "SK-CS-UK - BSKYB MRKTING DRTV",
                IncludeConversionIndex = true
            },
            new BusinessType {
                Id = 158,
                Code = "BMB",
                Name = "SK-CS-UK - BSKYB MRKTING BRAND",
                IncludeConversionIndex = true
            },
            new BusinessType {
                Id = 159,
                Code = "BMK",
                Name = "KI-CS-UK - BSKYB MRKTING KIDS",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 160,
                Code = "TEH",
                Name = "HR-TS-UK - HARPER TELESHOPPING",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 161,
                Code = "MMA",
                Name = "MM-NS-UK - MULTIMEDIA (INTERA)",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 162,
                Code = "PAB",
                Name = "SK-CS-UK - PARTNERSHIPS BARTER",
                IncludeConversionIndex = true
            },
            new BusinessType {
                Id = 163,
                Code = "IPA",
                Name = "SK-CS-UK - INTEGRATED PARTNERS",
                IncludeConversionIndex = true
            },
            new BusinessType {
                Id = 164,
                Code = "ANB",
                Name = "DG-NM-UK - ON DEMAND BARTER",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 165,
                Code = "SGO",
                Name = "SK-NS-UK - ADSMART SKY GO VOD",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 167,
                Code = "TEA",
                Name = "MA-TS-UK - MARK BROWN TELESHOP",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 168,
                Code = "KSH",
                Name = "KI-CS-UK - KIDS SHARED REWARD",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 169,
                Code = "PRP",
                Name = "SP-NS-UK - PRODUCT PLACEMENT",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 170,
                Code = "IAT",
                Name = "DG-NM-UK - INTEG ON DEMAND BAR",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 171,
                Code = "HEX",
                Name = "PU-NS-UK - HEATHROW EXPRESS",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 172,
                Code = "KPB",
                Name = "KI-CS-UK - KIDS PARTNERSHP BAR",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 173,
                Code = "MGS",
                Name = "MG-NS-UK - MGM SPONSORSHIP",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 174,
                Code = "CSP",
                Name = "CH-NS-UK - CHARTSHOW SPONSORSH",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 175,
                Code = "SHB",
                Name = "SK-CS-UK - SKY BET",
                IncludeConversionIndex = true
            },
            new BusinessType {
                Id = 176,
                Code = "SGI",
                Name = "DG-NS-UK - INTEGRATED SKYG VOD",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 177,
                Code = "SGP",
                Name = "DG-NS-IR - IRELAND SKY GO VOD",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 178,
                Code = "KPR",
                Name = "KI-CS-UK - KIDS INTERNAL PROMO",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 179,
                Code = "KPI",
                Name = "KI-CS-IR - KIDS INT PROMO IRE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 180,
                Code = "BKD",
                Name = "KI-CS-UK - BSKYB MRK KIDS DRTV",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 181,
                Code = "KIT",
                Name = "KI-CS-UK - KIDS BARTER TRADE D",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 182,
                Code = "TEB",
                Name = "MA-TS-IR - MARK BROWN TELE IRE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 183,
                Code = "CON",
                Name = "SK-CS-UK - CONTRA AIRTIME",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 184,
                Code = "ADS",
                Name = "SK-CS-UK - ADSMART",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 185,
                Code = "BPR",
                Name = "SK-CS-UK - BROADCAST PROMOTION",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 186,
                Code = "SPC",
                Name = "SP-NS-UK - CHALLENGE SPONSORSH",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 187,
                Code = "GLO",
                Name = "GL-CS-UK - GLOBAL TV SALES HSE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 188,
                Code = "SGL",
                Name = "SK-NM-UK - SKY GO LINEAR",
                IncludeConversionIndex = true
            },
            new BusinessType {
                Id = 189,
                Code = "SGN",
                Name = "SK-NM-UK - SKY GO NON-REC",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 190,
                Code = "SGB",
                Name = "SK-NM-UK - INTEG SKY GO BARTER",
                IncludeConversionIndex = true
            },
            new BusinessType {
                Id = 191,
                Code = "ISG",
                Name = "SK-NM-UK - INTEG SKY GO LINEAR",
                IncludeConversionIndex = true
            },
            new BusinessType {
                Id = 192,
                Code = "SBA",
                Name = "SK-NM-UK - SKY GO BARTER",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 193,
                Code = "AIP",
                Name = "SK-CS-UK - ADSMART INT PROMO",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 194,
                Code = "SPP",
                Name = "SP-NS-UK - PICK TV SPONSORSHIP",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 195,
                Code = "STA",
                Name = "ST-CS-UK - STAR TV SALES HOUSE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 196,
                Code = "VCM",
                Name = "VC-CS-UK - VIACOM 18 SALES HSE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 197,
                Code = "ARY",
                Name = "AR-CS-UK - ARY NETWK SALES HSE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 198,
                Code = "ASI",
                Name = "AS-CS-UK - ASIA TV SALES HOUSE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 199,
                Code = "MSM",
                Name = "MS-CS-UK - MSM ASIA SALES HSE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 200,
                Code = "TVT",
                Name = "TV-CS-UK - TV TODAY SALES HSE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 201,
                Code = "GEO",
                Name = "GE-CS-UK - GEO SALES HOUSE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 202,
                Code = "SCR",
                Name = "SC-CS-UK - SCRIPPS SALES HOUSE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 203,
                Code = "HOR",
                Name = "HO-CS-UK - HORSE & CO SALES HS",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 205,
                Code = "TER",
                Name = "SK-TS-IR - SKY IRISH TELESHOPP",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 206,
                Code = "SGS",
                Name = "SK-NM-UK - SHARED RE SKYGO LIN",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 207,
                Code = "BPS",
                Name = "SK-NM-UK - BCAST PR SKYGO LIN",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 209,
                Code = "SHI",
                Name = "SK-CS-IR - BSKYB IRELAND",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 210,
                Code = "TEZ",
                Name = "ZE-TS-UK - ZESTIFY MEDIA TELES",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 211,
                Code = "SON",
                Name = "SO-CS-UK - SONY PICS SALES HSE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 212,
                Code = "PFB",
                Name = "SK-CS-UK - PROD FIN VALUE",
                IncludeConversionIndex = true
            },
            new BusinessType {
                Id = 213,
                Code = "ADB",
                Name = "SK-CS-UK - ADSMART BARTER",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 214,
                Code = "SOS",
                Name = "SO-NS-UK - SONY PICS SPONSORSH",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 215,
                Code = "AWP",
                Name = "SP-NS-UK - AROUND WORLD SPONSO",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 216,
                Code = "S4C",
                Name = "SK-CS-UK - S4C STD AIRTIME",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 217,
                Code = "PPA",
                Name = "PU-NM-UK - PUB PARTNERSHIP",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 218,
                Code = "S4D",
                Name = "SK-CS-UK - S4C DRTV AIRTIME",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 219,
                Code = "S4B",
                Name = "SK-CS-UK - S4C BARTER AIRTIME",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 220,
                Code = "S4S",
                Name = "SP-NS-UK - S4C SPONSORSHIP",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 221,
                Code = "SHA",
                Name = "SK-CS-UK - SHARED REWD ADSMART",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 222,
                Code = "PRF",
                Name = "SK-CS-UK - PROD FIN BUY",
                IncludeConversionIndex = true
            },
            new BusinessType {
                Id = 223,
                Code = "IRD",
                Name = "SK-CS-IR - IRELAND DRTV AIRTIM",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 224,
                Code = "IKD",
                Name = "SK-CS-IR - IRELAND KIDS DRTV",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 225,
                Code = "VIN",
                Name = "VN-NS-UK - VINTAGE SPONSORSHIP",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 226,
                Code = "FOR",
                Name = "FO-CS-UK - FORCES TV SALES HSE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 227,
                Code = "TVV",
                Name = "DG-NS-UK - TV VOD",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 228,
                Code = "TVI",
                Name = "DG-NS-UK - INTEGRATED TV VOD",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 229,
                Code = "TIR",
                Name = "DG-NS-IR - IRELAND TV VOD",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 230,
                Code = "ATV",
                Name = "SK-NS-UK - ADSMART TV VOD",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 231,
                Code = "SCS",
                Name = "SC-NS-UK - SCRIPPS SPONSORSHIP",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 232,
                Code = "IPB",
                Name = "PU-NM-UK - INTEGRATED PUB BART",
                IncludeConversionIndex = true
            },
            new BusinessType {
                Id = 233,
                Code = "MAB",
                Name = "MZ-NS-UK - SKY PREVIEW MAG BAR",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 234,
                Code = "SPK",
                Name = "SP-NS-UK - SPIKE SPONSORSHIP",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 235,
                Code = "ST5",
                Name = "SK-CS-UK - 5 STANDARD NETWORK",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 236,
                Code = "DR5",
                Name = "SK-CS-UK - 5 DRTV NETWORK",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 237,
                Code = "KI5",
                Name = "KI-CS-UK - 5 KIDS STANDARD AIR",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 238,
                Code = "KD5",
                Name = "KI-CS-UK - 5 KIDS DRTV",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 239,
                Code = "BA5",
                Name = "SK-CS-UK - 5 BARTER TRADE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 240,
                Code = "5LN",
                Name = "SK-CS-UK - 5 REGION LONDON",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 241,
                Code = "5NO",
                Name = "SK-CS-UK - 5 REGION NORTH",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 242,
                Code = "5SC",
                Name = "SK-CS-UK - 5 REGION SCOTLAND",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 243,
                Code = "5UL",
                Name = "SK-CS-UK - 5 REGION ULSTER",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 245,
                Code = "SD5",
                Name = "SK-CS-UK - 5 STANDARD DIGITAL",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 246,
                Code = "DD5",
                Name = "SK-CS-UK - 5 DRTV DIGITAL",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 247,
                Code = "5NM",
                Name = "SK-CS-UK - 5 NWK MRKTING BRAND",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 248,
                Code = "5MD",
                Name = "SK-CS-UK - 5 NWK MRKTING DRTV",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 249,
                Code = "5DM",
                Name = "SK-CS-UK - 5 DIG MRKTING BRAND",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 250,
                Code = "5DD",
                Name = "SK-CS-UK - 5 DIG MRKTING DRTV",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 251,
                Code = "5SN",
                Name = "SK-CS-UK - CONTENT STUDIO 5NW",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 252,
                Code = "5SD",
                Name = "SK-CS-UK - CONTENT STUDIO 5DI",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 253,
                Code = "5CN",
                Name = "SK-CS-UK - CONTENT COMM 5NW",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 254,
                Code = "5CD",
                Name = "SK-CS-UK - CONTENT COMM 5DI",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 255,
                Code = "5FN",
                Name = "SK-CS-UK - CONTENT FUND 5NW",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 256,
                Code = "5FD",
                Name = "SK-CS-UK - CONTENT FUND 5DI",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 257,
                Code = "5IS",
                Name = "SP-NS-UK - C5 INTEGRATED SPONS",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 258,
                Code = "5SP",
                Name = "SP-NS-UK - C5 SPONSORSHIP",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 259,
                Code = "5PF",
                Name = "SK-CS-UK - 5 PROD FIN BUY DIG",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 260,
                Code = "5PR",
                Name = "SK-CS-UK - 5 PROD FIN BUY NW",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 261,
                Code = "5PV",
                Name = "SK-CS-UK - 5 PROD FIN VALUE NW",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 262,
                Code = "5FV",
                Name = "SK-CS-UK - 5 PROD FIN VALUE DI",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 263,
                Code = "5PP",
                Name = "SP-NS-UK - 5 PRODUCT PLACEMNT",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 264,
                Code = "5IP",
                Name = "SP-NS-UK - C5 INTEG PROD PLACE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 265,
                Code = "5ID",
                Name = "DG-NM-UK - INTEGRATED DEMAND 5",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 266,
                Code = "5PD",
                Name = "SK-CS-UK - 5 PARTNERSHIPS DIGI",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 267,
                Code = "5PA",
                Name = "SK-CS-UK - 5 PARTNERSHIPS NETW",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 268,
                Code = "5NN",
                Name = "SK-CS-UK - 5 NORTH & SHELL NWK",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 269,
                Code = "5ND",
                Name = "SK-CS-UK - 5 NORTH & SHELL DIG",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 270,
                Code = "5KB",
                Name = "KI-CS-UK - 5 KIDS BARTER TRADE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 271,
                Code = "5DE",
                Name = "DG-NS-UK - DEMAND 5",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 272,
                Code = "5DB",
                Name = "DG-NS-UK - DEMAND 5 BARTER",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 273,
                Code = "5DI",
                Name = "DG-NM-UK - INTEG DEMAND 5 BART",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 274,
                Code = "5SB",
                Name = "SP-NS-UK - C5 INTEG SPONS BART",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 275,
                Code = "SVS",
                Name = "SP-NS-UK - SKY GO VOD SPONSORS",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 276,
                Code = "TVS",
                Name = "SP-NS-UK - TV VOD SPONSORSHIP",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 277,
                Code = "5SH",
                Name = "SK-CS-UK - 5 SHARED REWARD NWK",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 278,
                Code = "5SR",
                Name = "SK-CS-UK - 5 SHARED REWARD DIG",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 279,
                Code = "KP5",
                Name = "KI-CS-UK - 5 KIDS PARTNERSHIP",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 280,
                Code = "SHP",
                Name = "PU-NM-UK - PUB SHARED REWARD",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 281,
                Code = "5NP",
                Name = "SK-CS-UK - 5 INTEG PARTNERS NW",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 282,
                Code = "5DP",
                Name = "SK-CS-UK - 5 INTEG PARTNER DIG",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 283,
                Code = "5DS",
                Name = "SP-NS-UK - DEMAND 5 SPONSORS",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 284,
                Code = "ODS",
                Name = "SP-NS-UK - ON DEMAND SPONSORS",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 285,
                Code = "SKG",
                Name = "DG-NS-UK - SKY GO LIN VOD NS",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 286,
                Code = "CCB",
                Name = "SK-CS-UK - CONTENT COMM BUY",
                IncludeConversionIndex = true
            },
            new BusinessType {
                Id = 287,
                Code = "CCV",
                Name = "SK-CS-UK - CONTENT COMM VALUE",
                IncludeConversionIndex = true
            },
            new BusinessType {
                Id = 288,
                Code = "CFB",
                Name = "SK-CS-UK - CONTENT FUND BUY",
                IncludeConversionIndex = true
            },
            new BusinessType {
                Id = 289,
                Code = "CFV",
                Name = "SK-CS-UK - CONTENT FUND VALUE",
                IncludeConversionIndex = true
            },
            new BusinessType {
                Id = 290,
                Code = "CSB",
                Name = "SK-CS-UK - CONTENT STUDIO BUY",
                IncludeConversionIndex = true
            },
            new BusinessType {
                Id = 291,
                Code = "CSV",
                Name = "SK-CS-UK - CONTENT STUDIO VALU",
                IncludeConversionIndex = true
            },
            new BusinessType {
                Id = 292,
                Code = "NOR",
                Name = "SK-CS-UK - NORTHERN & SHELL",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 293,
                Code = "5BU",
                Name = "SK-CS-UK - 5 PROD FIN BUY",
                IncludeConversionIndex = true
            },
            new BusinessType {
                Id = 294,
                Code = "5VA",
                Name = "SK-CS-UK - 5 PROD FIN VALUE",
                IncludeConversionIndex = true
            },
            new BusinessType {
                Id = 295,
                Code = "CFI",
                Name = "SK-CS-UK - CONTENT FINANCE BUY",
                IncludeConversionIndex = true
            },
            new BusinessType {
                Id = 296,
                Code = "CVA",
                Name = "SK-CS-UK - CONTENT FINANCE VAL",
                IncludeConversionIndex = true
            },
            new BusinessType {
                Id = 297,
                Code = "BIK",
                Name = "BI-CS-UK - BIKE CHAN SALES HS",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 298,
                Code = "COS",
                Name = "SP-NS-UK - CONTRA SPONSORSHIP",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 299,
                Code = "SRB",
                Name = "SK-CS-UK - SHARED RWARD BARTER",
                IncludeConversionIndex = true
            },
            new BusinessType {
                Id = 300,
                Code = "PER",
                Name = "SK-CS-UK - PERFORM SOLUTIONS",
                IncludeConversionIndex = true
            },
            new BusinessType {
                Id = 301,
                Code = "TIM",
                Name = "TI-CS-UK - TIMES NOW SALES HSE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 302,
                Code = "SGC",
                Name = "DG-NS-UK - SKY GO ONE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 303,
                Code = "SVC",
                Name = "DG-NS-IR - IRELAND SKY GO ONE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 305,
                Code = "KSB",
                Name = "KI-CS-UK - CONTENT STU KID BUY",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 306,
                Code = "KSV",
                Name = "KI-CS-UK - CONTENT STU KID VAL",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 307,
                Code = "PSB",
                Name = "PU-NM-UK - CONTENT COM PUB BUY",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 308,
                Code = "PSV",
                Name = "PU-NM-UK - CONTENT COM PUB VAL",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 309,
                Code = "CVK",
                Name = "KI-CS-UK - CONTENT FIN KID VAL",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 310,
                Code = "TVB",
                Name = "DG-NS-UK - INTEG TV VOD BARTER",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 311,
                Code = "TBA",
                Name = "DG-NS-UK - TV VOD BARTER",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 312,
                Code = "VIC",
                Name = "VL-CS-UK - VICELAND SALES HSE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 313,
                Code = "BAC",
                Name = "SK-CS-UK - BARTER TRADE PB",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 314,
                Code = "CPB",
                Name = "SK-CS-UK - PROG FINANCE PB",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 315,
                Code = "ADI",
                Name = "SK-CS-IR - ADSMART IRELAND",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 316,
                Code = "CPK",
                Name = "KI-CS-UK - PROG FINANCE PB KID",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 317,
                Code = "S4R",
                Name = "SK-CS-UK - S4C SHARED REWARD",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 318,
                Code = "IND",
                Name = "DG-NM-UK - INT ON DEM BAR PB",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 319,
                Code = "KBT",
                Name = "KI-CS-UK - KIDS BART TRD PB",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 320,
                Code = "S4T",
                Name = "SK-CS-UK - S4C BART TRADE CRED",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 321,
                Code = "MSO",
                Name = "DG-NS-UK - MY SKY ONE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 322,
                Code = "TVP",
                Name = "DG-NS-UK - TV VOD PF BUY",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 323,
                Code = "NOK",
                Name = "KI-CS-UK - NORTH & SHELL KIDS",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 324,
                Code = "CFD",
                Name = "SK-CS-UK - PROG FIN DRTV PB",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 325,
                Code = "CFK",
                Name = "KI-CS-UK - PROG FIN KID DRT PB",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 326,
                Code = "TVF",
                Name = "DG-NS-UK - TV VOD PF VAL",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 327,
                Code = "TSH",
                Name = "SK-TS-UK - LONG FORM TELESHOPP",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 328,
                Code = "SKC",
                Name = "DG-NS-UK - SKY GO VOD BART TC",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 329,
                Code = "BAD",
                Name = "SK-CS-UK - BARTER TRADE DRTV",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 330,
                Code = "BKS",
                Name = "BI-NS-UK - BIKE CHAN SPONSORS",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 331,
                Code = "PVD",
                Name = "DG-VS-UK - PUSH VOD",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 332,
                Code = "INT",
                Name = "DG-NM-UK - ON DEM BAR TRAD CRD",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 333,
                Code = "PVB",
                Name = "DG-VS-UK - PUSH VOD BARTER",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 334,
                Code = "ALV",
                Name = "DG-NS-UK - ALL VOD",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 335,
                Code = "BCV",
                Name = "DG-NS-UK - BIG SCREEN VOD",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 336,
                Code = "COV",
                Name = "DG-NS-UK - CLICKABLE ONLY VOD",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 337,
                Code = "SOB",
                Name = "DG-NS-UK - SKY GO ONE BARTER",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 338,
                Code = "SGV",
                Name = "DG-NS-IR - IRE SKY GO VOD BART",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 339,
                Code = "SPF",
                Name = "SK-CS-UK - SHARED RWD PROG FIN",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 340,
                Code = "5DF",
                Name = "DG-NS-UK - DEMAND 5 PROG FIN",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 341,
                Code = "SGF",
                Name = "DG-NS-UK - SKY GO ONE PROG FIN",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 342,
                Code = "ADP",
                Name = "SK-CS-UK - ADSMART PROG FIN BUY",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 343,
                Code = "TVC",
                Name = "DG-NS-UK - TV VOD BARTER TC",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 344,
                Code = "ATR",
                Name = "AT-CS-UK - AT RACES SALES HSE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 345,
                Code = "ATI",
                Name = "AT-CS-IR - AT RACES SLS HSE IR",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 346,
                Code = "SIC",
                Name = "SK-CS-IR - SPONS IRELAND CS",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 347,
                Code = "S4P",
                Name = "SK-CS-UK - S4C PROGRAM FINANCE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 348,
                Code = "SGM",
                Name = "SK-NM-UK - SKY GO LIN PROG FIN",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 349,
                Code = "PUP",
                Name = "PU-NM-UK - PUB PROG FINANCE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 350,
                Code = "SGR",
                Name = "SK-NM-UK - SGL PROG FIN VALUE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 351,
                Code = "ADV",
                Name = "SK-CS-UK - ADSMART THIRD PARTY",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 352,
                Code = "PBC",
                Name = "PU-NM-UK - PUB BART TRAD CRED",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 353,
                Code = "AVB",
                Name = "DG-NS-UK - ALL VOD PF BUY",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 354,
                Code = "AVV",
                Name = "DG-NS-UK - ALL VOD PF VAL",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 355,
                Code = "ABN",
                Name = "AB-CS-UK - ABN SALES HOUSE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 356,
                Code = "ATN",
                Name = "AN-CS-UK - ATN SALES HOUSE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 357,
                Code = "B4U",
                Name = "BU-CS-UK - B4U SALES HOUSE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 358,
                Code = "DUN",
                Name = "DU-CS-UK - DUNYA SALES HOUSE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 359,
                Code = "HUM",
                Name = "HU-CS-UK - HUM SALES HOUSE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 360,
                Code = "PTC",
                Name = "PT-CS-UK - PTC SALES HOUSE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 361,
                Code = "PTV",
                Name = "PV-CS-UK - PTV SALES HOUSE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 362,
                Code = "NOL",
                Name = "NO-CS-UK - NOLLYWOOD SALES HSE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 363,
                Code = "ROK",
                Name = "RO-CS-UK - ROK SALES HOUSE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 364,
                Code = "ALB",
                Name = "DG-NS-UK - ALL VOD BARTER",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 365,
                Code = "ALL",
                Name = "AL-CS-UK - ALLIANCE SALES HSE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 366,
                Code = "PRS",
                Name = "PS-CS-UK - PRIDESTONE SALES HS",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 367,
                Code = "BCB",
                Name = "DG-NS-UK - BIG SCREEN VOD BART",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 368,
                Code = "COB",
                Name = "DG-NS-UK - CLICKABLE VOD BART",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 369,
                Code = "BSA",
                Name = "DG-NS-UK - BIG SCREEN PF BUY",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 370,
                Code = "BSB",
                Name = "DG-NS-UK - BIG SCRN PF BUY BAR",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 371,
                Code = "BSC",
                Name = "DG-NS-UK - BIG SCREEN PF VALUE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 372,
                Code = "BSD",
                Name = "DG-NS-UK - BIG SCRN PF VAL BAR",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 373,
                Code = "CLA",
                Name = "DG-NS-UK - CLICKABLE PF BUY",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 374,
                Code = "CLB",
                Name = "DG-NS-UK - CLICKABLE PF BUY BA",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 375,
                Code = "CLC",
                Name = "DG-NS-UK - CLICKABLE PF VAL",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 376,
                Code = "CLD",
                Name = "DG-NS-UK - CLICKABLE PF VAL BA",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 377,
                Code = "COP",
                Name = "PU-NM-UK - PUB CONTRA AIRTIME",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 378,
                Code = "ADC",
                Name = "SK-CS-UK - ADSMART CONTRA",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 379,
                Code = "HUS",
                Name = "HU-NS-UK - HUM SPONSORSHIP",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 380,
                Code = "SGT",
                Name = "SK-NM-UK - SKY GO BARTER TC",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 381,
                Code = "FSB",
                Name = "DG-NS-UK - FIXED SPOT VOD BM",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 382,
                Code = "FSV",
                Name = "DG-NS-UK - FIXED SPOT VOD",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 383,
                Code = "STR",
                Name = "ST-NS-UK - STAR TV SPONSORSHIP",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 384,
                Code = "SGU",
                Name = "DG-NS-UK - SKY GO LIN ENTRY AD",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 385,
                Code = "SVO",
                Name = "DG-NS-UK - KIDS VOD",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 386,
                Code = "ZTP",
                Name = "SP-NS-UK - ZEE TV SPONSORSHIP",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 387,
                Code = "DRZ",
                Name = "SK-CS-UK - ZERO RATING DRTV",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 388,
                Code = "SSB",
                Name = "SP-NS-UK - SONY SAB SPONSORSHI",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 389,
                Code = "S4H",
                Name = "SK-CS-UK - S4C SALES HOUSE",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 390,
                Code = "STB",
                Name = "SP-NS-UK - TV VOD SPONS BARTER",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 391,
                Code = "SBS",
                Name = "SP-NS-UK - SKY GO VOD SPON BAR",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 392,
                Code = "ADR",
                Name = "SK-CS-IR - ADSMART IRE NI",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 393,
                Code = "SKA",
                Name = "SP-NS-UK - KIDS VOD",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 394,
                Code = "SGE",
                Name = "DG-NS-IR - IRELAND SKY GO LIN ENTRY AD",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 395,
                Code = "PPF",
                Name = "SK-CS-UK - PERFORMANCE SOLUTIONS PROG FIN",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 396,
                Code = "PVA",
                Name = "DG-NS-UK - PUSH VOD PF BUY",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 397,
                Code = "PVF",
                Name = "DG-NS-UK - PUSH VOD PF VAL",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 398,
                Code = "TSI",
                Name = "SP-NS-IR - TV VOD SPONSORSHIP IRELAND",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 399,
                Code = "SSI",
                Name = "SP-NS-IR - SKY GO VOD SPONSORS IRELAND",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 400,
                Code = "ADF",
                Name = "SK-CS-UK - ADSMART CONTENT FINANCE VAL",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 401,
                Code = "ADY",
                Name = "SK-CS-UK - ADSMART CONTENT FINANCE BUY",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 402,
                Code = "STN",
                Name = "SK-CS-UK - NBCU STANDARD",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 403,
                Code = "KVP",
                Name = "DG-NS-UK - KIDS VOD PROG FIN",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 404,
                Code = "SCM",
                Name = "SP-NS-UK - SKY CRIME SPONSORSHIP",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 405,
                Code = "SCO",
                Name = "SP-NS-UK - SKY COMEDY SPONSORSHIP",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 407,
                Code = "APV",
                Name = "SK-CS-UK - ADSMART PROG FIN VAL",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 408,
                Code = "BPK",
                Name = "KI-CS-UK - BROADCAST PROMOTION KIDS",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 409,
                Code = "SDO",
                Name = "SP-NS-UK - SKY DOCS SPONSORSHIP",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 410,
                Code = "SNA",
                Name = "SP-NS-UK - SKY NATURE SPONSORSHIP",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 411,
                Code = "KVI",
                Name = "DG-NS-IR - IRELAND KIDS VOD",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 412,
                Code = "ADL",
                Name = "SK-CS-UK - ADSMART LOCAL",
                IncludeConversionIndex = false
            },
            new BusinessType {
                Id = 413,
                Code = "BTH",
                Name = "BT-CS-UK - BT SALES HOUSE",
                IncludeConversionIndex = false
            }
        };

        private static void ValidateParametersBeforeUse(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            _ = UpdateValidator.ValidateTenantConnectionString(tenantConnectionStrings, throwOnInvalid: true);
            _ = UpdateValidator.ValidateUpdateFolderPath(updatesFolder, throwOnInvalid: true);
        }

        public void RollBack() => throw new NotImplementedException();
    }
}
