using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Jarvis.db.Migrations
{
    public partial class addsyncinfotable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RecordSyncInformation",
                columns: table => new
                {
                    sync_id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    request_model = table.Column<string>(nullable: true),
                    request_id = table.Column<string>(nullable: true),
                    device_info_id = table.Column<long>(nullable: false),
                    device_uuid = table.Column<Guid>(nullable: false),
                    device_battery_percentage = table.Column<string>(nullable: true),
                    mac_address = table.Column<string>(nullable: true),
                    device_latitude = table.Column<string>(nullable: true),
                    device_longitude = table.Column<string>(nullable: true),
                    is_inspection = table.Column<bool>(nullable: false),
                    is_workorder = table.Column<bool>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecordSyncInformation", x => x.sync_id);
                    table.ForeignKey(
                        name: "FK_RecordSyncInformation_DeviceInfo_device_info_id",
                        column: x => x.device_info_id,
                        principalTable: "DeviceInfo",
                        principalColumn: "device_info_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecordSyncInformation_device_info_id",
                table: "RecordSyncInformation",
                column: "device_info_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecordSyncInformation");
        }
    }
}
