﻿// <auto-generated />
using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.MasterMigrations
{
    [DbContext(typeof(MasterMigrationDbContext))]
    [Migration("20200914055805_XGGT-14867-Add_Paginated_Campaign_View_Product_Feature")]
    partial class XGGT14867Add_Paginated_Campaign_View_Product_Feature
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master.AccessToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasMaxLength(128);

                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(0);

                    b.Property<DateTime>("ValidUntilValue");

                    b.HasKey("Id");

                    b.HasIndex("Token");

                    b.ToTable("AccessTokens");
                });

            modelBuilder.Entity("ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master.PreviewFile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<byte[]>("Content");

                    b.Property<long?>("ContentLength")
                        .IsRequired();

                    b.Property<string>("ContentType")
                        .IsRequired()
                        .HasMaxLength(64);

                    b.Property<string>("FileExtension")
                        .HasMaxLength(64);

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasMaxLength(128);

                    b.Property<string>("Location")
                        .HasMaxLength(256);

                    b.Property<int?>("TenantId");

                    b.Property<int?>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("TenantId")
                        .IsUnique()
                        .HasFilter("[TenantId] IS NOT NULL");

                    b.HasIndex("UserId")
                        .IsUnique()
                        .HasFilter("[UserId] IS NOT NULL");

                    b.ToTable("Previews");
                });

            modelBuilder.Entity("ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master.ProductFeature", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Enabled")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(false);

                    b.Property<string>("Name")
                        .HasMaxLength(128);

                    b.Property<int>("ProductSettingsId");

                    b.Property<string>("Settings")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(2048)
                        .HasDefaultValue("{}");

                    b.HasKey("Id");

                    b.HasIndex("ProductSettingsId");

                    b.ToTable("ProductSettingFeatures");
                });

            modelBuilder.Entity("ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master.ProductSettings", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .HasMaxLength(1024);

                    b.HasKey("Id");

                    b.ToTable("ProductSettings");
                });

            modelBuilder.Entity("ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master.TaskInstance", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("newid()");

                    b.Property<int>("Status");

                    b.Property<string>("TaskId")
                        .IsRequired()
                        .HasMaxLength(128);

                    b.Property<int>("TenantId")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(0);

                    b.Property<DateTime>("TimeCompleted");

                    b.Property<DateTime>("TimeCreated");

                    b.Property<DateTime>("TimeLastActive");

                    b.HasKey("Id");

                    b.HasIndex("TaskId");

                    b.ToTable("TaskInstances");
                });

            modelBuilder.Entity("ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master.TaskInstanceParameter", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128);

                    b.Property<Guid>("TaskInstanceId")
                        .HasMaxLength(128);

                    b.Property<string>("Value")
                        .HasMaxLength(2048);

                    b.HasKey("Id");

                    b.HasIndex("TaskInstanceId");

                    b.ToTable("TaskInstanceParameters");
                });

            modelBuilder.Entity("ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master.Tenant", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("DefaultTheme")
                        .HasMaxLength(128);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128);

                    b.HasKey("Id");

                    b.ToTable("Tenants");
                });

            modelBuilder.Entity("ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master.TenantProductFeatures.TenantProductFeature", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Enabled")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(false);

                    b.Property<bool>("IsShared")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(false);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64);

                    b.Property<int>("TenantId");

                    b.HasKey("Id");

                    b.HasIndex("Name", "TenantId")
                        .IsUnique();

                    b.ToTable("TenantProductFeatures");
                });

            modelBuilder.Entity("ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master.TenantProductFeatures.TenantProductFeatureReference", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("TenantProductFeatureChildId");

                    b.Property<int>("TenantProductFeatureParentId");

                    b.HasKey("Id");

                    b.HasIndex("TenantProductFeatureChildId", "TenantProductFeatureParentId")
                        .IsUnique();

                    b.ToTable("TenantProductFeatureReferences");
                });

            modelBuilder.Entity("ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master.UpdateDetails", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("newid()");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128);

                    b.Property<int>("TenantId")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(0);

                    b.Property<DateTime>("TimeApplied");

                    b.HasKey("Id");

                    b.ToTable("UpdateDetails");
                });

            modelBuilder.Entity("ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("DefaultTimeZone")
                        .IsRequired()
                        .HasMaxLength(128);

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(128);

                    b.Property<string>("Location")
                        .HasMaxLength(256);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128);

                    b.Property<string>("Password")
                        .HasMaxLength(256);

                    b.Property<string>("Role")
                        .HasMaxLength(128);

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasMaxLength(128);

                    b.Property<int>("TenantId");

                    b.Property<string>("ThemeName")
                        .IsRequired()
                        .HasMaxLength(128);

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master.UserSetting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128);

                    b.Property<int>("UserId");

                    b.Property<string>("Value")
                        .HasColumnType("NVARCHAR(MAX)");

                    b.HasKey("Id");

                    b.HasIndex("Name");

                    b.HasIndex("UserId");

                    b.ToTable("UserSettings");
                });

            modelBuilder.Entity("ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master.PreviewFile", b =>
                {
                    b.HasOne("ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master.Tenant")
                        .WithOne("Preview")
                        .HasForeignKey("ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master.PreviewFile", "TenantId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master.User")
                        .WithOne("Preview")
                        .HasForeignKey("ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master.PreviewFile", "UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master.ProductFeature", b =>
                {
                    b.HasOne("ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master.ProductSettings")
                        .WithMany("Features")
                        .HasForeignKey("ProductSettingsId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master.TaskInstanceParameter", b =>
                {
                    b.HasOne("ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master.TaskInstance")
                        .WithMany("Parameters")
                        .HasForeignKey("TaskInstanceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master.Tenant", b =>
                {
                    b.OwnsOne("ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master.DatabaseProviderConfiguration", "TenantDb", b1 =>
                        {
                            b1.Property<int>("TenantId")
                                .ValueGeneratedOnAdd()
                                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                            b1.Property<string>("ConnectionString")
                                .IsRequired()
                                .HasMaxLength(256);

                            b1.Property<int>("Provider");

                            b1.HasKey("TenantId");

                            b1.ToTable("Tenants");

                            b1.HasOne("ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master.Tenant")
                                .WithOne("TenantDb")
                                .HasForeignKey("ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master.DatabaseProviderConfiguration", "TenantId")
                                .OnDelete(DeleteBehavior.Cascade);
                        });
                });

            modelBuilder.Entity("ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master.TenantProductFeatures.TenantProductFeatureReference", b =>
                {
                    b.HasOne("ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master.TenantProductFeatures.TenantProductFeature")
                        .WithMany("ParentFeatures")
                        .HasForeignKey("TenantProductFeatureChildId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master.UserSetting", b =>
                {
                    b.HasOne("ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master.User")
                        .WithMany("UserSettings")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
