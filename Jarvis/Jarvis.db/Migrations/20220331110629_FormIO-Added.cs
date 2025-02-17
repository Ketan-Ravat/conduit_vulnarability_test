using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class FormIOAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FormIO",
                columns: table => new
                {
                    form_id = table.Column<Guid>(nullable: false),
                    form_name = table.Column<string>(nullable: true),
                    form_type = table.Column<string>(nullable: true),
                    form_data = table.Column<string>(nullable: true),
                    company_id = table.Column<Guid>(nullable: false),
                    site_id = table.Column<Guid>(nullable: false),
                    asset_id = table.Column<Guid>(nullable: true),
                    status = table.Column<int>(nullable: true),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true)
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FormIO");
        }
    }
}
