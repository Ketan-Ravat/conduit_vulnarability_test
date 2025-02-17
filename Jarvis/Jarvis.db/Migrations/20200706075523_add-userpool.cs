using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class adduserpool : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "identity_pool_id",
                table: "Company",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "region",
                table: "Company",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "user_pool_id",
                table: "Company",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "user_pool_web_client_id",
                table: "Company",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    userrole_id = table.Column<Guid>(nullable: false),
                    user_id = table.Column<Guid>(nullable: false),
                    role_id = table.Column<Guid>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true),
                    status = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.userrole_id);
                    table.ForeignKey(
                        name: "FK_UserRoles_Role_role_id",
                        column: x => x.role_id,
                        principalTable: "Role",
                        principalColumn: "role_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_StatusMasters_status",
                        column: x => x.status,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRoles_User_user_id",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "uuid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_role_id",
                table: "UserRoles",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_status",
                table: "UserRoles",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_user_id",
                table: "UserRoles",
                column: "user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropColumn(
                name: "identity_pool_id",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "region",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "user_pool_id",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "user_pool_web_client_id",
                table: "Company");
        }
    }
}
