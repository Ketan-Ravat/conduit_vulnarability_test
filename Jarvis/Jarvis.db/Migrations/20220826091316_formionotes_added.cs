using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Jarvis.db.Migrations
{
    public partial class formionotes_added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FormIOLocationNotes",
                columns: table => new
                {
                    FormIOLocationNotes_id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    created_at = table.Column<DateTime>(nullable: true),
                    formiobuilding_id = table.Column<int>(nullable: true),
                    formiofloor_id = table.Column<int>(nullable: true),
                    formioroom_id = table.Column<int>(nullable: true),
                    formiosection_id = table.Column<int>(nullable: true),
                    notes = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormIOLocationNotes", x => x.FormIOLocationNotes_id);
                    table.ForeignKey(
                        name: "FK_FormIOLocationNotes_FormIOBuildings_formiobuilding_id",
                        column: x => x.formiobuilding_id,
                        principalTable: "FormIOBuildings",
                        principalColumn: "formiobuilding_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FormIOLocationNotes_FormIOFloors_formiofloor_id",
                        column: x => x.formiofloor_id,
                        principalTable: "FormIOFloors",
                        principalColumn: "formiofloor_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FormIOLocationNotes_FormIORooms_formioroom_id",
                        column: x => x.formioroom_id,
                        principalTable: "FormIORooms",
                        principalColumn: "formioroom_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FormIOLocationNotes_FormIOSections_formiosection_id",
                        column: x => x.formiosection_id,
                        principalTable: "FormIOSections",
                        principalColumn: "formiosection_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FormIOLocationNotes_formiobuilding_id",
                table: "FormIOLocationNotes",
                column: "formiobuilding_id");

            migrationBuilder.CreateIndex(
                name: "IX_FormIOLocationNotes_formiofloor_id",
                table: "FormIOLocationNotes",
                column: "formiofloor_id");

            migrationBuilder.CreateIndex(
                name: "IX_FormIOLocationNotes_formioroom_id",
                table: "FormIOLocationNotes",
                column: "formioroom_id");

            migrationBuilder.CreateIndex(
                name: "IX_FormIOLocationNotes_formiosection_id",
                table: "FormIOLocationNotes",
                column: "formiosection_id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FormIOLocationNotes");
        }
    }
}
