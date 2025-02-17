using Jarvis.db.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.db.EntityConfigurations
{
    public class StatusMasterConfiguration : IEntityTypeConfiguration<StatusMaster>
    {
        public void Configure(EntityTypeBuilder<StatusMaster> builder)
        {
            builder.HasKey(e => e.status_id)
                .HasName("statusType_pk");

            builder.Property(e => e.status_id)
                .HasColumnName("status_id")
                .ValueGeneratedOnAdd();

            builder.Property(e => e.status_name)
                .IsRequired()
                .HasColumnName("status_name")
                .HasColumnType("Character varying");

            builder.HasOne(e => e.StatusTypes)
                .WithMany(d => d.Status)
                .HasForeignKey(e => e.StatusTypes.status_type_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("status_statusmaster_fk");
        }
    }
}
