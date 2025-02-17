using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Jarvis.db.Migrations
{
    public partial class WOlinehierarchy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WOLineBuildingMapping",
                columns: table => new
                {
                    wolinebuildings_id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    created_at = table.Column<DateTime>(nullable: true),
                    formiobuilding_id = table.Column<int>(nullable: true),
                    formiofloor_id = table.Column<int>(nullable: true),
                    formioroom_id = table.Column<int>(nullable: true),
                    formiosection_id = table.Column<int>(nullable: true),
                    woonboardingassets_id = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WOLineBuildingMapping", x => x.wolinebuildings_id);
                    table.ForeignKey(
                        name: "FK_WOLineBuildingMapping_FormIOBuildings_formiobuilding_id",
                        column: x => x.formiobuilding_id,
                        principalTable: "FormIOBuildings",
                        principalColumn: "formiobuilding_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WOLineBuildingMapping_FormIOFloors_formiofloor_id",
                        column: x => x.formiofloor_id,
                        principalTable: "FormIOFloors",
                        principalColumn: "formiofloor_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WOLineBuildingMapping_FormIORooms_formioroom_id",
                        column: x => x.formioroom_id,
                        principalTable: "FormIORooms",
                        principalColumn: "formioroom_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WOLineBuildingMapping_FormIOSections_formiosection_id",
                        column: x => x.formiosection_id,
                        principalTable: "FormIOSections",
                        principalColumn: "formiosection_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WOLineBuildingMapping_WOOnboardingAssets_woonboardingassets~",
                        column: x => x.woonboardingassets_id,
                        principalTable: "WOOnboardingAssets",
                        principalColumn: "woonboardingassets_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WOLineBuildingMapping_formiobuilding_id",
                table: "WOLineBuildingMapping",
                column: "formiobuilding_id");

            migrationBuilder.CreateIndex(
                name: "IX_WOLineBuildingMapping_formiofloor_id",
                table: "WOLineBuildingMapping",
                column: "formiofloor_id");

            migrationBuilder.CreateIndex(
                name: "IX_WOLineBuildingMapping_formioroom_id",
                table: "WOLineBuildingMapping",
                column: "formioroom_id");

            migrationBuilder.CreateIndex(
                name: "IX_WOLineBuildingMapping_formiosection_id",
                table: "WOLineBuildingMapping",
                column: "formiosection_id");

            migrationBuilder.CreateIndex(
                name: "IX_WOLineBuildingMapping_woonboardingassets_id",
                table: "WOLineBuildingMapping",
                column: "woonboardingassets_id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WOLineBuildingMapping");
        }
    }
}
