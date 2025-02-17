﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Jarvis.db.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Jarvis.db.Migrations
{
    [DbContext(typeof(DBContextFactory))]
    [Migration("20200414060842_changetype-fixedby")]
    partial class changetypefixedby
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Jarvis.db.Models.Asset", b =>
                {
                    b.Property<Guid>("asset_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("asset_approved_by")
                        .HasColumnType("text");

                    b.Property<DateTime?>("asset_approved_on")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("asset_photo")
                        .HasColumnType("text");

                    b.Property<int?>("asset_request_status")
                        .HasColumnType("integer");

                    b.Property<string>("asset_requested_by")
                        .HasColumnType("text");

                    b.Property<DateTime?>("asset_requested_on")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("asset_serial_number")
                        .HasColumnType("text");

                    b.Property<string>("asset_type")
                        .HasColumnType("text");

                    b.Property<string>("children")
                        .HasColumnType("text");

                    b.Property<string>("company_id")
                        .HasColumnType("text");

                    b.Property<DateTime?>("created_at")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("current_stage")
                        .HasColumnType("text");

                    b.Property<Guid?>("inspectionform_id")
                        .HasColumnType("uuid");

                    b.Property<string>("internal_asset_id")
                        .HasColumnType("text");

                    b.Property<List<AssetsValueJsonObject>>("lastinspection_attribute_values")
                        .HasColumnType("jsonb");

                    b.Property<long?>("meter_hours")
                        .HasColumnType("bigint");

                    b.Property<string>("model_name")
                        .HasColumnType("text");

                    b.Property<string>("model_year")
                        .HasColumnType("text");

                    b.Property<DateTime?>("modified_at")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("modified_by")
                        .HasColumnType("text");

                    b.Property<string>("name")
                        .HasColumnType("text");

                    b.Property<string>("notes")
                        .HasColumnType("text");

                    b.Property<string>("parent")
                        .HasColumnType("text");

                    b.Property<string>("product_name")
                        .HasColumnType("text");

                    b.Property<Guid>("site_id")
                        .HasColumnType("uuid");

                    b.Property<string>("site_location")
                        .HasColumnType("text");

                    b.Property<int?>("status")
                        .HasColumnType("integer");

                    b.Property<int?>("usage")
                        .HasColumnType("integer");

                    b.HasKey("asset_id");

                    b.HasIndex("inspectionform_id");

                    b.HasIndex("site_id");

                    b.HasIndex("status");

                    b.ToTable("Assets");
                });

            modelBuilder.Entity("Jarvis.db.Models.AssetTransactionHistory", b =>
                {
                    b.Property<Guid>("asseet_txn_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("asset_id")
                        .HasColumnType("uuid");

                    b.Property<List<AssetsValueJsonObject>>("attributeValues")
                        .HasColumnType("jsonb");

                    b.Property<string>("comapny_id")
                        .HasColumnType("text");

                    b.Property<DateTime?>("created_at")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("inspection_form_id")
                        .HasColumnType("text");

                    b.Property<string>("inspection_id")
                        .HasColumnType("text");

                    b.Property<string>("manager_id")
                        .HasColumnType("text");

                    b.Property<long?>("meter_hours")
                        .HasColumnType("bigint");

                    b.Property<string>("operator_id")
                        .HasColumnType("text");

                    b.Property<int?>("shift")
                        .HasColumnType("integer");

                    b.Property<string>("site_id")
                        .HasColumnType("text");

                    b.Property<int?>("status")
                        .HasColumnType("integer");

                    b.HasKey("asseet_txn_id");

                    b.HasIndex("asset_id");

                    b.ToTable("AssetTransactionHistory");
                });

            modelBuilder.Entity("Jarvis.db.Models.Company", b =>
                {
                    b.Property<Guid>("company_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("company_code")
                        .HasColumnType("text");

                    b.Property<string>("company_name")
                        .HasColumnType("text");

                    b.Property<DateTime?>("created_at")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("created_by")
                        .HasColumnType("text");

                    b.Property<DateTime?>("modified_at")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("modified_by")
                        .HasColumnType("text");

                    b.Property<int?>("status")
                        .HasColumnType("integer");

                    b.HasKey("company_id");

                    b.HasIndex("status");

                    b.ToTable("Company");
                });

            modelBuilder.Entity("Jarvis.db.Models.DashboardOutstandingIssues", b =>
                {
                    b.Property<Guid>("site_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("created_at")
                        .HasColumnType("timestamp without time zone");

                    b.Property<List<ReportJsonData>>("data")
                        .HasColumnType("jsonb");

                    b.Property<DateTime>("modified_at")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("site_id");

                    b.ToTable("DashboardOutstandingIssues");
                });

            modelBuilder.Entity("Jarvis.db.Models.Inspection", b =>
                {
                    b.Property<Guid>("inspection_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("asset_id")
                        .HasColumnType("uuid");

                    b.Property<List<AssetsValueJsonObject>>("attribute_values")
                        .HasColumnType("jsonb");

                    b.Property<string>("company_id")
                        .HasColumnType("text");

                    b.Property<DateTime>("created_at")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("created_by")
                        .HasColumnType("text");

                    b.Property<DateTime?>("datetime_requested")
                        .HasColumnType("timestamp without time zone");

                    b.Property<ImagesListObject>("image_list")
                        .HasColumnType("jsonb");

                    b.Property<string>("manager_id")
                        .HasColumnType("text");

                    b.Property<string>("manager_notes")
                        .HasColumnType("text");

                    b.Property<long>("meter_hours")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("modified_at")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("modified_by")
                        .HasColumnType("text");

                    b.Property<Guid?>("operator_id")
                        .HasColumnType("uuid");

                    b.Property<string>("operator_notes")
                        .HasColumnType("text");

                    b.Property<int>("shift")
                        .HasColumnType("integer");

                    b.Property<Guid>("site_id")
                        .HasColumnType("uuid");

                    b.Property<int>("status")
                        .HasColumnType("integer");

                    b.HasKey("inspection_id");

                    b.HasIndex("asset_id");

                    b.HasIndex("operator_id");

                    b.HasIndex("site_id");

                    b.HasIndex("status");

                    b.ToTable("Inspection");
                });

            modelBuilder.Entity("Jarvis.db.Models.InspectionAttributeCategory", b =>
                {
                    b.Property<int>("category_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("name")
                        .HasColumnType("text");

                    b.HasKey("category_id");

                    b.ToTable("InspectionAttributeCategory");
                });

            modelBuilder.Entity("Jarvis.db.Models.InspectionFormAttributes", b =>
                {
                    b.Property<Guid>("attributes_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("category_id")
                        .HasColumnType("integer");

                    b.Property<string>("company_id")
                        .HasColumnType("text");

                    b.Property<string>("name")
                        .HasColumnType("text");

                    b.Property<string>("site_id")
                        .HasColumnType("text");

                    b.Property<AttributeValueJsonObject[]>("value_parameters")
                        .HasColumnType("jsonb");

                    b.Property<int>("values_type")
                        .HasColumnType("integer");

                    b.HasKey("attributes_id");

                    b.HasIndex("category_id");

                    b.ToTable("InspectionFormAttributes");
                });

            modelBuilder.Entity("Jarvis.db.Models.InspectionFormTypes", b =>
                {
                    b.Property<Guid>("inspection_form_type_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("company_id")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("created_at")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("created_by")
                        .HasColumnType("text");

                    b.Property<DateTime?>("modified_at")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("modified_by")
                        .HasColumnType("text");

                    b.Property<string>("name")
                        .HasColumnType("text");

                    b.Property<string>("site_id")
                        .HasColumnType("text");

                    b.Property<int?>("status")
                        .HasColumnType("integer");

                    b.HasKey("inspection_form_type_id");

                    b.HasIndex("company_id");

                    b.ToTable("InspectionFormTypes");
                });

            modelBuilder.Entity("Jarvis.db.Models.InspectionForms", b =>
                {
                    b.Property<Guid>("inspection_form_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("company_id")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("created_at")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("created_by")
                        .HasColumnType("text");

                    b.Property<FormAttributesJsonObject[]>("form_attributes")
                        .HasColumnType("jsonb");

                    b.Property<DateTime?>("modified_at")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("modified_by")
                        .HasColumnType("text");

                    b.Property<string>("name")
                        .HasColumnType("text");

                    b.Property<string>("site_id")
                        .HasColumnType("text");

                    b.Property<int?>("status")
                        .HasColumnType("integer");

                    b.HasKey("inspection_form_id");

                    b.HasIndex("company_id");

                    b.HasIndex("status");

                    b.ToTable("InspectionForms");
                });

            modelBuilder.Entity("Jarvis.db.Models.MasterData", b =>
                {
                    b.Property<int>("master_data_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("meter_hours")
                        .HasColumnType("text");

                    b.HasKey("master_data_id");

                    b.ToTable("MasterData");
                });

            modelBuilder.Entity("Jarvis.db.Models.NotificationData", b =>
                {
                    b.Property<Guid>("notification_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("OS")
                        .HasColumnType("text");

                    b.Property<DateTime>("createdDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("data")
                        .HasColumnType("jsonb");

                    b.Property<string>("device_key")
                        .HasColumnType("text");

                    b.Property<string>("heading")
                        .HasColumnType("text");

                    b.Property<string>("message")
                        .HasColumnType("text");

                    b.Property<int>("notification_type")
                        .HasColumnType("integer");

                    b.Property<string>("ref_id")
                        .HasColumnType("text");

                    b.Property<DateTime>("sendDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("status")
                        .HasColumnType("integer");

                    b.Property<int>("target_type")
                        .HasColumnType("integer");

                    b.Property<Guid>("user_id")
                        .HasColumnType("uuid");

                    b.HasKey("notification_id");

                    b.ToTable("NotificationData");
                });

            modelBuilder.Entity("Jarvis.db.Models.Role", b =>
                {
                    b.Property<Guid>("role_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("name")
                        .HasColumnType("text");

                    b.HasKey("role_id");

                    b.ToTable("Role");
                });

            modelBuilder.Entity("Jarvis.db.Models.Sites", b =>
                {
                    b.Property<Guid>("site_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("company_id")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("created_at")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("created_by")
                        .HasColumnType("text");

                    b.Property<string>("location")
                        .HasColumnType("text");

                    b.Property<DateTime?>("modified_at")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("modified_by")
                        .HasColumnType("text");

                    b.Property<string>("site_code")
                        .HasColumnType("text");

                    b.Property<string>("site_name")
                        .HasColumnType("text");

                    b.Property<int?>("status")
                        .HasColumnType("integer");

                    b.HasKey("site_id");

                    b.HasIndex("company_id");

                    b.HasIndex("status");

                    b.ToTable("Sites");
                });

            modelBuilder.Entity("Jarvis.db.Models.StatusMaster", b =>
                {
                    b.Property<int>("status_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("status_name")
                        .HasColumnType("text");

                    b.Property<int>("status_type_id")
                        .HasColumnType("integer");

                    b.HasKey("status_id");

                    b.HasIndex("status_type_id");

                    b.ToTable("StatusMasters");
                });

            modelBuilder.Entity("Jarvis.db.Models.StatusTypes", b =>
                {
                    b.Property<int>("status_type_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("status_type_name")
                        .HasColumnType("text");

                    b.HasKey("status_type_id");

                    b.ToTable("StatusTypes");
                });

            modelBuilder.Entity("Jarvis.db.Models.User", b =>
                {
                    b.Property<Guid>("uuid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("barcode_id")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("created_at")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("created_by")
                        .HasColumnType("text");

                    b.Property<string>("email")
                        .HasColumnType("text");

                    b.Property<string>("firstname")
                        .HasColumnType("text");

                    b.Property<string>("lastname")
                        .HasColumnType("text");

                    b.Property<DateTime?>("modified_at")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("modified_by")
                        .HasColumnType("text");

                    b.Property<string>("notification_token")
                        .HasColumnType("text");

                    b.Property<string>("os")
                        .HasColumnType("text");

                    b.Property<string>("password")
                        .HasColumnType("text");

                    b.Property<Guid>("role_id")
                        .HasColumnType("uuid");

                    b.Property<int?>("status")
                        .HasColumnType("integer");

                    b.Property<string>("username")
                        .HasColumnType("text");

                    b.HasKey("uuid");

                    b.HasIndex("role_id");

                    b.HasIndex("status");

                    b.ToTable("User");
                });

            modelBuilder.Entity("Jarvis.db.Models.UserSites", b =>
                {
                    b.Property<Guid>("usersite_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("company_id")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("created_at")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("created_by")
                        .HasColumnType("text");

                    b.Property<DateTime?>("modified_at")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("modified_by")
                        .HasColumnType("text");

                    b.Property<Guid>("site_id")
                        .HasColumnType("uuid");

                    b.Property<int?>("status")
                        .HasColumnType("integer");

                    b.Property<Guid>("user_id")
                        .HasColumnType("uuid");

                    b.HasKey("usersite_id");

                    b.HasIndex("site_id");

                    b.HasIndex("status");

                    b.HasIndex("user_id");

                    b.ToTable("UserSites");
                });

            modelBuilder.Entity("Jarvis.db.Models.WorkOrder", b =>
                {
                    b.Property<Guid>("work_order_uuid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("asset_id")
                        .HasColumnType("uuid");

                    b.Property<Guid>("attribute_id")
                        .HasColumnType("uuid");

                    b.Property<AssetsValueJsonObject[]>("attributes_value")
                        .HasColumnType("jsonb");

                    b.Property<DateTime>("checkout_datetime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<List<CommentJsonObject>>("comments")
                        .HasColumnType("jsonb");

                    b.Property<string>("company_id")
                        .HasColumnType("text");

                    b.Property<DateTime?>("created_at")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("created_by")
                        .HasColumnType("text");

                    b.Property<string>("description")
                        .HasColumnType("text");

                    b.Property<Guid>("inspection_id")
                        .HasColumnType("uuid");

                    b.Property<string>("internal_asset_id")
                        .HasColumnType("text");

                    b.Property<string>("maintainence_staff_id")
                        .HasColumnType("text");

                    b.Property<DateTime?>("modified_at")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("modified_by")
                        .HasColumnType("text");

                    b.Property<string>("name")
                        .HasColumnType("text");

                    b.Property<string>("notes")
                        .HasColumnType("text");

                    b.Property<int>("priority")
                        .HasColumnType("integer");

                    b.Property<DateTime>("requested_datetime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid?>("site_id")
                        .HasColumnType("uuid");

                    b.Property<int>("status")
                        .HasColumnType("integer");

                    b.Property<long?>("work_order_number")
                        .HasColumnType("bigint");

                    b.HasKey("work_order_uuid");

                    b.HasIndex("asset_id");

                    b.HasIndex("attribute_id");

                    b.HasIndex("inspection_id");

                    b.HasIndex("site_id");

                    b.HasIndex("status");

                    b.ToTable("WorkOrder");
                });

            modelBuilder.Entity("Jarvis.db.Models.WorkOrderRecord", b =>
                {
                    b.Property<Guid>("work_order_record_uuid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("asset_id")
                        .HasColumnType("uuid");

                    b.Property<Guid>("attrubute_id")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("checkout_datetime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("created_at")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("created_by")
                        .HasColumnType("text");

                    b.Property<string>("fixed_by")
                        .HasColumnType("text");

                    b.Property<DateTime?>("fixed_datetime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid>("inspection_id")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("requested_datetime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("status")
                        .HasColumnType("integer");

                    b.Property<Guid>("work_order_uuid")
                        .HasColumnType("uuid");

                    b.HasKey("work_order_record_uuid");

                    b.HasIndex("asset_id");

                    b.HasIndex("attrubute_id");

                    b.HasIndex("inspection_id");

                    b.HasIndex("status");

                    b.HasIndex("work_order_uuid")
                        .IsUnique();

                    b.ToTable("WorkOrderRecord");
                });

            modelBuilder.Entity("Jarvis.db.Models.WorkOrderStatus", b =>
                {
                    b.Property<Guid>("work_order_status_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("modified_at")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("modified_by")
                        .HasColumnType("text");

                    b.Property<int>("status")
                        .HasColumnType("integer");

                    b.Property<Guid>("work_order_id")
                        .HasColumnType("uuid");

                    b.HasKey("work_order_status_id");

                    b.HasIndex("work_order_id");

                    b.ToTable("WorkOrderStatus");
                });

            modelBuilder.Entity("Jarvis.db.Models.Asset", b =>
                {
                    b.HasOne("Jarvis.db.Models.InspectionForms", "InspectionForms")
                        .WithMany("Asset")
                        .HasForeignKey("inspectionform_id");

                    b.HasOne("Jarvis.db.Models.Sites", "Sites")
                        .WithMany("Asset")
                        .HasForeignKey("site_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Jarvis.db.Models.StatusMaster", "StatusMaster")
                        .WithMany("Asset")
                        .HasForeignKey("status");
                });

            modelBuilder.Entity("Jarvis.db.Models.AssetTransactionHistory", b =>
                {
                    b.HasOne("Jarvis.db.Models.Asset", "Asset")
                        .WithMany("AssetTransactionHistory")
                        .HasForeignKey("asset_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Jarvis.db.Models.Company", b =>
                {
                    b.HasOne("Jarvis.db.Models.StatusMaster", "StatusMaster")
                        .WithMany("Company")
                        .HasForeignKey("status");
                });

            modelBuilder.Entity("Jarvis.db.Models.Inspection", b =>
                {
                    b.HasOne("Jarvis.db.Models.Asset", "Asset")
                        .WithMany("Inspection")
                        .HasForeignKey("asset_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Jarvis.db.Models.User", "User")
                        .WithMany("Inspection")
                        .HasForeignKey("operator_id");

                    b.HasOne("Jarvis.db.Models.Sites", "Sites")
                        .WithMany("Inspection")
                        .HasForeignKey("site_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Jarvis.db.Models.StatusMaster", "StatusMaster")
                        .WithMany("Inspection")
                        .HasForeignKey("status")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Jarvis.db.Models.InspectionFormAttributes", b =>
                {
                    b.HasOne("Jarvis.db.Models.InspectionAttributeCategory", "InspectionAttributeCategory")
                        .WithMany("InspectionFormAttributes")
                        .HasForeignKey("category_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Jarvis.db.Models.InspectionFormTypes", b =>
                {
                    b.HasOne("Jarvis.db.Models.Company", "Company")
                        .WithMany("InspectionFormTypes")
                        .HasForeignKey("company_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Jarvis.db.Models.InspectionForms", b =>
                {
                    b.HasOne("Jarvis.db.Models.Company", "Company")
                        .WithMany("InspectionForms")
                        .HasForeignKey("company_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Jarvis.db.Models.StatusMaster", "StatusMaster")
                        .WithMany("InspectionForms")
                        .HasForeignKey("status");
                });

            modelBuilder.Entity("Jarvis.db.Models.Sites", b =>
                {
                    b.HasOne("Jarvis.db.Models.Company", "Company")
                        .WithMany("Sites")
                        .HasForeignKey("company_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Jarvis.db.Models.StatusMaster", "StatusMaster")
                        .WithMany("Sites")
                        .HasForeignKey("status");
                });

            modelBuilder.Entity("Jarvis.db.Models.StatusMaster", b =>
                {
                    b.HasOne("Jarvis.db.Models.StatusTypes", "StatusTypes")
                        .WithMany("Status")
                        .HasForeignKey("status_type_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Jarvis.db.Models.User", b =>
                {
                    b.HasOne("Jarvis.db.Models.Role", "Role")
                        .WithMany("User")
                        .HasForeignKey("role_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Jarvis.db.Models.StatusMaster", "StatusMaster")
                        .WithMany("User")
                        .HasForeignKey("status");
                });

            modelBuilder.Entity("Jarvis.db.Models.UserSites", b =>
                {
                    b.HasOne("Jarvis.db.Models.Sites", "Sites")
                        .WithMany("UserSites")
                        .HasForeignKey("site_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Jarvis.db.Models.StatusMaster", "StatusMaster")
                        .WithMany("UserSites")
                        .HasForeignKey("status");

                    b.HasOne("Jarvis.db.Models.User", "User")
                        .WithMany("Usersites")
                        .HasForeignKey("user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Jarvis.db.Models.WorkOrder", b =>
                {
                    b.HasOne("Jarvis.db.Models.Asset", "Asset")
                        .WithMany("WorkOrders")
                        .HasForeignKey("asset_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Jarvis.db.Models.InspectionFormAttributes", "Attributes")
                        .WithMany("WorkOrder")
                        .HasForeignKey("attribute_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Jarvis.db.Models.Inspection", "Inspection")
                        .WithMany("WorkOrders")
                        .HasForeignKey("inspection_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Jarvis.db.Models.Sites", "Sites")
                        .WithMany("WorkOrder")
                        .HasForeignKey("site_id");

                    b.HasOne("Jarvis.db.Models.StatusMaster", "StatusMaster")
                        .WithMany("WorkOrder")
                        .HasForeignKey("status")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Jarvis.db.Models.WorkOrderRecord", b =>
                {
                    b.HasOne("Jarvis.db.Models.Asset", "Asset")
                        .WithMany("WorkOrderRecords")
                        .HasForeignKey("asset_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Jarvis.db.Models.InspectionFormAttributes", "Attributes")
                        .WithMany("WorkOrderRecord")
                        .HasForeignKey("attrubute_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Jarvis.db.Models.Inspection", "Inspection")
                        .WithMany("WorkOrderRecords")
                        .HasForeignKey("inspection_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Jarvis.db.Models.StatusMaster", "StatusMaster")
                        .WithMany("WorkOrderRecord")
                        .HasForeignKey("status")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Jarvis.db.Models.WorkOrder", "WorkOrder")
                        .WithOne("WorkOrderRecord")
                        .HasForeignKey("Jarvis.db.Models.WorkOrderRecord", "work_order_uuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Jarvis.db.Models.WorkOrderStatus", b =>
                {
                    b.HasOne("Jarvis.db.Models.WorkOrder", "WorkOrder")
                        .WithMany("WorkOrderStatus")
                        .HasForeignKey("work_order_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
