using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookwormsMembership.Migrations
{
    public partial class AuditLOgginv4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateTime",
                table: "Audits",
                newName: "Time");

            migrationBuilder.AddColumn<string>(
                name: "PrimaryKey",
                table: "Audits",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "changedColumns",
                table: "Audits",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "newVal",
                table: "Audits",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "oldVal",
                table: "Audits",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "tableName",
                table: "Audits",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrimaryKey",
                table: "Audits");

            migrationBuilder.DropColumn(
                name: "changedColumns",
                table: "Audits");

            migrationBuilder.DropColumn(
                name: "newVal",
                table: "Audits");

            migrationBuilder.DropColumn(
                name: "oldVal",
                table: "Audits");

            migrationBuilder.DropColumn(
                name: "tableName",
                table: "Audits");

            migrationBuilder.RenameColumn(
                name: "Time",
                table: "Audits",
                newName: "DateTime");
        }
    }
}
