using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class form_id_nullableclass_form_mapping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetClassFormIOMapping_InspectionsTemplateFormIO_form_id",
                table: "AssetClassFormIOMapping");

            migrationBuilder.AlterColumn<Guid>(
                name: "form_id",
                table: "AssetClassFormIOMapping",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetClassFormIOMapping_InspectionsTemplateFormIO_form_id",
                table: "AssetClassFormIOMapping",
                column: "form_id",
                principalTable: "InspectionsTemplateFormIO",
                principalColumn: "form_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetClassFormIOMapping_InspectionsTemplateFormIO_form_id",
                table: "AssetClassFormIOMapping");

            migrationBuilder.AlterColumn<Guid>(
                name: "form_id",
                table: "AssetClassFormIOMapping",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetClassFormIOMapping_InspectionsTemplateFormIO_form_id",
                table: "AssetClassFormIOMapping",
                column: "form_id",
                principalTable: "InspectionsTemplateFormIO",
                principalColumn: "form_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
