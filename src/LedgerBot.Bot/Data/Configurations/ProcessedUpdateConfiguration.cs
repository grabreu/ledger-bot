using LedgerBot.Bot.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LedgerBot.Bot.Data.Configurations;

public sealed class ProcessedUpdateConfiguration : IEntityTypeConfiguration<ProcessedUpdate>
{
    public void Configure(EntityTypeBuilder<ProcessedUpdate> builder)
    {
        builder.HasKey(x => x.UpdateId);

        builder.Property(x => x.UpdateId)
            .ValueGeneratedNever();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(16);
    }
}
