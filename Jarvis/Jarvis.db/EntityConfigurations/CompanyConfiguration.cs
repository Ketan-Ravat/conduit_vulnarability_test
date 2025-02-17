using Jarvis.db.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.db.EntityConfigurations
{
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.HasKey(e => e.company_id)
                .HasName("newtable_pk");

            builder.Property(e => e.company_id)
                .HasColumnName("company_id")
                .ValueGeneratedOnAdd();

            builder.Property(e => e.company_name)
                .IsRequired()
                .HasColumnName("company_name")
                .HasColumnType("character varying");

            builder.Property(e => e.company_code)
                .IsRequired()
                .HasColumnName("company_code")
                .HasColumnType("character varying");

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

            builder.HasOne(e => e.StatusMaster)
            .WithMany(e => e.Company)
            .HasForeignKey(e => e.StatusMaster.status_id)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("status_company_fk");
            //builder.Property(e => e.roleid).HasColumnName("roleid");

            //builder.HasOne(d => d.Role)
            //    .WithMany(p => p.User)
            //    .HasForeignKey(d => d.Role.roleid)
            //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    .HasConstraintName("user_role_fk");
        }
    }
}
