using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Jarvis.db.Migrations
{
    public partial class app_versionadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MobileAppVersion",
                columns: table => new
                {
                    mobileappversion_id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    force_to_update_app_version = table.Column<double>(nullable: false),
                    store_app_version = table.Column<double>(nullable: false),
                    device_brand = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MobileAppVersion", x => x.mobileappversion_id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MobileAppVersion");
        }
    }
}
