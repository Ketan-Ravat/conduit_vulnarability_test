using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class category_id_inContactsVendors : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "vendor_category_id",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "category_id",
                table: "Contacts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "vendor_category_id",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "category_id",
                table: "Contacts");
        }
    }
}
