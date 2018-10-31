using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MISTDO.Web.Migrations
{
    public partial class addCentre : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trainings_TrainingCentres_CentreId1",
                table: "Trainings");

            migrationBuilder.DropIndex(
                name: "IX_Trainings_CentreId1",
                table: "Trainings");

            migrationBuilder.DropColumn(
                name: "CentreId1",
                table: "Trainings");

            migrationBuilder.AddColumn<int>(
                name: "CentreId",
                table: "Trainings",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CentreId",
                table: "Trainings");

            migrationBuilder.AddColumn<int>(
                name: "CentreId1",
                table: "Trainings",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Trainings_CentreId1",
                table: "Trainings",
                column: "CentreId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Trainings_TrainingCentres_CentreId1",
                table: "Trainings",
                column: "CentreId1",
                principalTable: "TrainingCentres",
                principalColumn: "CentreId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
