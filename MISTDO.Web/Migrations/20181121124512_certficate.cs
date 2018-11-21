using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MISTDO.Web.Migrations
{
    public partial class certficate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Certificates_TraineeApplicationUser_OwnerId",
                table: "Certificates");

            migrationBuilder.DropTable(
                name: "TraineeApplicationUser");

            migrationBuilder.DropIndex(
                name: "IX_Certificates_OwnerId",
                table: "Certificates");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "Certificates",
                newName: "Owner");

            migrationBuilder.AlterColumn<string>(
                name: "Owner",
                table: "Certificates",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Owner",
                table: "Certificates",
                newName: "OwnerId");

            migrationBuilder.AlterColumn<string>(
                name: "OwnerId",
                table: "Certificates",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "TraineeApplicationUser",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    CompanyAddress = table.Column<string>(maxLength: 100, nullable: true),
                    CompanyName = table.Column<string>(maxLength: 100, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    DateRegistered = table.Column<DateTime>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    FirstFinger = table.Column<byte[]>(nullable: true),
                    FirstName = table.Column<string>(maxLength: 100, nullable: false),
                    ImageUpload = table.Column<byte[]>(nullable: true),
                    LastFinger = table.Column<byte[]>(nullable: true),
                    LastName = table.Column<string>(maxLength: 100, nullable: false),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    MiddleFinger = table.Column<byte[]>(nullable: true),
                    NormalizedEmail = table.Column<string>(nullable: true),
                    NormalizedUserName = table.Column<string>(nullable: true),
                    PasswordHash = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    SecurityStamp = table.Column<string>(nullable: true),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    UserAddress = table.Column<string>(maxLength: 100, nullable: true),
                    UserName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TraineeApplicationUser", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_OwnerId",
                table: "Certificates",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Certificates_TraineeApplicationUser_OwnerId",
                table: "Certificates",
                column: "OwnerId",
                principalTable: "TraineeApplicationUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
