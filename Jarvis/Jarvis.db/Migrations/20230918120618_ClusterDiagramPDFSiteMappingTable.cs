using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Jarvis.db.Migrations
{
    public partial class ClusterDiagramPDFSiteMappingTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClusterDiagramPDFSiteMapping",
                columns: table => new
                {
                    cluster_diagram_pdf_id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    file_name = table.Column<string>(nullable: true),
                    upload_status = table.Column<int>(nullable: true),
                    site_id = table.Column<Guid>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true),
                    is_deleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClusterDiagramPDFSiteMapping", x => x.cluster_diagram_pdf_id);
                    table.ForeignKey(
                        name: "FK_ClusterDiagramPDFSiteMapping_Sites_site_id",
                        column: x => x.site_id,
                        principalTable: "Sites",
                        principalColumn: "site_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClusterDiagramPDFSiteMapping_site_id",
                table: "ClusterDiagramPDFSiteMapping",
                column: "site_id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClusterDiagramPDFSiteMapping");
        }
    }
}
