using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class OBWO_tables_added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WOOnboardingAssets",
                columns: table => new
                {
                    woonboardingassets_id = table.Column<Guid>(nullable: false),
                    asset_name = table.Column<string>(nullable: true),
                    asset_class_code = table.Column<string>(nullable: true),
                    asset_class_name = table.Column<string>(nullable: true),
                    back_office_note = table.Column<string>(nullable: true),
                    building = table.Column<string>(nullable: true),
                    floor = table.Column<string>(nullable: true),
                    room = table.Column<string>(nullable: true),
                    section = table.Column<string>(nullable: true),
                    QR_code = table.Column<string>(nullable: true),
                    field_note = table.Column<string>(nullable: true),
                    wo_id = table.Column<Guid>(nullable: false),
                    status = table.Column<int>(nullable: false),
                    site_id = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WOOnboardingAssets", x => x.woonboardingassets_id);
                    table.ForeignKey(
                        name: "FK_WOOnboardingAssets_Sites_site_id",
                        column: x => x.site_id,
                        principalTable: "Sites",
                        principalColumn: "site_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WOOnboardingAssets_StatusMasters_status",
                        column: x => x.status,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WOOnboardingAssets_WorkOrders_wo_id",
                        column: x => x.wo_id,
                        principalTable: "WorkOrders",
                        principalColumn: "wo_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WOOnboardingAssetsImagesMapping",
                columns: table => new
                {
                    woonboardingassetsimagesmapping_id = table.Column<Guid>(nullable: false),
                    asset_photo = table.Column<string>(nullable: true),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true),
                    is_deleted = table.Column<bool>(nullable: false),
                    woonboardingassets_id = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WOOnboardingAssetsImagesMapping", x => x.woonboardingassetsimagesmapping_id);
                    table.ForeignKey(
                        name: "FK_WOOnboardingAssetsImagesMapping_WOOnboardingAssets_woonboar~",
                        column: x => x.woonboardingassets_id,
                        principalTable: "WOOnboardingAssets",
                        principalColumn: "woonboardingassets_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WOOnboardingAssets_site_id",
                table: "WOOnboardingAssets",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "IX_WOOnboardingAssets_status",
                table: "WOOnboardingAssets",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_WOOnboardingAssets_wo_id",
                table: "WOOnboardingAssets",
                column: "wo_id");

            migrationBuilder.CreateIndex(
                name: "IX_WOOnboardingAssetsImagesMapping_woonboardingassets_id",
                table: "WOOnboardingAssetsImagesMapping",
                column: "woonboardingassets_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WOOnboardingAssetsImagesMapping");

            migrationBuilder.DropTable(
                name: "WOOnboardingAssets");
        }
    }
}
