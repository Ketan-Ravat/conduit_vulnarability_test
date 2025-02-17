using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Jarvis.db.Migrations
{
    public partial class assetinspectionreport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssetInspectionReport",
                columns: table => new
                {
                    report_id = table.Column<Guid>(nullable: false),
                    report_number = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    asset_id = table.Column<Guid>(nullable: false),
                    from_date = table.Column<DateTime>(nullable: false),
                    to_date = table.Column<DateTime>(nullable: false),
                    status = table.Column<int>(nullable: false),
                    download_link = table.Column<string>(nullable: true),
                    created_at = table.Column<DateTime>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_by = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetInspectionReport", x => x.report_id);
                    table.ForeignKey(
                        name: "FK_AssetInspectionReport_Assets_asset_id",
                        column: x => x.asset_id,
                        principalTable: "Assets",
                        principalColumn: "asset_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetInspectionReport_StatusMasters_status",
                        column: x => x.status,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetInspectionReport_asset_id",
                table: "AssetInspectionReport",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "IX_AssetInspectionReport_status",
                table: "AssetInspectionReport",
                column: "status");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetInspectionReport");
        }
    }
}
