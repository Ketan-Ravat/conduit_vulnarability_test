using Jarvis.db.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.db.EntityConfigurations
{
    public class RecordSyncInformationConfiguration : IEntityTypeConfiguration<RecordSyncInformation>
    {
        public void Configure(EntityTypeBuilder<RecordSyncInformation> builder)
        {
            builder.HasKey(e => e.sync_id)
                .HasName("asset_pk");

            builder.Property(e => e.sync_id)
                .HasColumnName("sync_id")
                .ValueGeneratedOnAdd();

            builder.Property(e => e.request_id)
                .HasColumnName("request_id");

            builder.Property(e => e.request_model)
                .HasColumnName("request_model");

            builder.Property(e => e.device_uuid)
                .HasColumnName("device_uuid");

            builder.Property(e => e.device_battery_percentage)
                .HasColumnName("device_battery_percentage");

            builder.Property(e => e.mac_address)
                .HasColumnName("mac_address");

            builder.Property(e => e.device_latitude)
                .HasColumnName("device_latitude");

            builder.Property(e => e.device_longitude)
                .HasColumnName("device_longitude");

            builder.Property(e => e.is_inspection)
                .HasColumnName("is_inspection");

            builder.Property(e => e.is_workorder)
                .HasColumnName("is_workorder");

            builder.HasOne(d => d.DeviceInfo)
                .WithMany(p => p.RecordSyncInformation)
                .HasForeignKey(d => d.DeviceInfo.device_uuid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("deviceinfo_recordsyncinformation_fk");

        }
    }
}
