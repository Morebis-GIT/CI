using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT17665_ProgrammeOptimization : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.Sql(
                @"IF EXISTS (
	                SELECT c.fulltext_catalog_id FROM sys.fulltext_catalogs c
	                JOIN sys.fulltext_indexes i ON i.fulltext_catalog_id = c.fulltext_catalog_id
	                WHERE OBJECT_NAME(i.[object_id]) = 'Programmes')
	                DROP FULLTEXT INDEX ON Programmes", suppressTransaction: true);

            _ = migrationBuilder.CreateTable(
                name: "ProgrammeDictionaries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ExternalReference = table.Column<string>(maxLength: 64, nullable: false),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    Description = table.Column<string>(maxLength: 512, nullable: true),
                    Classification = table.Column<string>(maxLength: 64, nullable: true),
                    TokenizedName = table.Column<string>(nullable: true, computedColumnSql: "CONCAT_WS(' ', ExternalReference, Name)")
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_ProgrammeDictionaries", x => x.Id);
                });

            _ = migrationBuilder.CreateIndex(
                name: "IX_ProgrammesDictionaries_ExternalReference",
                table: "ProgrammeDictionaries",
                column: "ExternalReference",
                unique: true);

            _ = migrationBuilder.Sql(
                @"INSERT INTO ProgrammeDictionaries
                  (ExternalReference, [Name], [Description], [Classification])
                  SELECT p.ExternalReference, p.ProgrammeName, p.[Description], p.[Classification] FROM (SELECT DISTINCT ExternalReference FROM Programmes) as t
                  JOIN Programmes p ON t.ExternalReference = p.ExternalReference
                  WHERE p.Id = (SELECT TOP 1 pl.Id FROM Programmes pl WHERE pl.ExternalReference = p.ExternalReference)");

            _ = migrationBuilder.DropTable(
                name: "ProgrammesDictionaries");

            _ = migrationBuilder.CreateTable(
                name: "ProgrammeCategories_Temp",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_ProgrammeCategories_Temp", x => x.Id);
                });

            _ = migrationBuilder.Sql(
                @"INSERT INTO ProgrammeCategories_Temp
                  ([Name])
                  SELECT DISTINCT [Name] FROM ProgrammeCategories");

            _ = migrationBuilder.CreateTable(
                name: "ProgrammeCategoryLinks",
                columns: table => new
                {
                    ProgrammeId = table.Column<Guid>(nullable: false),
                    ProgrammeCategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_ProgrammeCategoryLinks", x => new { x.ProgrammeId, x.ProgrammeCategoryId });
                });

            _ = migrationBuilder.Sql(
                @"INSERT INTO ProgrammeCategoryLinks
                  (ProgrammeId, ProgrammeCategoryId)
                  SELECT DISTINCT p.Id, tpc.Id FROM ProgrammeCategories pc
                  JOIN ProgrammeCategories_Temp tpc ON pc.[Name] = tpc.[Name]
                  JOIN Programmes p ON p.Id = pc.ProgrammeId");

            _ = migrationBuilder.DropTable(
                name: "ProgrammeCategories");

            _ = migrationBuilder.RenameTable(
                name: "ProgrammeCategories_Temp",
                newName: "ProgrammeCategories");

            _ = migrationBuilder.RenameIndex(
                name: "PK_ProgrammeCategories_Temp",
                table: "ProgrammeCategories",
                newName: "PK_ProgrammeCategories");

            _ = migrationBuilder.AddForeignKey(
                name: "FK_ProgrammeCategoryLinks_Programmes_ProgrammeId",
                table: "ProgrammeCategoryLinks",
                column: "ProgrammeId",
                principalTable: "Programmes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            _ = migrationBuilder.AddForeignKey(
                name: "FK_ProgrammeCategoryLinks_ProgrammeCategories_ProgrammeCategoryId",
                table: "ProgrammeCategoryLinks",
                column: "ProgrammeCategoryId",
                principalTable: "ProgrammeCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            _ = migrationBuilder.CreateIndex(
                name: "IX_ProgrammeCategories_Name",
                table: "ProgrammeCategories",
                column: "Name",
                unique: true);

            _ = migrationBuilder.CreateTable(
                name: "ProgrammeEpisodes_Temp",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProgrammeDictionaryId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 64, nullable: false),
                    Number = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_ProgrammeEpisodes_Temp", x => x.Id);
                });

            _ = migrationBuilder.Sql(
                @"INSERT INTO ProgrammeEpisodes_Temp
                  (ProgrammeDictionaryId, [Name], Number)
                  SELECT pd.Id, pe.[Name], pe.Number FROM (SELECT DISTINCT ProgrammeExternalReference, Number FROM ProgrammeEpisodes) AS t
                  JOIN ProgrammeEpisodes pe ON pe.ProgrammeExternalReference = t.ProgrammeExternalReference AND pe.Number = t.Number
                  JOIN ProgrammeDictionaries pd ON pd.ExternalReference = pe.ProgrammeExternalReference
                  WHERE pe.Id = (SELECT TOP 1 pel.Id FROM ProgrammeEpisodes pel WHERE pel.ProgrammeExternalReference = pe.ProgrammeExternalReference AND pel.Number = pe.Number)");

            _ = migrationBuilder.AddColumn<int>(
                name: "ProgrammeDictionaryId",
                table: "Programmes",
                nullable: true);

            _ = migrationBuilder.AddColumn<int>(
                name: "EpisodeId_New",
                table: "Programmes",
                nullable: true);

            _ = migrationBuilder.Sql(
                @"UPDATE Programmes
                  SET ProgrammeDictionaryId = t.ProgrammeDictionaryId,
	                  EpisodeId_New = t.EpisodeId
                  FROM Programmes p JOIN (
                  SELECT p.Id, pd.Id AS ProgrammeDictionaryId, pet.Id AS EpisodeId FROM Programmes p
                  JOIN ProgrammeDictionaries pd ON pd.ExternalReference = p.ExternalReference
                  LEFT JOIN ProgrammeEpisodes pe ON pe.Id = p.EpisodeId
                  LEFT JOIN ProgrammeDictionaries pde ON pde.ExternalReference = pe.ProgrammeExternalReference
                  LEFT JOIN ProgrammeEpisodes_Temp pet ON pet.ProgrammeDictionaryId = pde.Id) AS t ON p.Id = t.Id");

            _ = migrationBuilder.AlterColumn<int>(
                name: "ProgrammeDictionaryId",
                table: "Programmes",
                nullable: false,
                oldClrType: typeof(int));

            _ = migrationBuilder.DropForeignKey(
                name: "FK_Programmes_ProgrammeEpisodes_EpisodeId",
                table: "Programmes");

            _ = migrationBuilder.DropIndex(
                name: "IX_Programmes_EpisodeId",
                table: "Programmes");

            _ = migrationBuilder.DropColumn(
                name: "EpisodeId",
                table: "Programmes");

            _ = migrationBuilder.RenameColumn(
                name: "EpisodeId_New",
                table: "Programmes",
                newName: "EpisodeId");

            _ = migrationBuilder.DropIndex(
                name: "IX_Programmes_ExternalReference",
                table: "Programmes");

            _ = migrationBuilder.DropColumn(
                name: "TokenizedName",
                table: "Programmes");

            _ = migrationBuilder.DropColumn(
                name: "ExternalReference",
                table: "Programmes");

            _ = migrationBuilder.DropColumn(
                name: "ProgrammeName",
                table: "Programmes");

            _ = migrationBuilder.DropColumn(
                name: "Description",
                table: "Programmes");

            _ = migrationBuilder.DropColumn(
                name: "Classification",
                table: "Programmes");

            _ = migrationBuilder.DropTable(
                name: "ScheduleProgrammeCategories");

            _ = migrationBuilder.Sql(
                @"DELETE FROM ScheduleProgrammes
                  WHERE Id IN (SELECT sp.Id FROM ScheduleProgrammes sp
                  LEFT JOIN Programmes p ON p.Id = sp.Id
                  WHERE p.Id IS NULL)");

            _ = migrationBuilder.DropIndex(
                name: "IX_ScheduleProgrammes_EpisodeId",
                table: "ScheduleProgrammes");

            _ = migrationBuilder.DropIndex(
                name: "IX_ScheduleProgrammes_ExternalReference",
                table: "ScheduleProgrammes");

            _ = migrationBuilder.DropIndex(
                name: "IX_ScheduleProgrammes_SalesArea",
                table: "ScheduleProgrammes");

            _ = migrationBuilder.DropIndex(
                name: "IX_ScheduleProgrammes_ScheduleId",
                table: "ScheduleProgrammes");

            _ = migrationBuilder.DropForeignKey(
                name: "FK_ScheduleProgrammes_ProgrammeEpisodes_EpisodeId",
                table: "ScheduleProgrammes");

            _ = migrationBuilder.DropColumn(
                name: "PrgtNo",
                table: "ScheduleProgrammes");

            _ = migrationBuilder.DropColumn(
                name: "SalesArea",
                table: "ScheduleProgrammes");

            _ = migrationBuilder.DropColumn(
                name: "StartDateTime",
                table: "ScheduleProgrammes");

            _ = migrationBuilder.DropColumn(
                name: "Duration",
                table: "ScheduleProgrammes");

            _ = migrationBuilder.DropColumn(
                name: "ExternalReference",
                table: "ScheduleProgrammes");

            _ = migrationBuilder.DropColumn(
                name: "ProgrammeName",
                table: "ScheduleProgrammes");

            _ = migrationBuilder.DropColumn(
                name: "Description",
                table: "ScheduleProgrammes");

            _ = migrationBuilder.DropColumn(
                name: "Classification",
                table: "ScheduleProgrammes");

            _ = migrationBuilder.DropColumn(
                name: "LiveBroadcast",
                table: "ScheduleProgrammes");

            _ = migrationBuilder.DropColumn(
                name: "EpisodeId",
                table: "ScheduleProgrammes");

            _ = migrationBuilder.RenameColumn(
                name: "Id",
                table: "ScheduleProgrammes",
                newName: "ProgrammeId");

            _ = migrationBuilder.AddForeignKey(
                name: "FK_ScheduleProgrammes_Programmes_ProgrammeId",
                table: "ScheduleProgrammes",
                column: "ProgrammeId",
                principalTable: "Programmes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            _ = migrationBuilder.CreateIndex(
                    name: "IX_ScheduleProgrammes_ScheduleId",
                    table: "ScheduleProgrammes",
                    column: "ScheduleId")
                .Annotation("SqlServer:Include", new[] { "ProgrammeId" });

            _ = migrationBuilder.DropTable(
                name: "ProgrammeEpisodes");

            _ = migrationBuilder.RenameTable(
                name: "ProgrammeEpisodes_Temp",
                newName: "ProgrammeEpisodes");

            _ = migrationBuilder.RenameIndex(
                name: "PK_ProgrammeEpisodes_Temp",
                table: "ProgrammeEpisodes",
                newName: "PK_ProgrammeEpisodes");

            _ = migrationBuilder.CreateIndex(
                name: "IX_ProgrammeEpisodes_ProgrammeDictionaryId",
                table: "ProgrammeEpisodes",
                column: "ProgrammeDictionaryId");

            _ = migrationBuilder.AddForeignKey(
                name: "FK_ProgrammeEpisodes_ProgrammeDictionaries_ProgrammeDictionaryId",
                table: "ProgrammeEpisodes",
                column: "ProgrammeDictionaryId",
                principalTable: "ProgrammeDictionaries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            _ = migrationBuilder.CreateIndex(
                name: "IX_Programmes_EpisodeId",
                table: "Programmes",
                column: "EpisodeId");

            _ = migrationBuilder.CreateIndex(
                    name: "IX_Programmes_ProgrammeDictionaryId",
                    table: "Programmes",
                    column: "ProgrammeDictionaryId")
                .Annotation("SqlServer:Include", new[] { "LiveBroadcast" });

            _ = migrationBuilder.AddForeignKey(
                name: "FK_Programmes_ProgrammeDictionaries_ProgrammeDictionaryId",
                table: "Programmes",
                column: "ProgrammeDictionaryId",
                principalTable: "ProgrammeDictionaries",
                principalColumn: "Id");

            _ = migrationBuilder.AddForeignKey(
                name: "FK_Programmes_ProgrammeEpisodes_EpisodeId",
                table: "Programmes",
                column: "EpisodeId",
                principalTable: "ProgrammeEpisodes",
                principalColumn: "Id");

            _ = migrationBuilder.Sql(
                @"ALTER VIEW ProgrammesExternalRefs AS SELECT [Name] as ProgrammeName, ExternalReference FROM ProgrammeDictionaries");

            _ = migrationBuilder.Sql(
                @"CREATE FULLTEXT INDEX ON ProgrammeDictionaries KEY INDEX PK_ProgrammeDictionaries ON (Programmes) WITH (CHANGE_TRACKING AUTO)", suppressTransaction: true);

            _ = migrationBuilder.Sql(
                @"ALTER FULLTEXT INDEX ON ProgrammeDictionaries ADD (TokenizedName)", suppressTransaction: true);

            _ = migrationBuilder.Sql(
                @"ALTER FULLTEXT INDEX ON ProgrammeDictionaries ENABLE", suppressTransaction: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            throw new NotSupportedException($"{nameof(XGGT17665_ProgrammeOptimization)} migration is not revertible");
        }
    }
}
