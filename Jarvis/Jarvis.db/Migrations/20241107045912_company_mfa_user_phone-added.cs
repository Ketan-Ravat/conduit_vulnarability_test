using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class company_mfa_user_phoneadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "phone_number",
                table: "User",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "cognito_mfa_timer",
                table: "Company",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "phone_number",
                table: "User");

            migrationBuilder.DropColumn(
                name: "cognito_mfa_timer",
                table: "Company");
        }
    }
}
