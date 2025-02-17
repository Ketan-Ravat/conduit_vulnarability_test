using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class UserLocation_newtable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserLocation",
                columns: table => new
                {
                    user_location_id = table.Column<Guid>(nullable: false),
                    latitude = table.Column<string>(nullable: true),
                    longitude = table.Column<string>(nullable: true),
                    is_location_active = table.Column<bool>(nullable: false),
                    user_id = table.Column<Guid>(nullable: false),
                    device_id = table.Column<Guid>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLocation", x => x.user_location_id);
                    table.ForeignKey(
                        name: "FK_UserLocation_User_user_id",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "uuid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserLocation_user_id",
                table: "UserLocation",
                column: "user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserLocation");
        }
    }
}
