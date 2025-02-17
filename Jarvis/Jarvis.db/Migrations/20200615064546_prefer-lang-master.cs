using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Jarvis.db.Migrations
{
    public partial class preferlangmaster : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "prefer_language_id",
                table: "User",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LanguageMaster",
                columns: table => new
                {
                    language_id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    language_name = table.Column<string>(nullable: true),
                    is_active = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LanguageMaster", x => x.language_id);
                    table.ForeignKey(
                        name: "FK_LanguageMaster_StatusMasters_is_active",
                        column: x => x.is_active,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_User_prefer_language_id",
                table: "User",
                column: "prefer_language_id");

            migrationBuilder.CreateIndex(
                name: "IX_LanguageMaster_is_active",
                table: "LanguageMaster",
                column: "is_active");

            migrationBuilder.AddForeignKey(
                name: "FK_User_LanguageMaster_prefer_language_id",
                table: "User",
                column: "prefer_language_id",
                principalTable: "LanguageMaster",
                principalColumn: "language_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_LanguageMaster_prefer_language_id",
                table: "User");

            migrationBuilder.DropTable(
                name: "LanguageMaster");

            migrationBuilder.DropIndex(
                name: "IX_User_prefer_language_id",
                table: "User");

            migrationBuilder.DropColumn(
                name: "prefer_language_id",
                table: "User");
        }
    }
}
