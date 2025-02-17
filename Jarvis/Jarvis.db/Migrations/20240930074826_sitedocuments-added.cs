using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class sitedocumentsadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SiteDocuments",
                columns: table => new
                {
                    sitedocument_id = table.Column<Guid>(nullable: false),
                    company_id = table.Column<Guid>(nullable: false),
                    site_id = table.Column<Guid>(nullable: true),
                    file_name = table.Column<string>(nullable: true),
                    s3_folder_name = table.Column<string>(nullable: true),
                    is_archive = table.Column<bool>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteDocuments", x => x.sitedocument_id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SiteDocuments");
        }
    }
}
