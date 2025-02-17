using Jarvis.db.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.db.EntityConfigurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(e => e.uuid)
                .HasName("newtable_pk");

            builder.Property(e => e.uuid)
                .HasColumnName("uuid")
                .ValueGeneratedOnAdd();

            builder.Property(e => e.barcode_id)
                .IsRequired()
                .HasColumnName("barcode_id");

            builder.Property(e => e.email)
                .IsRequired()
                .HasColumnName("email")
                .HasColumnType("character varying");

            builder.Property(e => e.username)
                .IsRequired()
                .HasColumnName("username")
                .HasColumnType("character varying");

            builder.Property(e => e.firstname)
               .IsRequired()
               .HasColumnName("firstname")
               .HasColumnType("character varying");

            builder.Property(e => e.lastname)
               .IsRequired()
               .HasColumnName("lastname")
               .HasColumnType("character varying");

            builder.Property(e => e.password)
                .IsRequired()
                .HasColumnName("password")
                .HasColumnType("Character varying");

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

            builder.Property(e => e.os)
                .HasColumnName("os")
                .HasColumnType("Character varying");

            builder.HasOne(e => e.StatusMaster)
                   .WithMany(e => e.User)
                   .HasForeignKey(e => e.StatusMaster.status_id)
                   .OnDelete(DeleteBehavior.ClientSetNull)
                   .HasConstraintName("status_user_fk");
        }
    }
}
