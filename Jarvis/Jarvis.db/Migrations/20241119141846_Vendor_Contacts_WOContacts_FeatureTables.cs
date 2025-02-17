using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class Vendor_Contacts_WOContacts_FeatureTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "calendarId",
                table: "WorkOrders",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_reminder_required",
                table: "WorkOrders",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "reminders_frequency_json",
                table: "WorkOrders",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Vendors",
                columns: table => new
                {
                    vendor_id = table.Column<Guid>(nullable: false),
                    vendor_name = table.Column<string>(nullable: true),
                    vendor_email = table.Column<string>(nullable: true),
                    vendor_phone_number = table.Column<string>(nullable: true),
                    vendor_category = table.Column<string>(nullable: true),
                    vendor_address = table.Column<string>(nullable: true),
                    is_deleted = table.Column<bool>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true),
                    company_id = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendors", x => x.vendor_id);
                    table.ForeignKey(
                        name: "FK_Vendors_Company_company_id",
                        column: x => x.company_id,
                        principalTable: "Company",
                        principalColumn: "company_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    contact_id = table.Column<Guid>(nullable: false),
                    name = table.Column<string>(nullable: true),
                    category = table.Column<string>(nullable: true),
                    email = table.Column<string>(nullable: true),
                    phone_number = table.Column<string>(nullable: true),
                    notes = table.Column<string>(nullable: true),
                    mark_as_primary = table.Column<bool>(nullable: false),
                    vendor_id = table.Column<Guid>(nullable: false),
                    company_id = table.Column<Guid>(nullable: true),
                    is_deleted = table.Column<bool>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.contact_id);
                    table.ForeignKey(
                        name: "FK_Contacts_Company_company_id",
                        column: x => x.company_id,
                        principalTable: "Company",
                        principalColumn: "company_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Contacts_Vendors_vendor_id",
                        column: x => x.vendor_id,
                        principalTable: "Vendors",
                        principalColumn: "vendor_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkordersVendorContactsMapping",
                columns: table => new
                {
                    workorders_vendor_contacts_mapping_id = table.Column<Guid>(nullable: false),
                    vendor_id = table.Column<Guid>(nullable: true),
                    contact_id = table.Column<Guid>(nullable: false),
                    wo_id = table.Column<Guid>(nullable: false),
                    is_deleted = table.Column<bool>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkordersVendorContactsMapping", x => x.workorders_vendor_contacts_mapping_id);
                    table.ForeignKey(
                        name: "FK_WorkordersVendorContactsMapping_Contacts_contact_id",
                        column: x => x.contact_id,
                        principalTable: "Contacts",
                        principalColumn: "contact_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkordersVendorContactsMapping_Vendors_vendor_id",
                        column: x => x.vendor_id,
                        principalTable: "Vendors",
                        principalColumn: "vendor_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkordersVendorContactsMapping_WorkOrders_wo_id",
                        column: x => x.wo_id,
                        principalTable: "WorkOrders",
                        principalColumn: "wo_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_company_id",
                table: "Contacts",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_vendor_id",
                table: "Contacts",
                column: "vendor_id");

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_company_id",
                table: "Vendors",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_WorkordersVendorContactsMapping_contact_id",
                table: "WorkordersVendorContactsMapping",
                column: "contact_id");

            migrationBuilder.CreateIndex(
                name: "IX_WorkordersVendorContactsMapping_vendor_id",
                table: "WorkordersVendorContactsMapping",
                column: "vendor_id");

            migrationBuilder.CreateIndex(
                name: "IX_WorkordersVendorContactsMapping_wo_id",
                table: "WorkordersVendorContactsMapping",
                column: "wo_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkordersVendorContactsMapping");

            migrationBuilder.DropTable(
                name: "Contacts");

            migrationBuilder.DropTable(
                name: "Vendors");

            migrationBuilder.DropColumn(
                name: "calendarId",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "is_reminder_required",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "reminders_frequency_json",
                table: "WorkOrders");
        }
    }
}
