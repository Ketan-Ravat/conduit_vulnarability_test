using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Jarvis.db.Migrations
{
    public partial class RemoveTowInspectionMultipleAppAccess : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_AppMaster_default_app",
                table: "User");

            migrationBuilder.DropTable(
                name: "UserAccessApp");

            migrationBuilder.DropTable(
                name: "AppMaster");

            migrationBuilder.DropIndex(
                name: "IX_User_default_app",
                table: "User");

            migrationBuilder.DropColumn(
                name: "default_app",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ti_default_role",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ti_default_site",
                table: "User");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "default_app",
                table: "User",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ti_default_role",
                table: "User",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ti_default_site",
                table: "User",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AppMaster",
                columns: table => new
                {
                    app_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    app_name = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppMaster", x => x.app_id);
                    table.ForeignKey(
                        name: "FK_AppMaster_StatusMasters_status",
                        column: x => x.status,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserAccessApp",
                columns: table => new
                {
                    user_access_app_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    app_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    modified_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    modified_by = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAccessApp", x => x.user_access_app_id);
                    table.ForeignKey(
                        name: "FK_UserAccessApp_AppMaster_app_id",
                        column: x => x.app_id,
                        principalTable: "AppMaster",
                        principalColumn: "app_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAccessApp_StatusMasters_status",
                        column: x => x.status,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAccessApp_User_user_id",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "uuid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_User_default_app",
                table: "User",
                column: "default_app");

            migrationBuilder.CreateIndex(
                name: "IX_AppMaster_status",
                table: "AppMaster",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccessApp_app_id",
                table: "UserAccessApp",
                column: "app_id");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccessApp_status",
                table: "UserAccessApp",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccessApp_user_id",
                table: "UserAccessApp",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_User_AppMaster_default_app",
                table: "User",
                column: "default_app",
                principalTable: "AppMaster",
                principalColumn: "app_id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
