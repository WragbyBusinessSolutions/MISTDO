using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MISTDO.Web.Data.Migrations
{
    public partial class Training : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CentreId1",
                table: "Training",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Training_CentreId1",
                table: "Training",
                column: "CentreId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Training_TrainingCentres_CentreId1",
                table: "Training",
                column: "CentreId1",
                principalTable: "TrainingCentres",
                principalColumn: "CentreId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Training_TrainingCentres_CentreId1",
                table: "Training");

            migrationBuilder.DropIndex(
                name: "IX_Training_CentreId1",
                table: "Training");

            migrationBuilder.DropColumn(
                name: "CentreId1",
                table: "Training");
        }
    }
}
