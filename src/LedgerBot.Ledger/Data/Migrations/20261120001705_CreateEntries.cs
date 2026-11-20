using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LedgerBot.Ledger.Data.Migrations;

/// <inheritdoc />
public partial class _20261120001705_CreateEntries : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "Ledger");

        migrationBuilder.CreateTable(
            name: "Entries",
            schema: "Ledger",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ChatId = table.Column<long>(type: "bigint", nullable: false),
                Kind = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                OccurredOn = table.Column<DateOnly>(type: "date", nullable: false),
                Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Entries", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Entries_ChatId_OccurredOn",
            schema: "Ledger",
            table: "Entries",
            columns: new[] { "ChatId", "OccurredOn" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Entries",
            schema: "Ledger");
    }
}
