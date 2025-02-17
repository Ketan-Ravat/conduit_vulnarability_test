using Jarvis.db.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.db.EntityConfigurations
{
    public class IssueRecordConfigurations : IEntityTypeConfiguration<IssueRecord>
    {
        public void Configure(EntityTypeBuilder<IssueRecord> builder)
        {

            builder.HasKey(e => e.issue_record_uuid)
                .HasName("issue_record_uuid");

            builder.Property(e => e.issue_record_uuid)
                    .HasColumnName("issue_record_uuid")
                    .ValueGeneratedOnAdd();

            builder.Property(e => e.issue_uuid)
                    .HasColumnName("issue_uuid");

            builder.Property(e => e.created_at)
                    .HasColumnName("created_at");
            
            builder.Property(e => e.checkout_datetime)
                    .HasColumnName("checkout_datetime");

            builder.Property(e => e.fixed_datetime)
                    .HasColumnName("fixed_datetime");

            builder.Property(e => e.requested_datetime)
                    .HasColumnName("requested_datetime");

            builder.HasOne(e => e.StatusMaster)
                        .WithMany(e => e.IssueRecord)
                        .HasForeignKey(e => e.StatusMaster.status_id)
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("status_issue_record_fk");

            builder.HasOne(e => e.Asset)
                    .WithMany(e => e.IssueRecords)
                    .HasForeignKey(e => e.Asset.asset_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("issue_record_asset_fk");

            builder.HasOne(e => e.Inspection)
                    .WithMany(e => e.IssueRecords)
                    .HasForeignKey(e => e.Inspection.inspection_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("issue_record_inspection_fk");

            builder.HasOne(e => e.Attributes)
                    .WithMany(e => e.IssueRecord)
                    .HasForeignKey(e => e.Attributes.attributes_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("issue_record_attributes_fk");

        }
    }
}
