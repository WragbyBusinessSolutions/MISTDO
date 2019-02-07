using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MISTDO.Web.Migrations
{
    public partial class applictaional : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OwnerId",
                table: "TrainerSupports",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<string>(
                name: "SubjectId",
                table: "TrainerSupports",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "TrainerSupports");

            migrationBuilder.AlterColumn<string>(
                name: "OwnerId",
                table: "TrainerSupports",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
