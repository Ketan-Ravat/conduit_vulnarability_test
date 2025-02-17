using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class formiomodified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FormIO");

            migrationBuilder.AddColumn<int>(
                name: "condition_index",
                table: "Assets",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "form_id",
                table: "Assets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "levels",
                table: "Assets",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "InspectionsTemplateFormIO",
                columns: table => new
                {
                    form_id = table.Column<Guid>(nullable: false),
                    form_name = table.Column<string>(nullable: true),
                    form_type = table.Column<string>(nullable: true),
                    form_data = table.Column<string>(nullable: true),
                    company_id = table.Column<Guid>(nullable: false),
                    form_description = table.Column<string>(nullable: true),
                    status = table.Column<int>(nullable: true),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionsTemplateFormIO", x => x.form_id);
                    table.ForeignKey(
                        name: "FK_InspectionsTemplateFormIO_StatusMasters_status",
                        column: x => x.status,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assets_condition_index",
                table: "Assets",
                column: "condition_index");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_form_id",
                table: "Assets",
                column: "form_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InspectionsTemplateFormIO_status",
                table: "InspectionsTemplateFormIO",
                column: "status");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_StatusMasters_condition_index",
                table: "Assets",
                column: "condition_index",
                principalTable: "StatusMasters",
                principalColumn: "status_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_InspectionsTemplateFormIO_form_id",
                table: "Assets",
                column: "form_id",
                principalTable: "InspectionsTemplateFormIO",
                principalColumn: "form_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_StatusMasters_condition_index",
                table: "Assets");

            migrationBuilder.DropForeignKey(
                name: "FK_Assets_InspectionsTemplateFormIO_form_id",
                table: "Assets");

            migrationBuilder.DropTable(
                name: "InspectionsTemplateFormIO");

            migrationBuilder.DropIndex(
                name: "IX_Assets_condition_index",
                table: "Assets");

            migrationBuilder.DropIndex(
                name: "IX_Assets_form_id",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "condition_index",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "form_id",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "levels",
                table: "Assets");

            migrationBuilder.CreateTable(
                name: "FormIO",
                columns: table => new
                {
                    form_id = table.Column<Guid>(type: "uuid", nullable: false),
                    asset_id = table.Column<Guid>(type: "uuid", nullable: true),
                    company_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    form_data = table.Column<string>(type: "text", nullable: true),
                    form_name = table.Column<string>(type: "text", nullable: true),
                    form_type = table.Column<string>(type: "text", nullable: true),
                    modified_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    modified_by = table.Column<string>(type: "text", nullable: true),
                    site_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormIO", x => x.form_id);
                    table.ForeignKey(
                        name: "FK_FormIO_Assets_asset_id",
                        column: x => x.asset_id,
                        principalTable: "Assets",
                        principalColumn: "asset_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FormIO_Sites_site_id",
                        column: x => x.site_id,
                        principalTable: "Sites",
                        principalColumn: "site_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FormIO_StatusMasters_status",
                        column: x => x.status,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FormIO_asset_id",
                table: "FormIO",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "IX_FormIO_site_id",
                table: "FormIO",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "IX_FormIO_status",
                table: "FormIO",
                column: "status");
        }
    }
}
