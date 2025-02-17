using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class auto_desk_coloumn_added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "access_token",
                table: "Company",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "autodesk_app_id",
                table: "Company",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "autodesk_app_name",
                table: "Company",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "client_id",
                table: "Company",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "client_secret",
                table: "Company",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "expires_in",
                table: "Company",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "refresh_token",
                table: "Company",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "token_type",
                table: "Company",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "access_token",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "autodesk_app_id",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "autodesk_app_name",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "client_id",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "client_secret",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "expires_in",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "refresh_token",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "token_type",
                table: "Company");
        }
    }
}
