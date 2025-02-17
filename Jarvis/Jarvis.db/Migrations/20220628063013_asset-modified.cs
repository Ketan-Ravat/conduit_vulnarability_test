using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Jarvis.db.Migrations
{
    public partial class assetmodified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "customer",
                table: "Sites",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "customer_address",
                table: "Sites",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "owner",
                table: "ClientCompany",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "owner_address",
                table: "ClientCompany",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "asset_hierarchy_id",
                table: "Assets",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "form_type_id",
                table: "Assets",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Assethierarchy",
                columns: table => new
                {
                    asset_hierarchy_id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    asset_hierarchy_name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assethierarchy", x => x.asset_hierarchy_id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assets_asset_hierarchy_id",
                table: "Assets",
                column: "asset_hierarchy_id");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_form_type_id",
                table: "Assets",
                column: "form_type_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_Assethierarchy_asset_hierarchy_id",
                table: "Assets",
                column: "asset_hierarchy_id",
                principalTable: "Assethierarchy",
                principalColumn: "asset_hierarchy_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_FormIOType_form_type_id",
                table: "Assets",
                column: "form_type_id",
                principalTable: "FormIOType",
                principalColumn: "form_type_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_Assethierarchy_asset_hierarchy_id",
                table: "Assets");

            migrationBuilder.DropForeignKey(
                name: "FK_Assets_FormIOType_form_type_id",
                table: "Assets");

            migrationBuilder.DropTable(
                name: "Assethierarchy");

            migrationBuilder.DropIndex(
                name: "IX_Assets_asset_hierarchy_id",
                table: "Assets");

            migrationBuilder.DropIndex(
                name: "IX_Assets_form_type_id",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "customer",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "customer_address",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "owner",
                table: "ClientCompany");

            migrationBuilder.DropColumn(
                name: "owner_address",
                table: "ClientCompany");

            migrationBuilder.DropColumn(
                name: "asset_hierarchy_id",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "form_type_id",
                table: "Assets");
        }
    }
}
