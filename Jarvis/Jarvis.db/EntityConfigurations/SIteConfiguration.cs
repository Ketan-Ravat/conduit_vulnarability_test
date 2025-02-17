using Jarvis.db.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.db.EntityConfigurations
{
    public class SIteConfiguration : IEntityTypeConfiguration<Sites>
    {
        public void Configure(EntityTypeBuilder<Sites> builder)
        {
            builder.HasKey(e => e.site_id)
                .HasName("newtable_pk");

            builder.Property(e => e.site_id)
                .HasColumnName("site_id")
                .ValueGeneratedOnAdd();

            builder.Property(e => e.company_id)
                .HasColumnName("company_id");

            builder.Property(e => e.site_name)
                .IsRequired()
                .HasColumnName("site_name")
                .HasColumnType("character varying");

            builder.Property(e => e.site_code)
                .IsRequired()
                .HasColumnName("site_name")
                .HasColumnType("character varying");

            //builder.Property(e => e.location)
            //    .IsRequired()
            //    .HasColumnName("location")
            //    .HasColumnType("character varying");

            builder.HasIndex(u => u.location)
                   .IsUnique();

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

            builder.HasOne(d => d.Company)
                .WithMany(p => p.Sites)
                .HasForeignKey(d => d.Company.company_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("site_company_fk");

            builder.HasOne(e => e.StatusMaster)
                   .WithMany(e => e.Sites)
                   .HasForeignKey(e => e.StatusMaster.status_id)
                   .OnDelete(DeleteBehavior.ClientSetNull)
                   .HasConstraintName("status_site_fk");


        }
    }
}
