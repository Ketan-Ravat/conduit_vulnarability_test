using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class assetnameplateadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           migrationBuilder.DropForeignKey(
                name: "FK_AssetFormIO_StatusMasters_pdf_report_status",
                table: "AssetFormIO");

            migrationBuilder.AddColumn<string>(
                name: "form_retrived_nameplate_info",
                table: "Assets",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "pdf_report_status",
                table: "AssetFormIO",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "form_retrived_nameplate_info",
                table: "AssetFormIO",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetFormIO_StatusMasters_pdf_report_status",
                table: "AssetFormIO",
                column: "pdf_report_status",
                principalTable: "StatusMasters",
                principalColumn: "status_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetFormIO_StatusMasters_pdf_report_status",
                table: "AssetFormIO");

            migrationBuilder.DropColumn(
                name: "form_retrived_nameplate_info",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "form_retrived_nameplate_info",
                table: "AssetFormIO");

            migrationBuilder.AlterColumn<int>(
                name: "pdf_report_status",
                table: "AssetFormIO",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetFormIO_StatusMasters_pdf_report_status",
                table: "AssetFormIO",
                column: "pdf_report_status",
                principalTable: "StatusMasters",
                principalColumn: "status_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
