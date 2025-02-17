using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class wo_type_addedclass_formio_mapping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "wo_type",
                table: "AssetClassFormIOMapping",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetClassFormIOMapping_wo_type",
                table: "AssetClassFormIOMapping",
                column: "wo_type");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetClassFormIOMapping_StatusMasters_wo_type",
                table: "AssetClassFormIOMapping",
                column: "wo_type",
                principalTable: "StatusMasters",
                principalColumn: "status_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetClassFormIOMapping_StatusMasters_wo_type",
                table: "AssetClassFormIOMapping");

            migrationBuilder.DropIndex(
                name: "IX_AssetClassFormIOMapping_wo_type",
                table: "AssetClassFormIOMapping");

            migrationBuilder.DropColumn(
                name: "wo_type",
                table: "AssetClassFormIOMapping");
        }
    }
}
