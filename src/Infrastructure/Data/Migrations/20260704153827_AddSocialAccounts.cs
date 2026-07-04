using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Template_net10.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSocialAccounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Phone",
                table: "Users");

            migrationBuilder.CreateTable(
                name: "SocialAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Provider = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProviderUserId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    AvatarUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtHijri = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    UpdatedAtHijri = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SocialAccounts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Phone",
                table: "Users",
                column: "Phone",
                unique: true,
                filter: "[DeletedAt] IS NULL AND [Phone] <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_SocialAccounts_Provider_ProviderUserId",
                table: "SocialAccounts",
                columns: new[] { "Provider", "ProviderUserId" },
                unique: true,
                filter: "[DeletedAt] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SocialAccounts_UserId",
                table: "SocialAccounts",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SocialAccounts");

            migrationBuilder.DropIndex(
                name: "IX_Users_Phone",
                table: "Users");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Phone",
                table: "Users",
                column: "Phone",
                unique: true,
                filter: "[DeletedAt] IS NULL");
        }
    }
}
