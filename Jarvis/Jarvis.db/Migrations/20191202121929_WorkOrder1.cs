using System;
using Jarvis.db.Models;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Jarvis.db.Migrations
{
    public partial class WorkOrder1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<string>(
            //    name: "notification_token",
            //    table: "User",
            //    nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "status_type_id",
                table: "StatusTypes",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "status_type_id",
                table: "StatusMasters",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "status_id",
                table: "StatusMasters",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            //migrationBuilder.CreateTable(
            //    name: "WorkOrder",
            //    columns: table => new
            //    {
            //        work_order_uuid = table.Column<Guid>(nullable: false),
            //        asset_id = table.Column<Guid>(nullable: false),
            //        inspection_id = table.Column<Guid>(nullable: false),
            //        name = table.Column<string>(nullable: true),
            //        description = table.Column<string>(nullable: true),
            //        notes = table.Column<string>(nullable: true),
            //        attributes_value = table.Column<AssetsValueJsonObject[]>(type: "jsonb", nullable: true),
            //        status = table.Column<int>(nullable: false),
            //        comments = table.Column<CommentJsonObject[]>(type: "jsonb", nullable: true),
            //        priority = table.Column<int>(nullable: false),
            //        created_by = table.Column<string>(nullable: true),
            //        createdAt = table.Column<DateTime>(nullable: true),
            //        modified_by = table.Column<string>(nullable: true),
            //        modifiedAt = table.Column<DateTime>(nullable: true),
            //        maintainence_staff_id = table.Column<string>(nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_WorkOrder", x => x.work_order_uuid);
            //        table.ForeignKey(
            //            name: "FK_WorkOrder_StatusMasters_status",
            //            column: x => x.status,
            //            principalTable: "StatusMasters",
            //            principalColumn: "status_id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "WorkOrderStatus",
            //    columns: table => new
            //    {
            //        work_order_status_id = table.Column<Guid>(nullable: false),
            //        work_order_id = table.Column<Guid>(nullable: false),
            //        status = table.Column<int>(nullable: false),
            //        modified_by = table.Column<string>(nullable: true),
            //        modified_at = table.Column<DateTime>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_WorkOrderStatus", x => x.work_order_status_id);
            //        table.ForeignKey(
            //            name: "FK_WorkOrderStatus_WorkOrder_work_order_id",
            //            column: x => x.work_order_id,
            //            principalTable: "WorkOrder",
            //            principalColumn: "work_order_uuid",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "IX_WorkOrder_status",
            //    table: "WorkOrder",
            //    column: "status");

            //migrationBuilder.CreateIndex(
            //    name: "IX_WorkOrderStatus_work_order_id",
            //    table: "WorkOrderStatus",
            //    column: "work_order_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkOrderStatus");

            migrationBuilder.DropTable(
                name: "WorkOrder");

            migrationBuilder.DropColumn(
                name: "notification_token",
                table: "User");

            migrationBuilder.AlterColumn<long>(
                name: "status_type_id",
                table: "StatusTypes",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<long>(
                name: "status_type_id",
                table: "StatusMasters",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<long>(
                name: "status_id",
                table: "StatusMasters",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
        }
    }
}
