using Jarvis.db.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.db.EntityConfigurations
{
    public class NotificationDataConfiguration : IEntityTypeConfiguration<NotificationData>
    {

        public void Configure(EntityTypeBuilder<NotificationData> builder)
        {
            builder.HasKey(e => e.notification_id)
                .HasName("newtable_pk");

            builder.Property(e => e.notification_id)
                .HasColumnName("notification_id")
                .ValueGeneratedOnAdd();

            builder.Property(e => e.user_id)
                .IsRequired()
                .HasColumnName("user_id");

            builder.Property(e => e.target_type)
                .HasColumnName("target_type");

            builder.Property(e => e.device_key)
                .IsRequired()
                .HasColumnName("target_type");

            builder.Property(e => e.ref_id)
                 .IsRequired()
                .HasColumnName("ref_id");

            builder.Property(e => e.heading)
                .HasColumnName("heading");

            builder.Property(e => e.message)
                .HasColumnName("message");

            builder.Property(e => e.data)
                .HasColumnName("data");

            builder.Property(e => e.notification_type)
                .HasColumnName("notification_type");

            builder.Property(e => e.createdDate)
                .HasColumnName("createdDate")
                .HasColumnType("datetime");

            builder.Property(e => e.sendDate)
                .HasColumnName("sendDate")
                .HasColumnType("datetime");

            builder.Property(e => e.status)
                .HasColumnName("status");

            builder.Property(e => e.OS)
                .HasColumnName("OS");
        }
    }
}
