using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookwormsMembership.Migrations
{
    public partial class PasswordHistoryv6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PasswordExpired",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordExpired",
                table: "AspNetUsers");
        }
    }
}
