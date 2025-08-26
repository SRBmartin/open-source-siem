using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Siem.Platform.User.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddActivatedAtUserEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ActivatedAt",
                schema: "User",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActivatedAt",
                schema: "User",
                table: "Users");
        }
    }
}
