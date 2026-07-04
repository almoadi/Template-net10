using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Template_net10.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddHijriDateColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedAtHijri",
                table: "UserSessions",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UpdatedAtHijri",
                table: "UserSessions",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedAtHijri",
                table: "Users",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UpdatedAtHijri",
                table: "Users",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedAtHijri",
                table: "Roles",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UpdatedAtHijri",
                table: "Roles",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedAtHijri",
                table: "Permissions",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UpdatedAtHijri",
                table: "Permissions",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAtHijri",
                table: "UserSessions");

            migrationBuilder.DropColumn(
                name: "UpdatedAtHijri",
                table: "UserSessions");

            migrationBuilder.DropColumn(
                name: "CreatedAtHijri",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UpdatedAtHijri",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedAtHijri",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "UpdatedAtHijri",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "CreatedAtHijri",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "UpdatedAtHijri",
                table: "Permissions");
        }
    }
}
