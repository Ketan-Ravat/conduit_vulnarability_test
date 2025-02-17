using Jarvis.db.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.db.EntityConfigurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.Property(e => e.role_id)
                   .HasColumnName("role_id")
                   .ValueGeneratedNever();

            builder.Property(e => e.name)
                .IsRequired()
                .HasColumnName("name")
                .HasColumnType("character varying");
        }
    }
}
