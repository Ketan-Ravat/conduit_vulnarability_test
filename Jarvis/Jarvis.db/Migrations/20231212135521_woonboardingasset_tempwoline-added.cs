using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class woonboardingasset_tempwolineadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "temp_woonboardingassets_id",
                table: "WOOnboardingAssets",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "temp_woonboardingassets_id",
                table: "WOOnboardingAssets");
        }
    }
}
