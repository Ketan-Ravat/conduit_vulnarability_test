using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class AddAssetFormIOTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssetFormIO",
                columns: table => new
                {
                    asset_form_id = table.Column<Guid>(nullable: false),
                    asset_id = table.Column<Guid>(nullable: false),
                    form_id = table.Column<Guid>(nullable: true),
                    asset_form_name = table.Column<string>(nullable: true),
                    asset_form_type = table.Column<string>(nullable: true),
                    asset_form_description = table.Column<string>(nullable: true),
                    asset_form_data = table.Column<string>(nullable: true),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true),
                    status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetFormIO", x => x.asset_form_id);
                    table.ForeignKey(
                        name: "FK_AssetFormIO_Assets_asset_id",
                        column: x => x.asset_id,
                        principalTable: "Assets",
                        principalColumn: "asset_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetFormIO_StatusMasters_status",
                        column: x => x.status,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetFormIO_asset_id",
                table: "AssetFormIO",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "IX_AssetFormIO_status",
                table: "AssetFormIO",
                column: "status");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetFormIO");
        }
    }
}
