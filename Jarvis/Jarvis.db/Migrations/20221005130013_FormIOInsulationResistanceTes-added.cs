using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class FormIOInsulationResistanceTesadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FormIOInsulationResistanceTestMapping",
                columns: table => new
                {
                    FormIOInsulationResistanceTestMapping_id = table.Column<Guid>(nullable: false),
                    IRPoletoPoleAsFound1 = table.Column<string>(nullable: true),
                    IRPoletoPoleAsFound2 = table.Column<string>(nullable: true),
                    IRPoletoPoleAsFound3 = table.Column<string>(nullable: true),
                    IRAcrossPoleAsFound1 = table.Column<string>(nullable: true),
                    IRAcrossPoleAsFound2 = table.Column<string>(nullable: true),
                    IRAcrossPoleAsFound3 = table.Column<string>(nullable: true),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    updated_at = table.Column<DateTime>(nullable: true),
                    updated_by = table.Column<string>(nullable: true),
                    is_archived = table.Column<bool>(nullable: false),
                    asset_form_id = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormIOInsulationResistanceTestMapping", x => x.FormIOInsulationResistanceTestMapping_id);
                    table.ForeignKey(
                        name: "FK_FormIOInsulationResistanceTestMapping_AssetFormIO_asset_for~",
                        column: x => x.asset_form_id,
                        principalTable: "AssetFormIO",
                        principalColumn: "asset_form_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FormIOInsulationResistanceTestMapping_asset_form_id",
                table: "FormIOInsulationResistanceTestMapping",
                column: "asset_form_id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FormIOInsulationResistanceTestMapping");
        }
    }
}
