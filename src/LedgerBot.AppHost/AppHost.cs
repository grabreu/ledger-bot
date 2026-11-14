var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.LedgerBot_Bot>("bot");

var app = builder.Build();

await app.RunAsync();
