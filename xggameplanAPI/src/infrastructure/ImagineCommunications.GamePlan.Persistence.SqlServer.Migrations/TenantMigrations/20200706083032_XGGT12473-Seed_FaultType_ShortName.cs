using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT12473Seed_FaultType_ShortName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Out Camp Strk Wght' WHERE FaultTypeId = 1
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Prgm not in Req' WHERE FaultTypeId = 2
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Prgm Cat not in Req' WHERE FaultTypeId = 3
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Prgm Req Incl Mismatch' WHERE FaultTypeId = 4
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Spot Lgth Inval for Prd' WHERE FaultTypeId = 5
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Pos Req not avlbl' WHERE FaultTypeId = 6
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = '0 req for Lgth' WHERE FaultTypeId = 8
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Out Camp Dprt' WHERE FaultTypeId = 9
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Not Enough Avail in Brk' WHERE FaultTypeId = 10
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Cntr/End Req' WHERE FaultTypeId = 11
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Time not in Req' WHERE FaultTypeId = 12
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Brk Type' WHERE FaultTypeId = 13
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Clash det' WHERE FaultTypeId = 14
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Restrictions' WHERE FaultTypeId = 15
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Clash w/self det' WHERE FaultTypeId = 16
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Max 0 Rtgs' WHERE FaultTypeId = 19
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Effcy/Rank Lim' WHERE FaultTypeId = 20
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Camp Req (Ach/Ovrsp)' WHERE FaultTypeId = 21
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'SA Req (Ach/Ovrsp)' WHERE FaultTypeId = 22
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Cntr/End Req (Ach/Ovrsp)' WHERE FaultTypeId = 23
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Strk Wght Req (Ach/Ovrsp)' WHERE FaultTypeId = 24
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Strk Wght Len Req (Ach/Ovrsp)' WHERE FaultTypeId = 25
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Strk Wght/Dprt Req (Ach/Ovrsp)' WHERE FaultTypeId = 26
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Dprt Req (Ach/Ovrsp)' WHERE FaultTypeId = 27
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Spot Len Req (Ach/Ovrsp)' WHERE FaultTypeId = 28
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Book Pos Req (Ach/Ovrsp)' WHERE FaultTypeId = 29
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Max Spots/Day' WHERE FaultTypeId = 30
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Max Spots/Hr' WHERE FaultTypeId = 31
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Max Spots/2 Hrs' WHERE FaultTypeId = 32
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Min Brk Btw Spots' WHERE FaultTypeId = 33
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Max Spots/Prgm/Day' WHERE FaultTypeId = 34
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Max Spots/Prgm/Wk' WHERE FaultTypeId = 35
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Max Spots/Prgm/100 rtg' WHERE FaultTypeId = 36
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Min Wks Btw Prgm' WHERE FaultTypeId = 37
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Max Rtg for Spot Camp' WHERE FaultTypeId = 38
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Max Spots/Prgm/Time' WHERE FaultTypeId = 39
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Min Days Btw Prgm/Time' WHERE FaultTypeId = 40
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Min Wks Btw Prgm/Time' WHERE FaultTypeId = 41
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Min Hrs Btw Spots' WHERE FaultTypeId = 42
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Min Brk Avail' WHERE FaultTypeId = 43
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Split Book Not Alwd' WHERE FaultTypeId = 44
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Spnsp Clash det' WHERE FaultTypeId = 45
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Rgnl Brk book' WHERE FaultTypeId = 47
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Excl Camp Pkgs' WHERE FaultTypeId = 48
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'SA Req (Tgt Ach/Ovrsp)' WHERE FaultTypeId = 49
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Strk Wght Req (Tgt Ach/Ovrsp)' WHERE FaultTypeId = 50
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Strk Wght Len Req (Tgt Ach/Ovrsp)' WHERE FaultTypeId = 51
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Strk Wght/Dprt Req (Ach/Ovrsp)' WHERE FaultTypeId = 52
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Dprt Req (Tgt Ach/Ovrsp)' WHERE FaultTypeId = 53
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Spot Len Req (Tgt Ach/Ovrsp)' WHERE FaultTypeId = 54
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Existing Spot' WHERE FaultTypeId = 55
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'No avlbl Spot Tps' WHERE FaultTypeId = 56
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Camp Rev Req (Ach/Ovrsp)' WHERE FaultTypeId = 57
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'SA Rev Req (Ach/Oversp)' WHERE FaultTypeId = 58
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Camp Rtcd Restr' WHERE FaultTypeId = 59
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Bonus Book Req' WHERE FaultTypeId = 60
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Strk Wght/Dprt Lght Req (Ach/Ovrsp)' WHERE FaultTypeId = 61
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Dprt Len Req (Ach/Ovrsp)' WHERE FaultTypeId = 62
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Strk Wght Len Req (Ach/Ovrsp)' WHERE FaultTypeId = 63
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Strk Wght/Dprt. Len Req (Tgt Ach/Ovrsp)' WHERE FaultTypeId = 64
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Dprt Len Req (Tgt. Ach/Ovrsp)' WHERE FaultTypeId = 65
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Strk Wght Len Req (Tgt Ach/Ovrsp)' WHERE FaultTypeId = 66
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'No avlbl Sched Pmts' WHERE FaultTypeId = 67
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Floor Rate' WHERE FaultTypeId = 68
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Macro Book' WHERE FaultTypeId = 69
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Brk Demo Restr' WHERE FaultTypeId = 70
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Zero Rate Brks' WHERE FaultTypeId = 71
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Spot Len Book Rules' WHERE FaultTypeId = 72
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Max Rtgs for Rtg Camp' WHERE FaultTypeId = 73
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Bus Type Rev Req (Ach/Ovrsp)' WHERE FaultTypeId = 74
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Brk Type Req (Ach/Ovrsp)' WHERE FaultTypeId = 75
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Brk Type Rev Req (Ach/Ovrsp)' WHERE FaultTypeId = 76
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Strk Wght Rev Req (Ach/Oversp)' WHERE FaultTypeId = 77
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Prgm Req (Ach/Ovrsp)' WHERE FaultTypeId = 78
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Sponsorship Restr…<Clash>' WHERE FaultTypeId = 79
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Sponsorship Restr…<Advertiser>' WHERE FaultTypeId = 80
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Mob Restr' WHERE FaultTypeId = 81
                UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = 'Min TARP`s not met' WHERE FaultTypeId = 82
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE [dbo].[FaultTypeDescriptions] SET ShortName = NULL");
        }
    }
}
