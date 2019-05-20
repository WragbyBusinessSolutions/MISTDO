using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MISTDO.Web.Migrations.TraineeApplicationDb
{
    public partial class supportedittwo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Response",
                table: "TraineeSupports",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResponseTimeStamp",
                table: "TraineeSupports",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Response",
                table: "TraineeSupports");

            migrationBuilder.DropColumn(
                name: "ResponseTimeStamp",
                table: "TraineeSupports");
        }
    }
}
