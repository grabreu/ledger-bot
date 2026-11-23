using Azure.AI.OpenAI;
using Azure.Identity;
using LedgerBot.Llm.Contracts;
using LedgerBot.Llm.Tools;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace LedgerBot.Llm;

public static class LlmModuleExtensions
{
    private const string TelemetrySourceName = "LedgerBot.Llm";
    private const int MaximumToolIterations = 5;

    public static IHostApplicationBuilder AddLlmModuleServices(this IHostApplicationBuilder builder)
    {
        builder.Services.TryAddSingleton(TimeProvider.System);

        builder.Services.AddOptions<LlmOptions>()
            .BindConfiguration(LlmOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services
            .AddChatClient(sp =>
            {
                var options = sp.GetRequiredService<IOptions<LlmOptions>>().Value;

                return new AzureOpenAIClient(
                        new Uri(options.Endpoint),
                        new DefaultAzureCredential())
                    .GetChatClient(options.Deployment)
                    .AsIChatClient();
            })
            .UseFunctionInvocation(configure: client => client.MaximumIterationsPerRequest = MaximumToolIterations)
            .UseOpenTelemetry(sourceName: TelemetrySourceName, configure: client => client.EnableSensitiveData = false);

        builder.Services.AddScoped<ILlmToolSet, LedgerToolSet>();

        builder.Services.AddScoped<ILlmService, LlmService>();

        return builder;
    }
}
