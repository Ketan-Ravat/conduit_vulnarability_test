using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class TrackMobileSyncOfflineadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "initial_inspected_at",
                table: "WOOnboardingAssets",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "initial_inspected_by",
                table: "WOOnboardingAssets",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TrackMobileSyncOffline",
                columns: table => new
                {
                    trackmobilesyncoffline_id = table.Column<Guid>(nullable: false),
                    device_uuid = table.Column<Guid>(nullable: false),
                    device_code = table.Column<string>(nullable: true),
                    user_id = table.Column<Guid>(nullable: false),
                    sync_time = table.Column<DateTime>(nullable: false),
                    status = table.Column<int>(nullable: false),
                    lambda_logs = table.Column<string>(nullable: true),
                    s3_file_name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackMobileSyncOffline", x => x.trackmobilesyncoffline_id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrackMobileSyncOffline");

            migrationBuilder.DropColumn(
                name: "initial_inspected_at",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "initial_inspected_by",
                table: "WOOnboardingAssets");
        }
    }
}
