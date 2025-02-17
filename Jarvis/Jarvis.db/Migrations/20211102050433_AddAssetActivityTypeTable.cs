using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Jarvis.db.Migrations
{
    public partial class AddAssetActivityTypeTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssetActivityLogs",
                columns: table => new
                {
                    activity_id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    activity_header = table.Column<string>(nullable: true),
                    activity_message = table.Column<string>(nullable: true),
                    activity_type = table.Column<int>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: false),
                    updated_by = table.Column<string>(nullable: true),
                    data = table.Column<string>(nullable: true),
                    asset_id = table.Column<Guid>(nullable: false),
                    site_id = table.Column<Guid>(nullable: false),
                    ref_id = table.Column<string>(nullable: true),
                    status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetActivityLogs", x => x.activity_id);
                    table.ForeignKey(
                        name: "FK_AssetActivityLogs_Assets_asset_id",
                        column: x => x.asset_id,
                        principalTable: "Assets",
                        principalColumn: "asset_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetActivityLogs_asset_id",
                table: "AssetActivityLogs",
                column: "asset_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetActivityLogs");
        }
    }
}
