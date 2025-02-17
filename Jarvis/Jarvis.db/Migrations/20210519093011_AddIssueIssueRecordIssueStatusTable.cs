using System;
using System.Collections.Generic;
using Jarvis.db.Models;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class AddIssueIssueRecordIssueStatusTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Issue",
                columns: table => new
                {
                    issue_uuid = table.Column<Guid>(nullable: false),
                    asset_id = table.Column<Guid>(nullable: false),
                    attribute_id = table.Column<Guid>(nullable: false),
                    inspection_id = table.Column<Guid>(nullable: false),
                    internal_asset_id = table.Column<string>(nullable: true),
                    name = table.Column<string>(nullable: true),
                    issue_number = table.Column<long>(nullable: true),
                    description = table.Column<string>(nullable: true),
                    notes = table.Column<string>(nullable: true),
                    attributes_value = table.Column<AssetsValueJsonObject[]>(type: "jsonb", nullable: true),
                    status = table.Column<int>(nullable: false),
                    comments = table.Column<List<CommentJsonObject>>(type: "jsonb", nullable: true),
                    priority = table.Column<int>(nullable: false),
                    checkout_datetime = table.Column<DateTime>(nullable: false),
                    requested_datetime = table.Column<DateTime>(nullable: false),
                    created_by = table.Column<string>(nullable: true),
                    created_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    updated_at = table.Column<DateTime>(nullable: true),
                    site_id = table.Column<Guid>(nullable: true),
                    company_id = table.Column<string>(nullable: true),
                    maintainence_staff_id = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Issue", x => x.issue_uuid);
                    table.ForeignKey(
                        name: "FK_Issue_Assets_asset_id",
                        column: x => x.asset_id,
                        principalTable: "Assets",
                        principalColumn: "asset_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Issue_InspectionFormAttributes_attribute_id",
                        column: x => x.attribute_id,
                        principalTable: "InspectionFormAttributes",
                        principalColumn: "attributes_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Issue_Inspection_inspection_id",
                        column: x => x.inspection_id,
                        principalTable: "Inspection",
                        principalColumn: "inspection_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Issue_Sites_site_id",
                        column: x => x.site_id,
                        principalTable: "Sites",
                        principalColumn: "site_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Issue_StatusMasters_status",
                        column: x => x.status,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IssueRecord",
                columns: table => new
                {
                    issue_record_uuid = table.Column<Guid>(nullable: false),
                    issue_uuid = table.Column<Guid>(nullable: false),
                    attrubute_id = table.Column<Guid>(nullable: false),
                    asset_id = table.Column<Guid>(nullable: false),
                    inspection_id = table.Column<Guid>(nullable: false),
                    status = table.Column<int>(nullable: false),
                    requested_datetime = table.Column<DateTime>(nullable: false),
                    checkout_datetime = table.Column<DateTime>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: false),
                    created_by = table.Column<string>(nullable: true),
                    fixed_datetime = table.Column<DateTime>(nullable: true),
                    fixed_by = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssueRecord", x => x.issue_record_uuid);
                    table.ForeignKey(
                        name: "FK_IssueRecord_Assets_asset_id",
                        column: x => x.asset_id,
                        principalTable: "Assets",
                        principalColumn: "asset_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IssueRecord_InspectionFormAttributes_attrubute_id",
                        column: x => x.attrubute_id,
                        principalTable: "InspectionFormAttributes",
                        principalColumn: "attributes_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IssueRecord_Inspection_inspection_id",
                        column: x => x.inspection_id,
                        principalTable: "Inspection",
                        principalColumn: "inspection_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IssueRecord_Issue_issue_uuid",
                        column: x => x.issue_uuid,
                        principalTable: "Issue",
                        principalColumn: "issue_uuid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IssueRecord_StatusMasters_status",
                        column: x => x.status,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IssueStatus",
                columns: table => new
                {
                    issue_status_id = table.Column<Guid>(nullable: false),
                    issue_id = table.Column<Guid>(nullable: false),
                    status = table.Column<int>(nullable: false),
                    modified_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssueStatus", x => x.issue_status_id);
                    table.ForeignKey(
                        name: "FK_IssueStatus_Issue_issue_id",
                        column: x => x.issue_id,
                        principalTable: "Issue",
                        principalColumn: "issue_uuid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Issue_asset_id",
                table: "Issue",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "IX_Issue_attribute_id",
                table: "Issue",
                column: "attribute_id");

            migrationBuilder.CreateIndex(
                name: "IX_Issue_inspection_id",
                table: "Issue",
                column: "inspection_id");

            migrationBuilder.CreateIndex(
                name: "IX_Issue_site_id",
                table: "Issue",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "IX_Issue_status",
                table: "Issue",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_IssueRecord_asset_id",
                table: "IssueRecord",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "IX_IssueRecord_attrubute_id",
                table: "IssueRecord",
                column: "attrubute_id");

            migrationBuilder.CreateIndex(
                name: "IX_IssueRecord_inspection_id",
                table: "IssueRecord",
                column: "inspection_id");

            migrationBuilder.CreateIndex(
                name: "IX_IssueRecord_issue_uuid",
                table: "IssueRecord",
                column: "issue_uuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IssueRecord_status",
                table: "IssueRecord",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_IssueStatus_issue_id",
                table: "IssueStatus",
                column: "issue_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IssueRecord");

            migrationBuilder.DropTable(
                name: "IssueStatus");

            migrationBuilder.DropTable(
                name: "Issue");
        }
    }
}
