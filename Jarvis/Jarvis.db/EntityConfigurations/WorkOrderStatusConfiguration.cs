using Jarvis.db.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.db.EntityConfigurations
{
    public class IssueStatusConfiguration : IEntityTypeConfiguration<IssueStatus>
    {
        public void Configure(EntityTypeBuilder<IssueStatus> builder)
        {
            builder.HasKey(e => e.issue_status_id)
                .HasName("issue_status_id");

            builder.Property(e => e.issue_status_id)
                .HasColumnName("issue_status_id")
                .ValueGeneratedOnAdd();

            builder.Property(e => e.issue_id)
                .IsRequired()
                .HasColumnName("issue_id");

            builder.Property(e => e.status)
                .HasColumnName("status");

            builder.Property(e => e.modified_by)
                .HasColumnName("modified_by");

            builder.Property(e => e.modified_at)
                .HasColumnName("modified_at");

            builder.HasOne(d => d.Issue)
               .WithMany(p => p.IssueStatus)
               .HasForeignKey(d => d.Issue.issue_uuid)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("issue_issue_status_fk");
        }
    }
}