using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Jarvis.db.Migrations
{
    public partial class AddMaintenanceRequestsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CancelReasonMaster",
                columns: table => new
                {
                    reason_id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(nullable: true),
                    is_archive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CancelReasonMaster", x => x.reason_id);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceRequests",
                columns: table => new
                {
                    mr_id = table.Column<Guid>(nullable: false),
                    title = table.Column<string>(nullable: true),
                    description = table.Column<string>(nullable: true),
                    asset_id = table.Column<Guid>(nullable: false),
                    priority = table.Column<int>(nullable: false),
                    mr_type = table.Column<int>(nullable: false),
                    mr_type_id = table.Column<Guid>(nullable: true),
                    requested_by = table.Column<Guid>(nullable: false),
                    status = table.Column<int>(nullable: false),
                    site_id = table.Column<Guid>(nullable: false),
                    is_archive = table.Column<bool>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceRequests", x => x.mr_id);
                    table.ForeignKey(
                        name: "FK_MaintenanceRequests_Assets_asset_id",
                        column: x => x.asset_id,
                        principalTable: "Assets",
                        principalColumn: "asset_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaintenanceRequests_StatusMasters_mr_type",
                        column: x => x.mr_type,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaintenanceRequests_StatusMasters_priority",
                        column: x => x.priority,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaintenanceRequests_Sites_site_id",
                        column: x => x.site_id,
                        principalTable: "Sites",
                        principalColumn: "site_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaintenanceRequests_StatusMasters_status",
                        column: x => x.status,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceReqCancelRequests",
                columns: table => new
                {
                    mr_cancel_id = table.Column<Guid>(nullable: false),
                    reason_id = table.Column<int>(nullable: false),
                    mr_id = table.Column<Guid>(nullable: false),
                    notes = table.Column<string>(nullable: true),
                    is_archive = table.Column<bool>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceReqCancelRequests", x => x.mr_cancel_id);
                    table.ForeignKey(
                        name: "FK_MaintenanceReqCancelRequests_MaintenanceRequests_mr_id",
                        column: x => x.mr_id,
                        principalTable: "MaintenanceRequests",
                        principalColumn: "mr_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaintenanceReqCancelRequests_CancelReasonMaster_reason_id",
                        column: x => x.reason_id,
                        principalTable: "CancelReasonMaster",
                        principalColumn: "reason_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceReqCancelRequests_mr_id",
                table: "MaintenanceReqCancelRequests",
                column: "mr_id");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceReqCancelRequests_reason_id",
                table: "MaintenanceReqCancelRequests",
                column: "reason_id");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceRequests_asset_id",
                table: "MaintenanceRequests",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceRequests_mr_type",
                table: "MaintenanceRequests",
                column: "mr_type");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceRequests_priority",
                table: "MaintenanceRequests",
                column: "priority");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceRequests_site_id",
                table: "MaintenanceRequests",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceRequests_status",
                table: "MaintenanceRequests",
                column: "status");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MaintenanceReqCancelRequests");

            migrationBuilder.DropTable(
                name: "MaintenanceRequests");

            migrationBuilder.DropTable(
                name: "CancelReasonMaster");
        }
    }
}
