using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MISTDO.Web.Migrations.TraineeApplicationDb
{
    public partial class suppfd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SubjectId",
                table: "TraineeSupports",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "OwnerId",
                table: "TraineeSupports",
                nullable: true,
                oldClrType: typeof(string));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SubjectId",
                table: "TraineeSupports",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OwnerId",
                table: "TraineeSupports",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
