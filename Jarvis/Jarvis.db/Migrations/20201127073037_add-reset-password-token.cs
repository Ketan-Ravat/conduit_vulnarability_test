using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Jarvis.db.Migrations
{
    public partial class addresetpasswordtoken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ResetPasswordToken",
                columns: table => new
                {
                    token_id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    email = table.Column<string>(nullable: true),
                    token = table.Column<string>(nullable: true),
                    user_id = table.Column<Guid>(nullable: false),
                    is_used = table.Column<bool>(nullable: false),
                    used_at = table.Column<DateTime>(nullable: true),
                    created_at = table.Column<DateTime>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResetPasswordToken", x => x.token_id);
                    table.ForeignKey(
                        name: "FK_ResetPasswordToken_User_user_id",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "uuid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResetPasswordToken_user_id",
                table: "ResetPasswordToken",
                column: "user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResetPasswordToken");
        }
    }
}
