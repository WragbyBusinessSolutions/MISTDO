using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MISTDO.Web.Migrations.TraineeApplicationDb
{
    public partial class Trainingupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trainings_TrainingCentre_CentreId1",
                table: "Trainings");

            migrationBuilder.DropTable(
                name: "TrainingCentre");

            migrationBuilder.DropTable(
                name: "ApplicationUser");

            migrationBuilder.DropIndex(
                name: "IX_Trainings_CentreId1",
                table: "Trainings");

            migrationBuilder.DropColumn(
                name: "CentreId1",
                table: "Trainings");

            migrationBuilder.DropColumn(
                name: "ImageUpload",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "CentreId",
                table: "Trainings",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CertificateId",
                table: "Trainings",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TraineeId",
                table: "Trainings",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CentreId",
                table: "Trainings");

            migrationBuilder.DropColumn(
                name: "CertificateId",
                table: "Trainings");

            migrationBuilder.DropColumn(
                name: "TraineeId",
                table: "Trainings");

            migrationBuilder.AddColumn<int>(
                name: "CentreId1",
                table: "Trainings",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageUpload",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ApplicationUser",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    City = table.Column<string>(nullable: true),
                    CompanyAddress = table.Column<string>(maxLength: 100, nullable: true),
                    CompanyName = table.Column<string>(maxLength: 100, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    DateRegistered = table.Column<DateTime>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    FirstName = table.Column<string>(maxLength: 100, nullable: false),
                    LastName = table.Column<string>(maxLength: 100, nullable: false),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    NormalizedEmail = table.Column<string>(nullable: true),
                    NormalizedUserName = table.Column<string>(nullable: true),
                    PasswordHash = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    SecurityStamp = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    UserAddress = table.Column<string>(maxLength: 100, nullable: true),
                    UserName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUser", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrainingCentre",
                columns: table => new
                {
                    CentreId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CentreName = table.Column<string>(maxLength: 100, nullable: false),
                    OGISPId = table.Column<string>(nullable: false),
                    OGISPUserName = table.Column<string>(maxLength: 100, nullable: false),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingCentre", x => x.CentreId);
                    table.ForeignKey(
                        name: "FK_TrainingCentre_ApplicationUser_UserId",
                        column: x => x.UserId,
                        principalTable: "ApplicationUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Trainings_CentreId1",
                table: "Trainings",
                column: "CentreId1");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingCentre_UserId",
                table: "TrainingCentre",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Trainings_TrainingCentre_CentreId1",
                table: "Trainings",
                column: "CentreId1",
                principalTable: "TrainingCentre",
                principalColumn: "CentreId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
