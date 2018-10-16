using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MISTDO.Web.Data.Migrations
{
    public partial class TrainerdbUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trainees_Certificates_CertId",
                table: "Trainees");

            migrationBuilder.DropIndex(
                name: "IX_Trainees_CertId",
                table: "Trainees");

            migrationBuilder.DropColumn(
                name: "CertId",
                table: "Trainees");

            migrationBuilder.AddColumn<int>(
                name: "OwnerTraineeId",
                table: "Certificates",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_OwnerTraineeId",
                table: "Certificates",
                column: "OwnerTraineeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Certificates_Trainees_OwnerTraineeId",
                table: "Certificates",
                column: "OwnerTraineeId",
                principalTable: "Trainees",
                principalColumn: "TraineeId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Certificates_Trainees_OwnerTraineeId",
                table: "Certificates");

            migrationBuilder.DropIndex(
                name: "IX_Certificates_OwnerTraineeId",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "OwnerTraineeId",
                table: "Certificates");

            migrationBuilder.AddColumn<int>(
                name: "CertId",
                table: "Trainees",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Trainees_CertId",
                table: "Trainees",
                column: "CertId");

            migrationBuilder.AddForeignKey(
                name: "FK_Trainees_Certificates_CertId",
                table: "Trainees",
                column: "CertId",
                principalTable: "Certificates",
                principalColumn: "CertId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
