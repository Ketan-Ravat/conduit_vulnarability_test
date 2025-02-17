using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class clientcompany_codeadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AssetFormIO_asset_id",
                table: "AssetFormIO");

            migrationBuilder.AddColumn<string>(
                name: "clientcompany_code",
                table: "ClientCompany",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetFormIO_asset_id",
                table: "AssetFormIO",
                column: "asset_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AssetFormIO_asset_id",
                table: "AssetFormIO");

            migrationBuilder.DropColumn(
                name: "clientcompany_code",
                table: "ClientCompany");

            migrationBuilder.CreateIndex(
                name: "IX_AssetFormIO_asset_id",
                table: "AssetFormIO",
                column: "asset_id",
                unique: true);
        }
    }
}
