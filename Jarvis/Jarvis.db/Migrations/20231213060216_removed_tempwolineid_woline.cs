using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class removed_tempwolineid_woline : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "temp_woonboardingassets_id",
                table: "WOOnboardingAssets");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "temp_woonboardingassets_id",
                table: "WOOnboardingAssets",
                type: "uuid",
                nullable: true);
        }
    }
}
