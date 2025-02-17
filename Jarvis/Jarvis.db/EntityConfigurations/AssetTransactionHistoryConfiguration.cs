using Jarvis.db.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.db.EntityConfigurations
{
    public class AssetTransactionHistoryConfiguration : IEntityTypeConfiguration<AssetTransactionHistory>
    {
        public void Configure(EntityTypeBuilder<AssetTransactionHistory> builder)
        {
            builder.HasKey(e => e.asseet_txn_id)
                .HasName("asseet_txn_id");

            builder.Property(e => e.asseet_txn_id)
                .HasColumnName("asseet_txn_id")
                .ValueGeneratedOnAdd();

            builder.Property(e => e.asset_id)
                .HasColumnName("asset_id");
            
            builder.HasOne(e=>e.Asset)
                .WithMany(e=>e.AssetTransactionHistory)
                .HasForeignKey(e=>e.Asset.asset_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("asset_asset_history_fk");

            builder.Property(e => e.inspection_id)
                .HasColumnName("inspection_id");

            builder.Property(e => e.operator_id)
                .HasColumnName("operator_id");

            builder.Property(e => e.manager_id)
                .HasColumnName("manager_id");

            builder.Property(e => e.status)
                .HasColumnName("manager_id");

            builder.Property(e => e.comapny_id)
                .HasColumnName("comapny_id");

            builder.Property(e => e.site_id)
                .HasColumnName("site_id");

            builder.Property(e => e.attributeValues)
                .HasColumnName("attributeValues")
                .HasColumnType("jsonb");

            builder.Property(e => e.inspection_form_id)
                .HasColumnName("inspection_form_id");

            builder.Property(e => e.meter_hours)
                .HasColumnName("meter_hours");

            builder.Property(e => e.shift)
                .HasColumnName("shift");

            builder.Property(e => e.created_at)
                .HasColumnName("created_at");

        }

    }
}
