using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookwormsMembership.Migrations
{
    public partial class AuditLOgginv6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrimaryKey",
                table: "Audits");

            migrationBuilder.DropColumn(
                name: "UserId",
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

            migrationBuilder.RenameColumn(
                name: "tableName",
                table: "Audits",
                newName: "UserName");

            migrationBuilder.AddColumn<bool>(
                name: "isSuccessful",
                table: "Audits",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isSuccessful",
                table: "Audits");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "Audits",
                newName: "tableName");

            migrationBuilder.AddColumn<string>(
                name: "PrimaryKey",
                table: "Audits",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
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
        }
    }
}
