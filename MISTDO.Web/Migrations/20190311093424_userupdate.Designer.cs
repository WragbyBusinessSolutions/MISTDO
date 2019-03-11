﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using MISTDO.Web.Data;
using System;

namespace MISTDO.Web.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20190311093424_userupdate")]
    partial class userupdate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.3-rtm-10026")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("MISTDO.Web.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("CentreAddress")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("CentreName")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("City");

                    b.Property<string>("CompanyAddress")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("CompanyName")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<DateTime>("DateRegistered");

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<byte[]>("ImageUpload");

                    b.Property<string>("LicenseExpDate")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("Otp")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PermitNumber")
                        .IsRequired();

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<string>("State");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("MISTDO.Web.Models.Calender", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("Cost");

                    b.Property<string>("ModuleId");

                    b.Property<string>("TrainingCentreId");

                    b.Property<DateTime>("TrainingEndDate");

                    b.Property<DateTime>("TrainingEndTime");

                    b.Property<string>("TrainingName");

                    b.Property<DateTime>("TrainingStartDate");

                    b.Property<DateTime>("TrainingStartTime");

                    b.Property<string>("Venue");

                    b.HasKey("Id");

                    b.ToTable("Calenders");
                });

            modelBuilder.Entity("MISTDO.Web.Models.Certificate", b =>
                {
                    b.Property<int>("CertId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CertNumber");

                    b.Property<string>("CertStatus");

                    b.Property<DateTime>("DateGenerated");

                    b.Property<int>("ModuleId");

                    b.Property<string>("Owner");

                    b.Property<string>("TrainerId");

                    b.Property<string>("TrainerOrg");

                    b.Property<string>("TrainerOrgId");

                    b.Property<int>("TrainingId");

                    b.HasKey("CertId");

                    b.HasIndex("TrainerId");

                    b.ToTable("Certificates");
                });

            modelBuilder.Entity("MISTDO.Web.Models.Notification", b =>
                {
                    b.Property<int>("NotificationId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("NotificationDateTime");

                    b.Property<string>("NotificationMessage");

                    b.HasKey("NotificationId");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("MISTDO.Web.Models.OgispTemp", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CompanyAddress");

                    b.Property<string>("CompanyName");

                    b.Property<DateTime>("DateCreated");

                    b.Property<string>("Email");

                    b.Property<string>("LicenseExpDate");

                    b.Property<string>("Otp");

                    b.Property<string>("PermitNumber");

                    b.Property<DateTime>("Time");

                    b.HasKey("Id");

                    b.ToTable("OgispTemps");
                });

            modelBuilder.Entity("MISTDO.Web.Models.Support", b =>
                {
                    b.Property<int>("SupportId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Issue");

                    b.Property<string>("OwnerId");

                    b.Property<string>("Response");

                    b.Property<DateTime>("ResponseTimeStamp");

                    b.Property<string>("Subject");

                    b.Property<string>("SubjectId");

                    b.Property<DateTime>("SupportTimeStamp");

                    b.HasKey("SupportId");

                    b.ToTable("TrainerSupports");
                });

            modelBuilder.Entity("MISTDO.Web.Models.Training", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CertGenDate");

                    b.Property<string>("CertificateId");

                    b.Property<DateTime>("DateCreated");

                    b.Property<string>("ModuleId");

                    b.Property<string>("PaymentRefId");

                    b.Property<string>("TraineeId");

                    b.Property<string>("TrainingCentreId");

                    b.Property<DateTime>("TrainingEndDate");

                    b.Property<string>("TrainingName");

                    b.Property<DateTime>("TrainingStartDate");

                    b.HasKey("Id");

                    b.ToTable("Trainings");
                });

            modelBuilder.Entity("MISTDO.Web.Models.TrainingCentre", b =>
                {
                    b.Property<int>("CentreId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CentreName")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("OGISPId")
                        .IsRequired();

                    b.Property<string>("OGISPUserName")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("CentreId");

                    b.HasIndex("UserId");

                    b.ToTable("TrainingCentres");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("MISTDO.Web.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("MISTDO.Web.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MISTDO.Web.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("MISTDO.Web.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MISTDO.Web.Models.Certificate", b =>
                {
                    b.HasOne("MISTDO.Web.Models.ApplicationUser", "Trainer")
                        .WithMany()
                        .HasForeignKey("TrainerId");
                });

            modelBuilder.Entity("MISTDO.Web.Models.TrainingCentre", b =>
                {
                    b.HasOne("MISTDO.Web.Models.ApplicationUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
