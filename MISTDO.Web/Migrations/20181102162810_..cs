using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MISTDO.Web.Migrations
{
    public partial class _ : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "TraineeApplicationUser");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "TraineeApplicationUser");

            migrationBuilder.DropColumn(
                name: "State",
                table: "TraineeApplicationUser");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "TraineeApplicationUser",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "TraineeApplicationUser",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "TraineeApplicationUser",
                nullable: true);
        }
    }
}
