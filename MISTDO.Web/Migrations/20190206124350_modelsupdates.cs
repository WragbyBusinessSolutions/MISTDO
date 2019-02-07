using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MISTDO.Web.Migrations
{
    public partial class modelsupdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Country",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserAddress",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "OGISPUserName",
                table: "AspNetUsers",
                newName: "Otp");

            migrationBuilder.RenameColumn(
                name: "OGISPId",
                table: "AspNetUsers",
                newName: "PermitNumber");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "AspNetUsers",
                newName: "LicenseExpDate");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "AspNetUsers",
                newName: "CentreAddress");

            migrationBuilder.AlterColumn<string>(
                name: "CompanyName",
                table: "AspNetUsers",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CompanyAddress",
                table: "AspNetUsers",
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
                table: "AspNetUsers",
                newName: "OGISPId");

            migrationBuilder.RenameColumn(
                name: "Otp",
                table: "AspNetUsers",
                newName: "OGISPUserName");

            migrationBuilder.RenameColumn(
                name: "LicenseExpDate",
                table: "AspNetUsers",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "CentreAddress",
                table: "AspNetUsers",
                newName: "FirstName");

            migrationBuilder.AlterColumn<string>(
                name: "CompanyName",
                table: "AspNetUsers",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "CompanyAddress",
                table: "AspNetUsers",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserAddress",
                table: "AspNetUsers",
                maxLength: 100,
                nullable: true);
        }
    }
}
