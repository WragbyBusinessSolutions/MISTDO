using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MISTDO.Web.Migrations
{
    public partial class addcalender : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Calenders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Cost = table.Column<decimal>(nullable: false),
                    ModuleId = table.Column<string>(nullable: true),
                    TraineeId = table.Column<string>(nullable: true),
                    TrainingCentreId = table.Column<string>(nullable: true),
                    TrainingEndDate = table.Column<DateTime>(nullable: false),
                    TrainingEndTime = table.Column<DateTime>(nullable: false),
                    TrainingName = table.Column<string>(nullable: true),
                    TrainingStartDate = table.Column<DateTime>(nullable: false),
                    TrainingStartTime = table.Column<DateTime>(nullable: false),
                    Venue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calenders", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Calenders");
        }
    }
}
