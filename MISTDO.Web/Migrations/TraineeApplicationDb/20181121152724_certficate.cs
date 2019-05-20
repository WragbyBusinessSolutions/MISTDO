using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MISTDO.Web.Migrations.TraineeApplicationDb
{
    public partial class certficate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Certificates_AspNetUsers_OwnerId",
                table: "Certificates");

            migrationBuilder.DropIndex(
                name: "IX_Certificates_OwnerId",
                table: "Certificates");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "Certificates",
                newName: "Owner");

            migrationBuilder.AlterColumn<string>(
                name: "Owner",
                table: "Certificates",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Owner",
                table: "Certificates",
                newName: "OwnerId");

            migrationBuilder.AlterColumn<string>(
                name: "OwnerId",
                table: "Certificates",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_OwnerId",
                table: "Certificates",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Certificates_AspNetUsers_OwnerId",
                table: "Certificates",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
