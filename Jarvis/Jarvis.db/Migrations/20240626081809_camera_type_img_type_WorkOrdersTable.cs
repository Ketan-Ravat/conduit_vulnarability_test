using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class camera_type_img_type_WorkOrdersTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ir_visual_camera_type",
                table: "WorkOrders",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ir_visual_image_type",
                table: "WorkOrders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ir_visual_camera_type",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "ir_visual_image_type",
                table: "WorkOrders");
        }
    }
}
