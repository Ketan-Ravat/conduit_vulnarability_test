using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Jarvis.db.Migrations
{
    public partial class defaultappmaster : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "default_app",
                table: "User",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AppMaster",
                columns: table => new
                {
                    app_id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    app_name = table.Column<string>(nullable: true),
                    status = table.Column<int>(nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_User_default_app",
                table: "User",
                column: "default_app");

            migrationBuilder.CreateIndex(
                name: "IX_AppMaster_status",
                table: "AppMaster",
                column: "status");

            migrationBuilder.AddForeignKey(
                name: "FK_User_AppMaster_default_app",
                table: "User",
                column: "default_app",
                principalTable: "AppMaster",
                principalColumn: "app_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_AppMaster_default_app",
                table: "User");

            migrationBuilder.DropTable(
                name: "AppMaster");

            migrationBuilder.DropIndex(
                name: "IX_User_default_app",
                table: "User");

            migrationBuilder.DropColumn(
                name: "default_app",
                table: "User");
        }
    }
}
