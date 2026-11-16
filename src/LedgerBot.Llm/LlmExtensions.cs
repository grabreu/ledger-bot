using LedgerBot.Llm.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace LedgerBot.Llm;

public static class LlmExtensions
{
    public static IServiceCollection AddLlm(this IServiceCollection services)
    {
        services.AddSingleton<IEntryInterpreter, FakeEntryInterpreter>();

        return services;
    }
}
