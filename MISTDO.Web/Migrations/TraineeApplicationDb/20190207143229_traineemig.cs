using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MISTDO.Web.Migrations.TraineeApplicationDb
{
    public partial class traineemig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Country",
                table: "ApplicationUser");

            migrationBuilder.DropColumn(
                name: "UserAddress",
                table: "ApplicationUser");

            migrationBuilder.RenameColumn(
                name: "OGISPUserName",
                table: "ApplicationUser",
                newName: "Otp");

            migrationBuilder.RenameColumn(
                name: "OGISPId",
                table: "ApplicationUser",
                newName: "PermitNumber");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "ApplicationUser",
                newName: "LicenseExpDate");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "ApplicationUser",
                newName: "CentreAddress");

            migrationBuilder.AlterColumn<string>(
                name: "CompanyName",
                table: "ApplicationUser",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CompanyAddress",
                table: "ApplicationUser",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 100,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PermitNumber",
                table: "ApplicationUser",
                newName: "OGISPId");

            migrationBuilder.RenameColumn(
                name: "Otp",
                table: "ApplicationUser",
                newName: "OGISPUserName");

            migrationBuilder.RenameColumn(
                name: "LicenseExpDate",
                table: "ApplicationUser",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "CentreAddress",
                table: "ApplicationUser",
                newName: "FirstName");

            migrationBuilder.AlterColumn<string>(
                name: "CompanyName",
                table: "ApplicationUser",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "CompanyAddress",
                table: "ApplicationUser",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "ApplicationUser",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserAddress",
                table: "ApplicationUser",
                maxLength: 100,
                nullable: true);
        }
    }
}
