using LedgerBot.Ledger.Domain;

namespace LedgerBot.Ledger.Data.Configurations;

public sealed class LedgerEntryConfiguration : IEntityTypeConfiguration<LedgerEntry>
{
    public void Configure(EntityTypeBuilder<LedgerEntry> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Kind)
            .HasConversion<string>()
            .HasMaxLength(16);

        builder.Property(x => x.Amount)
            .HasPrecision(18, 2);

        builder.HasIndex(x => new { x.HouseholdId, x.OccurredOn });
    }
}
