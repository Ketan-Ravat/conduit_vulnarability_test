using Jarvis.db.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.db.EntityConfigurations
{
    public class DashboardOutstandingIssuesConfiguration : IEntityTypeConfiguration<DashboardOutstandingIssues>
    {
        public void Configure(EntityTypeBuilder<DashboardOutstandingIssues> builder)
        {

            builder.Property(e => e.site_id)
                .HasColumnName("site_id");

            builder.Property(e => e.data)
                .HasColumnName("data")
                .HasColumnType("jsonb");

            builder.Property(e => e.created_at)
                .HasColumnName("created_at");

            builder.Property(e => e.modified_at)
                .HasColumnName("modified_at");
        }
    }
}
