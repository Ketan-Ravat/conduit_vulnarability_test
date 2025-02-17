using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class new_table_SiteProjectManagerMapping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SiteProjectManagerMapping",
                columns: table => new
                {
                    site_projectmanager_mapping_id = table.Column<Guid>(nullable: false),
                    site_id = table.Column<Guid>(nullable: false),
                    user_id = table.Column<Guid>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_by = table.Column<string>(nullable: true),
                    is_deleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteProjectManagerMapping", x => x.site_projectmanager_mapping_id);
                    table.ForeignKey(
                        name: "FK_SiteProjectManagerMapping_Sites_site_id",
                        column: x => x.site_id,
                        principalTable: "Sites",
                        principalColumn: "site_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SiteProjectManagerMapping_User_user_id",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "uuid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SiteProjectManagerMapping_site_id",
                table: "SiteProjectManagerMapping",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "IX_SiteProjectManagerMapping_user_id",
                table: "SiteProjectManagerMapping",
                column: "user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SiteProjectManagerMapping");
        }
    }
}
