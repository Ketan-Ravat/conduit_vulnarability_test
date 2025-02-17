using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class UpdateStatusAsset : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "status", table: "AssetTransactionHistory");

            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "AssetTransactionHistory",
                nullable: true);

            migrationBuilder.DropColumn(
                name: "status",
                table: "Assets");

            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "Assets",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "status",
                table: "AssetTransactionHistory",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "Assets",
                type: "text",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
