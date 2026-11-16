using LedgerBot.Llm.Contracts;

namespace LedgerBot.Llm;

public static class LlmModuleExtensions
{
    public static IHostApplicationBuilder AddLlmModuleServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<ILlmProvider, FakeLlmProvider>();

        return builder;
    }
}
