using Jarvis.db.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.db.EntityConfigurations
{
    public class DeviceInfoConfiguration : IEntityTypeConfiguration<DeviceInfo>
    {
        public void Configure(EntityTypeBuilder<DeviceInfo> builder)
        {
            
            builder.HasKey(e => e.device_info_id)
                .HasName("device_info_id");

            builder.Property(e => e.device_info_id)
                .HasColumnName("device_info_id")
                .ValueGeneratedOnAdd();

            builder.Property(e => e.device_uuid)
                .IsRequired()
                .HasColumnName("device_uuid");

            builder.Property(e => e.device_code)
                .IsRequired()
                .HasColumnName("device_code");

            builder.Property(e => e.name)
                .HasColumnName("name");

            builder.Property(e => e.type)
                .HasColumnName("type");

            builder.Property(e => e.brand)
                .HasColumnName("brand");

            builder.Property(e => e.model)
                .HasColumnName("model");

            builder.Property(e => e.version)
                .HasColumnName("version");

            builder.Property(e => e.os)
                .HasColumnName("os");

            builder.Property(e => e.mac_address)
                .HasColumnName("mac_address");

            builder.Property(e => e.created_at)
                .HasColumnName("created_at")
                .HasColumnType("datetime");

            builder.Property(e => e.modified_at)
                .HasColumnName("modified_at")
                .HasColumnType("datetime");

            builder.HasIndex(e => e.device_uuid).IsUnique();

            builder.HasIndex(e => e.device_code).IsUnique();

        }
    }
}
