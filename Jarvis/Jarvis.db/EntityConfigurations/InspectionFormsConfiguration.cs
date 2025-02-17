using Jarvis.db.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.db.EntityConfigurations
{
    public class InspectionFormsConfiguration : IEntityTypeConfiguration<InspectionForms>
    {
        public void Configure(EntityTypeBuilder<InspectionForms> builder)
        {
            builder.HasKey(e => e.inspection_form_id)
                .HasName("inspection_form_pk");

            builder.Property(e => e.inspection_form_id)
               .HasColumnName("inspection_form_id")
               .ValueGeneratedOnAdd();

            builder.Property(e => e.name)
               .HasColumnName("name")
               .HasColumnType("Character varying");

            builder.Property(e => e.company_id)
               .HasColumnName("company_id")
               .HasColumnType("Character varying");

            builder.HasOne(d => d.Company)
                .WithMany(p=>p.InspectionForms)
                .HasForeignKey(d => d.Company.company_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("company_inspectionForm_fk");

            //builder.HasOne(d => d.InspectionFormTypes)
            //    .WithMany(p => p.InspectionForms)
            //    .HasForeignKey(d => d.InspectionFormTypes.inspection_form_type_id)
            //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    .HasConstraintName("inspectionForm_inspectionType_fk");


            //builder.Property(e => e.inspection_form_type_id).HasColumnName("inspection_form_type_id");

            builder.Property(e => e.created_at)
                .HasColumnName("created_at")
                .HasColumnType("datetime");

            builder.Property(e => e.created_by)
                .HasColumnName("created_by")
                .HasColumnType("Character varying");

            builder.Property(e => e.modified_at)
                .HasColumnName("modified_at")
                .HasColumnType("datetime");

            builder.Property(e => e.modified_by)
                .HasColumnName("modified_by")
                .HasColumnType("Character varying");

            builder.Property(e => e.status)
                .HasColumnName("status")
                .HasColumnType("SMALLINT");

            builder.Property(e => e.form_attributes)
                .HasColumnName("form_attributes")
                .HasColumnType("jsonb");


            builder.HasOne(e => e.StatusMaster)
                   .WithMany(e => e.InspectionForms)
                   .HasForeignKey(e => e.StatusMaster.status_id)
                   .OnDelete(DeleteBehavior.ClientSetNull)
                   .HasConstraintName("status_inspectionforms_fk");
        }
    }
}
