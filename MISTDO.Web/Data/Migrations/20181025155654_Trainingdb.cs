using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MISTDO.Web.Data.Migrations
{
    public partial class Trainingdb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "ImageUpload",
                table: "Trainees",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Trainings",
                columns: table => new
                {
                    TrainingId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CentreId1 = table.Column<int>(nullable: true),
                    TrainingCost = table.Column<int>(nullable: false),
                    TrainingEndDate = table.Column<DateTime>(nullable: false),
                    TrainingName = table.Column<string>(nullable: true),
                    TrainingStartDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trainings", x => x.TrainingId);
                    table.ForeignKey(
                        name: "FK_Trainings_TrainingCentres_CentreId1",
                        column: x => x.CentreId1,
                        principalTable: "TrainingCentres",
                        principalColumn: "CentreId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Trainings_CentreId1",
                table: "Trainings",
                column: "CentreId1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Trainings");

            migrationBuilder.DropColumn(
                name: "ImageUpload",
                table: "Trainees");
        }
    }
}
