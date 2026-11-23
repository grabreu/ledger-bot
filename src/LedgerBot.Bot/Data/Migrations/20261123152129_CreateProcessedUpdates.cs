using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LedgerBot.Bot.Data.Migrations;

/// <inheritdoc />
public partial class _20261123152129_CreateProcessedUpdates : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "Bot");

        migrationBuilder.CreateTable(
            name: "ProcessedUpdates",
            schema: "Bot",
            columns: table => new
            {
                UpdateId = table.Column<long>(type: "bigint", nullable: false),
                Status = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                ReceivedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ProcessedUpdates", x => x.UpdateId);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "ProcessedUpdates",
            schema: "Bot");
    }
}
