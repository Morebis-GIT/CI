using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi.DefaultParameters;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.AutoBookApi.DefaultParameters
{
    public class AutoBookDefaultParametersEntityConfiguration : IEntityTypeConfiguration<AutoBookDefaultParameters>
    {
        public void Configure(EntityTypeBuilder<AutoBookDefaultParameters> builder)
        {
            builder.ToTable("AutoBookDefaultParameters");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasDefaultValueSql("newid()");            

            builder.Property(e => e.AgBreak_ScheduledDate).HasMaxLength(32);
            builder.Property(e => e.AgBreak_ExternalNo).HasMaxLength(64);
            builder.Property(e => e.AgBreak_NominalTime).HasMaxLength(32);
            builder.Property(e => e.AgBreak_Solus).HasMaxLength(32);
            builder.Property(e => e.AgBreak_PositionInProg).HasMaxLength(64);
            builder.Property(e => e.AgBreak_BreakTypeCode).HasMaxLength(32);
            builder.Property(e => e.AgBreak_LongForm).HasMaxLength(512);

            builder.Property(e => e.AgCampaign_ExternalNo).HasMaxLength(64);
            builder.Property(e => e.AgCampaign_ClearanceCode).HasMaxLength(64);
            builder.Property(e => e.AgCampaign_MultiPartFlag).HasMaxLength(64);
            builder.Property(e => e.AgCampaign_StartDate).HasMaxLength(32);
            builder.Property(e => e.AgCampaign_EndDate).HasMaxLength(32);
            builder.Property(e => e.AgCampaign_RootClashCode).HasMaxLength(64);
            builder.Property(e => e.AgCampaign_ClashCode).HasMaxLength(64);
            builder.Property(e => e.AgCampaign_AdvertiserIdentifier).HasMaxLength(256);

            builder.Property(e => e.AgExposure_ClashCode).HasMaxLength(64);
            builder.Property(e => e.AgExposure_MasterClashCode).HasMaxLength(64);
            builder.Property(e => e.AgExposure_StartDate).HasMaxLength(32);
            builder.Property(e => e.AgExposure_EndDate).HasMaxLength(32);
            builder.Property(e => e.AgExposure_StartTime).HasMaxLength(32);
            builder.Property(e => e.AgExposure_EndTime).HasMaxLength(32);

            builder.Property(e => e.AgISRTimeBand_StartTime).HasMaxLength(32);
            builder.Property(e => e.AgISRTimeBand_EndTime).HasMaxLength(32);
            builder.Property(e => e.AgISRTimeBand_Exclude).HasMaxLength(16);

            builder.Property(e => e.AgPeakStartEndTime_StartTimeOfDayPart).HasMaxLength(32);
            builder.Property(e => e.AgPeakStartEndTime_EndTimeOfDayPart).HasMaxLength(32);
            builder.Property(e => e.AgPeakStartEndTime_Name).HasMaxLength(256);

            builder.Property(e => e.AgProgRestriction_IncludeExcludeFlag).HasMaxLength(64);

            builder.Property(e => e.AgProgTxDetail_TxDate).HasMaxLength(32);
            builder.Property(e => e.AgProgTxDetail_ScheduledStartTime).HasMaxLength(32);
            builder.Property(e => e.AgProgTxDetail_ScheduledEndTime).HasMaxLength(32);
            builder.Property(e => e.AgProgTxDetail_ClassCode).HasMaxLength(64);
            builder.Property(e => e.AgProgTxDetail_LiveBroadcast).HasMaxLength(16);

            builder.Property(e => e.AgRestriction_ClashCode).HasMaxLength(64);
            builder.Property(e => e.AgRestriction_CopyCode).HasMaxLength(64).IsRequired();
            builder.Property(e => e.AgRestriction_ClearanceCode).HasMaxLength(64);
            builder.Property(e => e.AgRestriction_StartDate).HasMaxLength(32);
            builder.Property(e => e.AgRestriction_EndDate).HasMaxLength(32);
            builder.Property(e => e.AgRestriction_PublicHolidayIndicator).HasMaxLength(64);
            builder.Property(e => e.AgRestriction_SchoolHolidayIndicator).HasMaxLength(64);
            builder.Property(e => e.AgRestriction_StartTime).HasMaxLength(32);
            builder.Property(e => e.AgRestriction_EndTime).HasMaxLength(32);
            builder.Property(e => e.AgRestriction_ProgClassCode).HasMaxLength(64);
            builder.Property(e => e.AgRestriction_ProgClassFlag).HasMaxLength(16);
            builder.Property(e => e.AgRestriction_LiveBroadcastFlag).HasMaxLength(16);

            builder.Property(e => e.AgSpot_BreakDate).HasMaxLength(32);
            builder.Property(e => e.AgSpot_BreakTime).HasMaxLength(32);
            builder.Property(e => e.AgSpot_Status).HasMaxLength(32);
            builder.Property(e => e.AgSpot_MultipartIndicator).HasMaxLength(16);
            builder.Property(e => e.AgSpot_BonusSpot).HasMaxLength(64);
            builder.Property(e => e.AgSpot_ClientPicked).HasMaxLength(16);
            builder.Property(e => e.AgSpot_ClashCode).HasMaxLength(64);
            builder.Property(e => e.AgSpot_AdvertiserIdentifier).HasMaxLength(256);
            builder.Property(e => e.AgSpot_RootClashCode).HasMaxLength(64);

            builder.Property(e => e.AgBreak_AgProgCategories).AsDelimitedString();

            builder.HasMany(e => e.AgBreak_AgRegionalBreaks).WithOne()
                .HasForeignKey(e => e.AutoBookDefaultParametersId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.AgBreak_AgPredictions).WithOne()
                .HasForeignKey(e => e.AutoBookDefaultParametersId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.AgBreak_AgAvals).WithOne()
                .HasForeignKey(e => e.AutoBookDefaultParametersId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.AgCampaign_AgCampaignSalesAreas).WithOne()
                .HasForeignKey(e => e.AutoBookDefaultParametersId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.AgCampaign_AgProgrammeList).WithOne()
                .HasForeignKey(e => e.AutoBookDefaultParametersId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
