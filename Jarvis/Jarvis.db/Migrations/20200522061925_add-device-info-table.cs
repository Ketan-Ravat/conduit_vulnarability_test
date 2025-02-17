using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Jarvis.db.Migrations
{
    public partial class adddeviceinfotable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeviceInfo",
                columns: table => new
                {
                    device_info_id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    device_uuid = table.Column<Guid>(nullable: false),
                    device_code = table.Column<string>(nullable: false),
                    name = table.Column<string>(nullable: true),
                    type = table.Column<string>(nullable: true),
                    brand = table.Column<string>(nullable: true),
                    model = table.Column<string>(nullable: true),
                    version = table.Column<string>(nullable: true),
                    os = table.Column<string>(nullable: true),
                    mac_address = table.Column<string>(nullable: true),
                    is_approved = table.Column<bool>(nullable: false),
                    approved_by = table.Column<Guid>(nullable: true),
                    created_at = table.Column<DateTime>(nullable: true,defaultValueSql: ("now() at time zone 'utc'")),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceInfo", x => x.device_info_id);
                    table.UniqueConstraint("device_code", x => x.device_code);
                    table.UniqueConstraint("device_uuid", x => x.device_uuid);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceInfo");
        }
    }
}
