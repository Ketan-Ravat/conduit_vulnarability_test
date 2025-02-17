using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class CompanyFeatureMapping_Features_new_tables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Features",
                columns: table => new
                {
                    feature_id = table.Column<Guid>(nullable: false),
                    feature_name = table.Column<string>(nullable: true),
                    feature_description = table.Column<string>(nullable: true),
                    created_at = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Features", x => x.feature_id);
                });

            migrationBuilder.CreateTable(
                name: "CompanyFeatureMappings",
                columns: table => new
                {
                    company_feature_id = table.Column<Guid>(nullable: false),
                    company_id = table.Column<Guid>(nullable: true),
                    feature_id = table.Column<Guid>(nullable: true),
                    is_required = table.Column<bool>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyFeatureMappings", x => x.company_feature_id);
                    table.ForeignKey(
                        name: "FK_CompanyFeatureMappings_Company_company_id",
                        column: x => x.company_id,
                        principalTable: "Company",
                        principalColumn: "company_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyFeatureMappings_Features_feature_id",
                        column: x => x.feature_id,
                        principalTable: "Features",
                        principalColumn: "feature_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyFeatureMappings_company_id",
                table: "CompanyFeatureMappings",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyFeatureMappings_feature_id",
                table: "CompanyFeatureMappings",
                column: "feature_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyFeatureMappings");

            migrationBuilder.DropTable(
                name: "Features");
        }
    }
}
