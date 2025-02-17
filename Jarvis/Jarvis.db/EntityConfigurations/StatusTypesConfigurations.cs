using Jarvis.db.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.db.EntityConfigurations
{
    public class StatusTypesConfigurations : IEntityTypeConfiguration<StatusTypes>
    {
        public void Configure(EntityTypeBuilder<StatusTypes> builder)
        {
            builder.HasKey(e => e.status_type_id)
                .HasName("statusType_pk");

            builder.Property(e => e.status_type_id)
                .HasColumnName("status_type_id")
                .ValueGeneratedOnAdd();

            builder.Property(e => e.status_type_name)
                .IsRequired()
                .HasColumnName("status_type_name")
                .HasColumnType("Character varying");

        }
    }
}
