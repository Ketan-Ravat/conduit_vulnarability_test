using Jarvis.db.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.db.EntityConfigurations
{
    public class InspectionFormTypesConfiguration : IEntityTypeConfiguration<InspectionFormTypes>
    {
        public void Configure(EntityTypeBuilder<InspectionFormTypes> builder)
        {
            builder.HasKey(e => e.inspection_form_type_id)
                .HasName("inspection_form_type_pk");

            builder.Property(e => e.inspection_form_type_id)
               .HasColumnName("inspection_form_type_id")
               .ValueGeneratedOnAdd();

            builder.Property(e => e.name)
                .IsRequired()
                .HasColumnName("name")
                .HasColumnType("character varying");

            builder.Property(e => e.company_id)
                .HasColumnName("company_id")
                .HasColumnType("character varying");

            builder.Property(e => e.site_id)
                .HasColumnName("site_id")
                .HasColumnType("character varying");

            builder.Property(e => e.status)
                            .HasColumnName("status")
                            .HasColumnType("SMALLINT");

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

            builder.HasOne(d => d.Company)
               .WithMany(p => p.InspectionFormTypes)
               .HasForeignKey(d => d.Company.company_id)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("company_inspectionFormType_fk");

        }
    }
}
