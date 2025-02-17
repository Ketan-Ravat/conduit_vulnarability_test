using Jarvis.db.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.db.EntityConfigurations
{
    public class UserSitesConfiguration : IEntityTypeConfiguration<UserSites>
    {
        public void Configure(EntityTypeBuilder<UserSites> builder)
        {
            builder.HasKey(e => e.usersite_id)
                .HasName("newtable_pk");

            builder.Property(e => e.usersite_id)
                .HasColumnName("usersite_id")
                .ValueGeneratedOnAdd();

            builder.Property(e => e.user_id).HasColumnName("user_id");

            //builder.Property(e => e.roleid)
            //    .IsRequired()
            //    .HasColumnName("roleid")
            //    .HasColumnType("character varying");

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

            builder.Property(e => e.company_id).HasColumnName("company_id");

            builder.Property(e => e.site_id).HasColumnName("site_id");

            builder.HasOne(d => d.User)
                .WithMany(p => p.Usersites)
                .HasForeignKey(d => d.User.uuid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_usersite_fk");

            builder.HasOne(d => d.Sites)
                .WithMany(p => p.UserSites)
                .HasForeignKey(d => d.Sites.site_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("site_usersite_fk");

            builder.HasOne(e => e.StatusMaster)
                   .WithMany(e => e.UserSites)
                   .HasForeignKey(e => e.StatusMaster.status_id)
                   .OnDelete(DeleteBehavior.ClientSetNull)
                   .HasConstraintName("status_usersites_fk");
        }
    }
}
