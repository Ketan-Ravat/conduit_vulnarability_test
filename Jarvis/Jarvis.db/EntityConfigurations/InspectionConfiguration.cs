using Jarvis.db.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.db.EntityConfigurations
{
    public class InspectionConfiguration : IEntityTypeConfiguration<Inspection>
    {
        public void Configure(EntityTypeBuilder<Inspection> builder)
        {
            builder.HasKey(e => e.inspection_id)
                .HasName("inspection_pk");

            builder.Property(e => e.inspection_id)
                .HasColumnName("inspection_id")
                .ValueGeneratedOnAdd();

            builder.Property(e => e.asset_id)
                .HasColumnName("asset_id");

            builder.HasOne(x => x.Asset)
                .WithMany(x => x.Inspection)
                .HasForeignKey(x => x.Asset.asset_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("asset_inspection_fk");

            builder.Property(e => e.status)
               .HasColumnName("status")
               .HasColumnType("SMALLINT");

            builder.Property(e => e.created_at)
                .HasColumnName("created_at")
                .HasColumnType("datetime");

            builder.Property(e => e.modified_at)
                .HasColumnName("modified_at")
                .HasColumnType("datetime");

            builder.Property(e => e.created_by)
              .HasColumnName("created_by")
              .HasColumnType("character varying");

            builder.Property(e => e.modified_by)
              .HasColumnName("modified_by")
              .HasColumnType("character varying");

            builder.Property(e => e.operator_id).HasColumnName("operator_id");

            builder.HasOne(x => x.User)
                .WithMany(x => x.Inspection)
                .HasForeignKey(x => x.User.uuid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_inspection_operator_fk");

            builder.Property(e => e.manager_id)
              .HasColumnName("manager_id")
              .HasColumnType("character varying");

            builder.Property(e => e.operator_notes)
              .HasColumnName("operator_notes")
              .HasColumnType("character varying");

            builder.Property(e => e.attribute_values)
              .HasColumnName("attribute_values")
              .HasColumnType("jsonb");

            builder.Property(e => e.company_id)
             .HasColumnName("company_id")
             .HasColumnType("character varying");

            builder.Property(e => e.site_id)
             .HasColumnName("site_id");

            builder.HasOne(x => x.Sites)
                .WithMany(x => x.Inspection)
                .HasForeignKey(x => x.Sites.site_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("site_inspection_fk");

            builder.Property(e => e.meter_hours)
             .HasColumnName("meter_hours")
             .HasColumnType("BIGINT");

            builder.Property(e => e.shift)
             .HasColumnName("shift")
             .HasColumnType("SMALLINT");

            builder.Property(e => e.image_list)
              .HasColumnName("image_list")
              .HasColumnType("jsonb");

            builder.Property(e => e.manager_notes)
             .HasColumnName("manager_notes")
             .HasColumnType("character varying");

            builder.Property(e => e.datetime_requested)
             .HasColumnName("datetime_requested")
             .HasColumnType("datetime");

            builder.HasOne(e => e.StatusMaster)
                .WithMany(e => e.Inspection)
                .HasForeignKey(e => e.StatusMaster.status_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("status_inspection_fk");
        }
    }
}
