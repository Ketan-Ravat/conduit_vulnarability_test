using Jarvis.db.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.db.EntityConfigurations
{
    public class AssetConfiguration : IEntityTypeConfiguration<Asset>
    {
        public void Configure(EntityTypeBuilder<Asset> builder)
        {
            builder.HasKey(e => e.asset_id)
                .HasName("asset_pk");

            builder.Property(e => e.asset_id)
                .HasColumnName("asset_id")
                .ValueGeneratedOnAdd();

            builder.Property(e => e.asset_photo)
                .HasColumnName("asset_photo");

            builder.Property(e => e.internal_asset_id)
                .IsRequired()
                .HasColumnName("internal_asset_id")
                .HasColumnType("character varying");

            builder.Property(e => e.company_id)
                .HasColumnName("company_id")
                .HasColumnType("character varying");


            builder.Property(e => e.site_id)
                .HasColumnName("site_id");

            builder.HasOne(d => d.Sites)
                .WithMany(p => p.Asset)
                .HasForeignKey(d => d.Sites.site_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("site_asset_fk");

            builder.Property(e => e.status)
                .HasColumnName("status")
                .HasColumnType("SMALLINT");

            builder.Property(e => e.inspectionform_id)
                .HasColumnName("inspectionform_id");

            //builder.HasOne(d => d.InspectionForms)
            //    .WithMany(p => p.Asset)
            //    .HasForeignKey(d => d.InspectionForms.inspection_form_id)
            //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    .HasConstraintName("site_asset_fk");

            builder.Property(e => e.created_at)
                .HasColumnName("created_at")
                .HasColumnType("datetime");

            builder.Property(e => e.modified_at)
                .HasColumnName("modified_at")
                .HasColumnType("datetime");

            builder.Property(e => e.notes)
                .HasColumnName("notes")
                .HasColumnType("character varying");

            builder.Property(e => e.asset_request_status)
                .HasColumnName("asset_request_status")
                .HasColumnType("SMALLINT");

            builder.Property(e => e.asset_requested_by)
                .HasColumnName("asset_requested_by")
                .HasColumnType("character varying");

            builder.Property(e => e.asset_requested_on)
               .HasColumnName("asset_requested_on")
               .HasColumnType("datetime");

            builder.Property(e => e.asset_approved_by)
               .HasColumnName("asset_approved_by")
               .HasColumnType("character varying");

            builder.Property(e => e.asset_approved_on)
              .HasColumnName("asset_approved_on")
              .HasColumnType("datetime");

            builder.Property(e => e.lastinspection_attribute_values)
              .HasColumnName("lastinspection_attribute_values")
              .HasColumnType("jsonb");

            builder.Property(e => e.usage)
              .HasColumnName("usage")
              .HasColumnType("BIGINT");

            builder.Property(e => e.name)
               .HasColumnName("name")
               .HasColumnType("character varying");

            builder.Property(e => e.asset_type)
              .HasColumnName("asset_type")
              .HasColumnType("character varying");

            builder.Property(e => e.product_name)
              .HasColumnName("product_name")
              .HasColumnType("character varying");

            builder.Property(e => e.model_name)
              .HasColumnName("model_name")
              .HasColumnType("character varying");

            builder.Property(e => e.asset_serial_number)
              .HasColumnName("asset_serial_number")
              .HasColumnType("character varying");

            builder.Property(e => e.model_year)
              .HasColumnName("model_year")
              .HasColumnType("character varying");

            builder.Property(e => e.site_location)
              .HasColumnName("site_location")
              .HasColumnType("character varying");

            builder.Property(e => e.current_stage)
              .HasColumnName("current_stage")
              .HasColumnType("character varying");

            builder.Property(e => e.parent)
              .HasColumnName("parent")
              .HasColumnType("character varying");

            builder.Property(e => e.children)
              .HasColumnName("children")
              .HasColumnType("character varying");

            builder.HasOne(e => e.StatusMaster)
              .WithMany(e => e.Asset)
              .HasForeignKey(e => e.StatusMaster.status_id)
              .OnDelete(DeleteBehavior.ClientSetNull)
              .HasConstraintName("status_asset_fk");
        }
    }
}
