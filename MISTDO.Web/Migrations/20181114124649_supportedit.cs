using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MISTDO.Web.Migrations
{
    public partial class supportedit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Response",
                table: "TrainerSupports",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResponseTimeStamp",
                table: "TrainerSupports",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Response",
                table: "TrainerSupports");

            migrationBuilder.DropColumn(
                name: "ResponseTimeStamp",
                table: "TrainerSupports");
        }
    }
}
