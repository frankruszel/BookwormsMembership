using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookwormsMembership.Migrations
{
    public partial class PasswordHistoryv4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PasswordHistory",
                table: "PasswordHistory",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHistory",
                table: "PasswordHistory");
        }
    }
}
