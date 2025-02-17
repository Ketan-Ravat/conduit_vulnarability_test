using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class WOInspectionsTemplateFormIOAssignment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           /* migrationBuilder.DropIndex(
                name: "IX_AssetFormIO_asset_id",
                table: "AssetFormIO");
           */
            migrationBuilder.CreateTable(
                name: "WOInspectionsTemplateFormIOAssignment",
                columns: table => new
                {
                    wo_inspectionsTemplateFormIOAssignment_id = table.Column<Guid>(nullable: false),
                    wo_id = table.Column<Guid>(nullable: false),
                    form_id = table.Column<Guid>(nullable: false),
                    technician_user_id = table.Column<Guid>(nullable: true),
                    asset_id = table.Column<Guid>(nullable: true),
                    created_at = table.Column<DateTime>(nullable: false),
                    updated_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<Guid>(nullable: true),
                    updated_by = table.Column<Guid>(nullable: true),
                    is_archived = table.Column<bool>(nullable: false),
                    status_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WOInspectionsTemplateFormIOAssignment", x => x.wo_inspectionsTemplateFormIOAssignment_id);
                    table.ForeignKey(
                        name: "FK_WOInspectionsTemplateFormIOAssignment_Assets_asset_id",
                        column: x => x.asset_id,
                        principalTable: "Assets",
                        principalColumn: "asset_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WOInspectionsTemplateFormIOAssignment_InspectionsTemplateFo~",
                        column: x => x.form_id,
                        principalTable: "InspectionsTemplateFormIO",
                        principalColumn: "form_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WOInspectionsTemplateFormIOAssignment_StatusMasters_status_~",
                        column: x => x.status_id,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WOInspectionsTemplateFormIOAssignment_User_technician_user_~",
                        column: x => x.technician_user_id,
                        principalTable: "User",
                        principalColumn: "uuid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WOInspectionsTemplateFormIOAssignment_WorkOrders_wo_id",
                        column: x => x.wo_id,
                        principalTable: "WorkOrders",
                        principalColumn: "wo_id",
                        onDelete: ReferentialAction.Cascade);
                });

          /*  migrationBuilder.CreateIndex(
                name: "IX_AssetFormIO_asset_id",
                table: "AssetFormIO",
                column: "asset_id",
                unique: true);
          */
            migrationBuilder.CreateIndex(
                name: "IX_WOInspectionsTemplateFormIOAssignment_asset_id",
                table: "WOInspectionsTemplateFormIOAssignment",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "IX_WOInspectionsTemplateFormIOAssignment_form_id",
                table: "WOInspectionsTemplateFormIOAssignment",
                column: "form_id");

            migrationBuilder.CreateIndex(
                name: "IX_WOInspectionsTemplateFormIOAssignment_status_id",
                table: "WOInspectionsTemplateFormIOAssignment",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_WOInspectionsTemplateFormIOAssignment_technician_user_id",
                table: "WOInspectionsTemplateFormIOAssignment",
                column: "technician_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_WOInspectionsTemplateFormIOAssignment_wo_id",
                table: "WOInspectionsTemplateFormIOAssignment",
                column: "wo_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WOInspectionsTemplateFormIOAssignment");

          /*  migrationBuilder.DropIndex(
                name: "IX_AssetFormIO_asset_id",
                table: "AssetFormIO");

            migrationBuilder.CreateIndex(
                name: "IX_AssetFormIO_asset_id",
                table: "AssetFormIO",
                column: "asset_id");*/
        }
    }
}
