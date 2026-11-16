using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LedgerBot.Ledger.Data.Migrations;

/// <inheritdoc />
public partial class AddLedgerEntrySchema : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "Ledger");

        migrationBuilder.CreateTable(
            name: "LedgerEntries",
            schema: "Ledger",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                HouseholdId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Kind = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                OccurredOn = table.Column<DateOnly>(type: "date", nullable: false),
                Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_LedgerEntries", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_LedgerEntries_HouseholdId_OccurredOn",
            schema: "Ledger",
            table: "LedgerEntries",
            columns: ["HouseholdId", "OccurredOn"]);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "LedgerEntries",
            schema: "Ledger");
    }
}
