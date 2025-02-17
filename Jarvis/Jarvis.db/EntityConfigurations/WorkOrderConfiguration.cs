using Jarvis.db.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.db.EntityConfigurations
{
    public class IssueConfiguration : IEntityTypeConfiguration<Issue>
    {
        public void Configure(EntityTypeBuilder<Issue> builder)
        {
            builder.HasKey(e => e.issue_uuid)
                .HasName("issue_uuid");

            builder.Property(e => e.issue_uuid)
                .HasColumnName("issue_uuid")
                .ValueGeneratedOnAdd();

            builder.Property(e => e.asset_id)
                .HasColumnName("asset_id");

            //builder.HasOne(e => e.Asset)
            //    .WithMany(e => e.WorkOrders)
            //    .HasForeignKey(e => e.Asset.asset_id)
            //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    .HasConstraintName("asset_work_order_fk");

            builder.Property(e => e.name)
                .HasColumnName("name");

            builder.Property(e => e.attributes_value)
                .HasColumnName("attributes_value")
                .HasColumnType("jsonb");

            builder.Property(e => e.comments)
                .HasColumnName("comments")
                .HasColumnType("jsonb");

            builder.Property(e => e.created_at)
                .HasColumnName("createdAt");

            builder.Property(e => e.created_by)
                .HasColumnName("created_by");

            builder.Property(e => e.description)
                .HasColumnName("description");

            builder.Property(e => e.modified_at)
                .HasColumnName("modifiedAt");

            builder.Property(e => e.requested_datetime)
                .HasColumnName("requested_datetime");

            builder.Property(e => e.checkout_datetime)
                .HasColumnName("checkout_datetime");

            builder.Property(e => e.modified_by)
                .HasColumnName("modified_by");

            builder.Property(e => e.notes)
                .HasColumnName("notes");

            builder.Property(e => e.priority)
                .HasColumnName("priority");

            builder.Property(e => e.status)
                .HasColumnName("status");

            builder.HasOne(e => e.StatusMaster)
               .WithMany(e => e.Issue)
               .HasForeignKey(e => e.StatusMaster.status_id)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("status_issue_fk");

            builder.HasOne(e => e.Sites)
               .WithMany(e => e.Issue)
               .HasForeignKey(e => e.Sites.site_id)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("site_issue_fk");


            builder.HasOne(e => e.Inspection)
               .WithMany(e => e.Issues)
               .HasForeignKey(e => e.Inspection.inspection_id)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("inspection_issue_fk");


            builder.HasOne(e => e.Asset)
                .WithMany(e => e.Issues)
                .HasForeignKey(e => e.Asset.asset_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("asset_issue_fk");


            builder.HasOne(e => e.Attributes)
                .WithMany(e => e.Issue)
                .HasForeignKey(e => e.Attributes.attributes_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("attributes_issue_fk");

        }
    }
}