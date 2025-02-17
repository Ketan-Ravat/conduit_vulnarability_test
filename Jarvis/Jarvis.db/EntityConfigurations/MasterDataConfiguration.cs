using Jarvis.db.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.db.EntityConfigurations
{
    public class MasterDataConfiguration
    {
        public class CompanyConfiguration : IEntityTypeConfiguration<MasterData>
        {
            public void Configure(EntityTypeBuilder<MasterData> builder)
            {
                builder.HasKey(e => e.master_data_id)
                    .HasName("newtable_pk");

                builder.Property(e => e.master_data_id)
                    .HasColumnName("master_data_id")
                    .ValueGeneratedOnAdd();

                builder.Property(e => e.meter_hours)
                    .IsRequired()
                    .HasColumnName("meter_hours")
                    .HasColumnType("character varying");              
            }
        }
    }
}
