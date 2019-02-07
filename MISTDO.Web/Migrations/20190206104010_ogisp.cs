using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MISTDO.Web.Migrations
{
    public partial class ogisp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OgispTemps",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CompanyAddress = table.Column<string>(nullable: true),
                    CompanyName = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    LicenseExpDate = table.Column<string>(nullable: true),
                    Otp = table.Column<string>(nullable: true),
                    PermitNumber = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OgispTemps", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OgispTemps");
        }
    }
}
