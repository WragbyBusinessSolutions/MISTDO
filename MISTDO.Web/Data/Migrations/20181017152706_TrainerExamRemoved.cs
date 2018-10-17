using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MISTDO.Web.Data.Migrations
{
    public partial class TrainerExamRemoved : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TraineeExaminations");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TraineeExaminations",
                columns: table => new
                {
                    ExamId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateGenerated = table.Column<DateTime>(nullable: false),
                    OwnerTraineeId = table.Column<int>(nullable: true),
                    Remark = table.Column<string>(nullable: true),
                    Score = table.Column<string>(nullable: true),
                    TrainerOrg = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TraineeExaminations", x => x.ExamId);
                    table.ForeignKey(
                        name: "FK_TraineeExaminations_Trainees_OwnerTraineeId",
                        column: x => x.OwnerTraineeId,
                        principalTable: "Trainees",
                        principalColumn: "TraineeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TraineeExaminations_OwnerTraineeId",
                table: "TraineeExaminations",
                column: "OwnerTraineeId");
        }
    }
}
