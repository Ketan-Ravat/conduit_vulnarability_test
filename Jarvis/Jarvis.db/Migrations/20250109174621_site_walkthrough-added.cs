using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class site_walkthroughadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "access_notes",
                table: "TempMasterRoom",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "issue",
                table: "TempMasterRoom",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "room_conditions",
                table: "TempMasterRoom",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "city",
                table: "Sites",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "customer_address_2",
                table: "Sites",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "sitecontact_id",
                table: "Sites",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "state",
                table: "Sites",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "zip",
                table: "Sites",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "form_id",
                table: "PMs",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "work_procedure_type",
                table: "PMs",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "inpsection_form_type",
                table: "InspectionsTemplateFormIO",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "access_notes",
                table: "FormIORooms",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "issue",
                table: "FormIORooms",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "room_conditions",
                table: "FormIORooms",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "feature_type",
                table: "Features",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "user_id",
                table: "CompanyFeatureMappings",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "form_id",
                table: "AssetPMs",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "work_procedure_type",
                table: "AssetPMs",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FormIORoomsImagesMapping",
                columns: table => new
                {
                    formioroomsimagesmapping_id = table.Column<Guid>(nullable: false),
                    formioroom_id = table.Column<int>(nullable: false),
                    image_file_name = table.Column<string>(nullable: true),
                    image_thumbnail_file_name = table.Column<string>(nullable: true),
                    s3_image_folder_name = table.Column<string>(nullable: true),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true),
                    is_deleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormIORoomsImagesMapping", x => x.formioroomsimagesmapping_id);
                    table.ForeignKey(
                        name: "FK_FormIORoomsImagesMapping_FormIORooms_formioroom_id",
                        column: x => x.formioroom_id,
                        principalTable: "FormIORooms",
                        principalColumn: "formioroom_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SiteContact",
                columns: table => new
                {
                    sitecontact_id = table.Column<Guid>(nullable: false),
                    company_id = table.Column<Guid>(nullable: false),
                    sitecontact_title = table.Column<string>(nullable: true),
                    sitecontact_name = table.Column<string>(nullable: true),
                    sitecontact_email = table.Column<string>(nullable: true),
                    sitecontact_phone = table.Column<string>(nullable: true),
                    is_deleted = table.Column<bool>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteContact", x => x.sitecontact_id);
                    table.ForeignKey(
                        name: "FK_SiteContact_Company_company_id",
                        column: x => x.company_id,
                        principalTable: "Company",
                        principalColumn: "company_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SitewalkthroughTempPmEstimation",
                columns: table => new
                {
                    sitewalkthrough_temp_pm_estimation_id = table.Column<Guid>(nullable: false),
                    tempasset_id = table.Column<Guid>(nullable: false),
                    pm_plan_id = table.Column<Guid>(nullable: false),
                    pm_id = table.Column<Guid>(nullable: false),
                    woonboardingassets_id = table.Column<Guid>(nullable: false),
                    estimation_time = table.Column<int>(nullable: true),
                    inspectiontemplate_asset_class_id = table.Column<Guid>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true),
                    is_deleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SitewalkthroughTempPmEstimation", x => x.sitewalkthrough_temp_pm_estimation_id);
                    table.ForeignKey(
                        name: "FK_SitewalkthroughTempPmEstimation_InspectionTemplateAssetClas~",
                        column: x => x.inspectiontemplate_asset_class_id,
                        principalTable: "InspectionTemplateAssetClass",
                        principalColumn: "inspectiontemplate_asset_class_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SitewalkthroughTempPmEstimation_PMs_pm_id",
                        column: x => x.pm_id,
                        principalTable: "PMs",
                        principalColumn: "pm_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SitewalkthroughTempPmEstimation_PMPlans_pm_plan_id",
                        column: x => x.pm_plan_id,
                        principalTable: "PMPlans",
                        principalColumn: "pm_plan_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SitewalkthroughTempPmEstimation_TempAsset_tempasset_id",
                        column: x => x.tempasset_id,
                        principalTable: "TempAsset",
                        principalColumn: "tempasset_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SitewalkthroughTempPmEstimation_WOOnboardingAssets_woonboar~",
                        column: x => x.woonboardingassets_id,
                        principalTable: "WOOnboardingAssets",
                        principalColumn: "woonboardingassets_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TempMasterRoomImagesMapping",
                columns: table => new
                {
                    tempmasterimagesmapping_id = table.Column<Guid>(nullable: false),
                    temp_master_room_id = table.Column<Guid>(nullable: false),
                    image_file_name = table.Column<string>(nullable: true),
                    image_thumbnail_file_name = table.Column<string>(nullable: true),
                    s3_image_folder_name = table.Column<string>(nullable: true),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true),
                    is_deleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TempMasterRoomImagesMapping", x => x.tempmasterimagesmapping_id);
                    table.ForeignKey(
                        name: "FK_TempMasterRoomImagesMapping_TempMasterRoom_temp_master_room~",
                        column: x => x.temp_master_room_id,
                        principalTable: "TempMasterRoom",
                        principalColumn: "temp_master_room_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sites_sitecontact_id",
                table: "Sites",
                column: "sitecontact_id");

            migrationBuilder.CreateIndex(
                name: "IX_PMs_form_id",
                table: "PMs",
                column: "form_id");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyFeatureMappings_user_id",
                table: "CompanyFeatureMappings",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_AssetPMs_form_id",
                table: "AssetPMs",
                column: "form_id");

            migrationBuilder.CreateIndex(
                name: "IX_FormIORoomsImagesMapping_formioroom_id",
                table: "FormIORoomsImagesMapping",
                column: "formioroom_id");

            migrationBuilder.CreateIndex(
                name: "IX_SiteContact_company_id",
                table: "SiteContact",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_SitewalkthroughTempPmEstimation_inspectiontemplate_asset_cl~",
                table: "SitewalkthroughTempPmEstimation",
                column: "inspectiontemplate_asset_class_id");

            migrationBuilder.CreateIndex(
                name: "IX_SitewalkthroughTempPmEstimation_pm_id",
                table: "SitewalkthroughTempPmEstimation",
                column: "pm_id");

            migrationBuilder.CreateIndex(
                name: "IX_SitewalkthroughTempPmEstimation_pm_plan_id",
                table: "SitewalkthroughTempPmEstimation",
                column: "pm_plan_id");

            migrationBuilder.CreateIndex(
                name: "IX_SitewalkthroughTempPmEstimation_tempasset_id",
                table: "SitewalkthroughTempPmEstimation",
                column: "tempasset_id");

            migrationBuilder.CreateIndex(
                name: "IX_SitewalkthroughTempPmEstimation_woonboardingassets_id",
                table: "SitewalkthroughTempPmEstimation",
                column: "woonboardingassets_id");

            migrationBuilder.CreateIndex(
                name: "IX_TempMasterRoomImagesMapping_temp_master_room_id",
                table: "TempMasterRoomImagesMapping",
                column: "temp_master_room_id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetPMs_InspectionsTemplateFormIO_form_id",
                table: "AssetPMs",
                column: "form_id",
                principalTable: "InspectionsTemplateFormIO",
                principalColumn: "form_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyFeatureMappings_User_user_id",
                table: "CompanyFeatureMappings",
                column: "user_id",
                principalTable: "User",
                principalColumn: "uuid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PMs_InspectionsTemplateFormIO_form_id",
                table: "PMs",
                column: "form_id",
                principalTable: "InspectionsTemplateFormIO",
                principalColumn: "form_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sites_SiteContact_sitecontact_id",
                table: "Sites",
                column: "sitecontact_id",
                principalTable: "SiteContact",
                principalColumn: "sitecontact_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetPMs_InspectionsTemplateFormIO_form_id",
                table: "AssetPMs");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyFeatureMappings_User_user_id",
                table: "CompanyFeatureMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_PMs_InspectionsTemplateFormIO_form_id",
                table: "PMs");

            migrationBuilder.DropForeignKey(
                name: "FK_Sites_SiteContact_sitecontact_id",
                table: "Sites");

            migrationBuilder.DropTable(
                name: "FormIORoomsImagesMapping");

            migrationBuilder.DropTable(
                name: "SiteContact");

            migrationBuilder.DropTable(
                name: "SitewalkthroughTempPmEstimation");

            migrationBuilder.DropTable(
                name: "TempMasterRoomImagesMapping");

            migrationBuilder.DropIndex(
                name: "IX_Sites_sitecontact_id",
                table: "Sites");

            migrationBuilder.DropIndex(
                name: "IX_PMs_form_id",
                table: "PMs");

            migrationBuilder.DropIndex(
                name: "IX_CompanyFeatureMappings_user_id",
                table: "CompanyFeatureMappings");

            migrationBuilder.DropIndex(
                name: "IX_AssetPMs_form_id",
                table: "AssetPMs");

            migrationBuilder.DropColumn(
                name: "access_notes",
                table: "TempMasterRoom");

            migrationBuilder.DropColumn(
                name: "issue",
                table: "TempMasterRoom");

            migrationBuilder.DropColumn(
                name: "room_conditions",
                table: "TempMasterRoom");

            migrationBuilder.DropColumn(
                name: "city",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "customer_address_2",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "sitecontact_id",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "state",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "zip",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "form_id",
                table: "PMs");

            migrationBuilder.DropColumn(
                name: "work_procedure_type",
                table: "PMs");

            migrationBuilder.DropColumn(
                name: "inpsection_form_type",
                table: "InspectionsTemplateFormIO");

            migrationBuilder.DropColumn(
                name: "access_notes",
                table: "FormIORooms");

            migrationBuilder.DropColumn(
                name: "issue",
                table: "FormIORooms");

            migrationBuilder.DropColumn(
                name: "room_conditions",
                table: "FormIORooms");

            migrationBuilder.DropColumn(
                name: "feature_type",
                table: "Features");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "CompanyFeatureMappings");

            migrationBuilder.DropColumn(
                name: "form_id",
                table: "AssetPMs");

            migrationBuilder.DropColumn(
                name: "work_procedure_type",
                table: "AssetPMs");
        }
    }
}
