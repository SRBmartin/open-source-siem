using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Siem.Platform.Logging.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLogTagEntityInitial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Logging");

            migrationBuilder.CreateTable(
                name: "LogTags",
                schema: "Logging",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Topic = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Partitions = table.Column<int>(type: "integer", nullable: false),
                    RetentionMs = table.Column<long>(type: "bigint", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogTags", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LogTags_Name",
                schema: "Logging",
                table: "LogTags",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LogTags_Topic",
                schema: "Logging",
                table: "LogTags",
                column: "Topic",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LogTags",
                schema: "Logging");
        }
    }
}
