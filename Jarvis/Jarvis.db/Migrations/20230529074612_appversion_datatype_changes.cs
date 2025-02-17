using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class appversion_datatype_changes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "store_app_version",
                table: "MobileAppVersion",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<string>(
                name: "force_to_update_app_version",
                table: "MobileAppVersion",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "store_app_version",
                table: "MobileAppVersion",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "force_to_update_app_version",
                table: "MobileAppVersion",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
