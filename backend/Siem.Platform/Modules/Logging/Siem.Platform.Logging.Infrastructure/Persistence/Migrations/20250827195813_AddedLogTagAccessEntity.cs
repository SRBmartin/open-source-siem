using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Siem.Platform.Logging.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedLogTagAccessEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LogTagAccess",
                schema: "Logging",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogTagAccess", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LogTagAccess_TagId_UserId",
                schema: "Logging",
                table: "LogTagAccess",
                columns: new[] { "TagId", "UserId" },
                unique: true,
                filter: "\"IsDeleted\" = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LogTagAccess",
                schema: "Logging");
        }
    }
}
