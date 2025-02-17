using Jarvis.db.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.db.EntityConfigurations
{
    public class InspectionFormAttributesConfigurations : IEntityTypeConfiguration<InspectionFormAttributes>
    {
        public void Configure(EntityTypeBuilder<InspectionFormAttributes> builder)
        {
            builder.HasKey(e => e.attributes_id)
                    .HasName("attribute_pk");

            builder.Property(e => e.attributes_id)
                .HasColumnName("attributes_id")
                .ValueGeneratedOnAdd();

            builder.Property(e => e.name)
                .IsRequired()
                .HasColumnName("name")
                .HasColumnType("Character varying");

            builder.Property(e => e.company_id)
                .HasColumnName("company_id")
                .HasColumnType("Character varying");

            builder.Property(e => e.values_type)
                .HasColumnName("values_type")
                .HasColumnType("SMALLINT");

            builder.Property(e => e.site_id)
                .HasColumnName("site_id")
                .HasColumnType("Character varying");

            builder.Property(e => e.value_parameters)
                .HasColumnName("value_parameters")
                .HasColumnType("jsonb");

            builder.Property(e => e.category_id)
                .HasColumnName("category_id");

            builder.HasOne(d => d.InspectionAttributeCategory)
                .WithMany(p => p.InspectionFormAttributes)
                .HasForeignKey(d => d.InspectionAttributeCategory.category_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("category_inspection_form_attribute_fk");
        }
    }
}
