using LedgerBot.Llm.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace LedgerBot.Llm;

public static class LlmModuleExtensions
{
    private const string TelemetrySourceName = "LedgerBot.Llm";
    private const int MaximumToolIterations = 5;

    public static IHostApplicationBuilder AddLlmModuleServices(this IHostApplicationBuilder builder)
    {
        builder.Services.TryAddSingleton(TimeProvider.System);

        //builder.Services.AddOptions<LlmOptions>()
        //    .BindConfiguration(LlmOptions.SectionName)
        //    .ValidateDataAnnotations()
        //    .ValidateOnStart();

        //builder.Services
        //    .AddChatClient(sp =>
        //    {
        //        var options = sp.GetRequiredService<IOptions<LlmOptions>>().Value;

        //        return new ChatClient(
        //                options.Deployment,
        //                new ApiKeyCredential(options.ApiKey),
        //                new OpenAIClientOptions { Endpoint = new Uri(options.Endpoint) })
        //            .AsIChatClient();
        //    })
        //    .UseFunctionInvocation(configure: client => client.MaximumIterationsPerRequest = MaximumToolIterations)
        //    .UseOpenTelemetry(sourceName: TelemetrySourceName, configure: client => client.EnableSensitiveData = false);

        builder.Services.AddScoped<ILlmToolSet, CoreToolSet>();
        builder.Services.AddScoped<ILlmService, FakeLlmService>();

        return builder;
    }
}
