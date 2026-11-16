using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LedgerBot.Ledger.Data.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // This migration is intentionally empty and serves as the initial database baseline.
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // This migration is intentionally empty because there is nothing to revert from the initial baseline.
    }
}
