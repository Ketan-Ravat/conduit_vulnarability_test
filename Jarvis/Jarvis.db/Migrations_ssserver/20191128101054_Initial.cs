using System;
using Jarvis.db.Models;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Jarvis.db.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Assets",
                columns: table => new
                {
                    asset_id = table.Column<Guid>(nullable: false),
                    internal_asset_id = table.Column<string>(nullable: true),
                    company_id = table.Column<Guid>(nullable: false),
                    site_id = table.Column<Guid>(nullable: false),
                    status = table.Column<string>(nullable: true),
                    inspectionform_id = table.Column<Guid>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: false),
                    modified_at = table.Column<DateTime>(nullable: false),
                    notes = table.Column<string>(nullable: true),
                    asset_request_status = table.Column<int>(nullable: true),
                    asset_requested_by = table.Column<Guid>(nullable: false),
                    asset_requested_on = table.Column<DateTime>(nullable: false),
                    asset_approved_by = table.Column<Guid>(nullable: false),
                    asset_approved_on = table.Column<DateTime>(nullable: false),
                    lastinspection_attribute_values = table.Column<AssetsValueJsonObject[]>(type: "jsonb", nullable: true),
                    usage = table.Column<int>(nullable: false),
                    meter_hours = table.Column<long>(nullable: false),
                    name = table.Column<string>(nullable: true),
                    asset_type = table.Column<string>(nullable: true),
                    product_name = table.Column<string>(nullable: true),
                    model_name = table.Column<string>(nullable: true),
                    asset_serial_number = table.Column<string>(nullable: true),
                    model_year = table.Column<string>(nullable: true),
                    site_location = table.Column<string>(nullable: true),
                    current_stage = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets", x => x.asset_id);
                });

            migrationBuilder.CreateTable(
                name: "AssetTransactionHistory",
                columns: table => new
                {
                    asseet_txn_id = table.Column<Guid>(nullable: false),
                    asset_id = table.Column<Guid>(nullable: false),
                    inspection_id = table.Column<string>(nullable: true),
                    operator_id = table.Column<string>(nullable: true),
                    manager_id = table.Column<string>(nullable: true),
                    status = table.Column<int>(nullable: false),
                    comapny_id = table.Column<string>(nullable: true),
                    site_id = table.Column<string>(nullable: true),
                    attributeValues = table.Column<AssetsValueJsonObject>(type: "jsonb", nullable: true),
                    inspection_form_id = table.Column<string>(nullable: true),
                    meter_hours = table.Column<long>(nullable: false),
                    shift = table.Column<int>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetTransactionHistory", x => x.asseet_txn_id);
                });

            migrationBuilder.CreateTable(
                name: "Company",
                columns: table => new
                {
                    company_id = table.Column<Guid>(nullable: false),
                    company_name = table.Column<string>(nullable: true),
                    company_code = table.Column<string>(nullable: true),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true),
                    status = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Company", x => x.company_id);
                });

            migrationBuilder.CreateTable(
                name: "InspectionAttributeCategory",
                columns: table => new
                {
                    category_id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionAttributeCategory", x => x.category_id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    role_id = table.Column<Guid>(nullable: false),
                    name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.role_id);
                });

            migrationBuilder.CreateTable(
                name: "StatusTypes",
                columns: table => new
                {
                    status_type_id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    status_type_name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusTypes", x => x.status_type_id);
                });

            migrationBuilder.CreateTable(
                name: "InspectionForms",
                columns: table => new
                {
                    inspection_form_id = table.Column<Guid>(nullable: false),
                    name = table.Column<string>(nullable: true),
                    company_id = table.Column<Guid>(nullable: false),
                    site_id = table.Column<string>(nullable: true),
                    status = table.Column<int>(nullable: true),
                    form_attributes = table.Column<FormAttributesJsonObject[]>(type: "jsonb", nullable: true),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionForms", x => x.inspection_form_id);
                    table.ForeignKey(
                        name: "FK_InspectionForms_Company_company_id",
                        column: x => x.company_id,
                        principalTable: "Company",
                        principalColumn: "company_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InspectionFormTypes",
                columns: table => new
                {
                    inspection_form_type_id = table.Column<Guid>(nullable: false),
                    name = table.Column<string>(nullable: true),
                    company_id = table.Column<Guid>(nullable: false),
                    site_id = table.Column<string>(nullable: true),
                    status = table.Column<int>(nullable: true),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionFormTypes", x => x.inspection_form_type_id);
                    table.ForeignKey(
                        name: "FK_InspectionFormTypes_Company_company_id",
                        column: x => x.company_id,
                        principalTable: "Company",
                        principalColumn: "company_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sites",
                columns: table => new
                {
                    site_id = table.Column<Guid>(nullable: false),
                    company_id = table.Column<Guid>(nullable: false),
                    site_name = table.Column<string>(nullable: true),
                    site_code = table.Column<string>(nullable: true),
                    location = table.Column<string>(nullable: true),
                    status = table.Column<int>(nullable: true),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sites", x => x.site_id);
                    table.ForeignKey(
                        name: "FK_Sites_Company_company_id",
                        column: x => x.company_id,
                        principalTable: "Company",
                        principalColumn: "company_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InspectionFormAttributes",
                columns: table => new
                {
                    attributes_id = table.Column<Guid>(nullable: false),
                    name = table.Column<string>(nullable: true),
                    values_type = table.Column<int>(nullable: false),
                    company_id = table.Column<string>(nullable: true),
                    category_id = table.Column<int>(nullable: false),
                    site_id = table.Column<string>(nullable: true),
                    value_parameters = table.Column<AttributeValueJsonObject[]>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionFormAttributes", x => x.attributes_id);
                    table.ForeignKey(
                        name: "FK_InspectionFormAttributes_InspectionAttributeCategory_catego~",
                        column: x => x.category_id,
                        principalTable: "InspectionAttributeCategory",
                        principalColumn: "category_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    uuid = table.Column<Guid>(nullable: false),
                    role_id = table.Column<Guid>(nullable: false),
                    email = table.Column<string>(nullable: true),
                    password = table.Column<string>(nullable: true),
                    username = table.Column<string>(nullable: true),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true),
                    status = table.Column<int>(nullable: true),
                    notification_token = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.uuid);
                    table.ForeignKey(
                        name: "FK_User_Role_role_id",
                        column: x => x.role_id,
                        principalTable: "Role",
                        principalColumn: "role_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StatusMasters",
                columns: table => new
                {
                    status_id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    status_type_id = table.Column<long>(nullable: false),
                    status_name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusMasters", x => x.status_id);
                    table.ForeignKey(
                        name: "FK_StatusMasters_StatusTypes_status_type_id",
                        column: x => x.status_type_id,
                        principalTable: "StatusTypes",
                        principalColumn: "status_type_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSites",
                columns: table => new
                {
                    usersite_id = table.Column<Guid>(nullable: false),
                    user_id = table.Column<Guid>(nullable: false),
                    company_id = table.Column<Guid>(nullable: false),
                    site_id = table.Column<Guid>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true),
                    status = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSites", x => x.usersite_id);
                    table.ForeignKey(
                        name: "FK_UserSites_Sites_site_id",
                        column: x => x.site_id,
                        principalTable: "Sites",
                        principalColumn: "site_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserSites_User_user_id",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "uuid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InspectionFormAttributes_category_id",
                table: "InspectionFormAttributes",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionForms_company_id",
                table: "InspectionForms",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionFormTypes_company_id",
                table: "InspectionFormTypes",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_Sites_company_id",
                table: "Sites",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_StatusMasters_status_type_id",
                table: "StatusMasters",
                column: "status_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_User_role_id",
                table: "User",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_UserSites_site_id",
                table: "UserSites",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "IX_UserSites_user_id",
                table: "UserSites",
                column: "user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Assets");

            migrationBuilder.DropTable(
                name: "AssetTransactionHistory");

            migrationBuilder.DropTable(
                name: "InspectionFormAttributes");

            migrationBuilder.DropTable(
                name: "InspectionForms");

            migrationBuilder.DropTable(
                name: "InspectionFormTypes");

            migrationBuilder.DropTable(
                name: "StatusMasters");

            migrationBuilder.DropTable(
                name: "UserSites");

            migrationBuilder.DropTable(
                name: "InspectionAttributeCategory");

            migrationBuilder.DropTable(
                name: "StatusTypes");

            migrationBuilder.DropTable(
                name: "Sites");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Company");

            migrationBuilder.DropTable(
                name: "Role");
        }
    }
}
