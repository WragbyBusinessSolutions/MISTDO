using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MISTDO.Web.Data.Migrations
{
    public partial class CertdbUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CertNumber",
                table: "Certificates",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateGenerated",
                table: "Certificates",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CertNumber",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "DateGenerated",
                table: "Certificates");
        }
    }
}
