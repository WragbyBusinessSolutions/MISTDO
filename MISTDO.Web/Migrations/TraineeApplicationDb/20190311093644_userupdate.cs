using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MISTDO.Web.Migrations.TraineeApplicationDb
{
    public partial class userupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MiddleFinger",
                table: "AspNetUsers",
                newName: "RightThumb");

            migrationBuilder.RenameColumn(
                name: "LastFinger",
                table: "AspNetUsers",
                newName: "RightIndex");

            migrationBuilder.RenameColumn(
                name: "FirstFinger",
                table: "AspNetUsers",
                newName: "LeftThumb");

            migrationBuilder.AddColumn<byte[]>(
                name: "LeftIndex",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UID",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageUpload",
                table: "ApplicationUser",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LeftIndex",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UID",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ImageUpload",
                table: "ApplicationUser");

            migrationBuilder.RenameColumn(
                name: "RightThumb",
                table: "AspNetUsers",
                newName: "MiddleFinger");

            migrationBuilder.RenameColumn(
                name: "RightIndex",
                table: "AspNetUsers",
                newName: "LastFinger");

            migrationBuilder.RenameColumn(
                name: "LeftThumb",
                table: "AspNetUsers",
                newName: "FirstFinger");
        }
    }
}
