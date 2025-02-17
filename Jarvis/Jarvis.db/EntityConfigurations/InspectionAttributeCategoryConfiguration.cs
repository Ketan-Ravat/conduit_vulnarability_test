using Jarvis.db.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.db.EntityConfigurations
{
    public class InspectionAttributeCategoryConfiguration : IEntityTypeConfiguration<InspectionAttributeCategory>
    {
        public void Configure(EntityTypeBuilder<InspectionAttributeCategory> builder)
        {
            builder.HasKey(e => e.category_id)
                .HasName("category_pk");

            builder.Property(e => e.category_id)
                .HasColumnName("category_id")
                .ValueGeneratedOnAdd();

            builder.Property(e => e.name)
                .IsRequired()
                .HasColumnName("name")
                .HasColumnType("character varying");
        }
    }
}
