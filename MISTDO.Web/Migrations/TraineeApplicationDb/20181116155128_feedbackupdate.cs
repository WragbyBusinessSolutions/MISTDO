using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MISTDO.Web.Migrations.TraineeApplicationDb
{
    public partial class feedbackupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MFeedbackTimeStamp",
                table: "Feedbacks");

            migrationBuilder.AddColumn<DateTime>(
                name: "FeedbackTimeStamp",
                table: "Feedbacks",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FeedbackTimeStamp",
                table: "Feedbacks");

            migrationBuilder.AddColumn<string>(
                name: "MFeedbackTimeStamp",
                table: "Feedbacks",
                nullable: true);
        }
    }
}
