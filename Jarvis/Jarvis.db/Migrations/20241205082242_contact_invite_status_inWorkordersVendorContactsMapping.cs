using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class contact_invite_status_inWorkordersVendorContactsMapping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "contact_invite_status",
                table: "WorkordersVendorContactsMapping",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "contact_invite_status",
                table: "WorkordersVendorContactsMapping");
        }
    }
}
