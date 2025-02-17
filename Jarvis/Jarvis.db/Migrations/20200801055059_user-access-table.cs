using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Jarvis.db.Migrations
{
    public partial class useraccesstable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserAccessApp",
                columns: table => new
                {
                    user_access_app_id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(nullable: false),
                    app_id = table.Column<int>(nullable: false),
                    status = table.Column<int>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true, defaultValueSql: "timezone('UTC'::text, now())"),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true)
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserAccessApp");
        }
    }
}
