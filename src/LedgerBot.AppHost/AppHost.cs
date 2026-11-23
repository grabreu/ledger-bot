var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

var db = sql.AddDatabase("ledgerbot");

builder.AddDotnetProject("api", "../LedgerBot.Api/LedgerBot.Api.csproj")
    .WithReference(db)
    .WaitFor(db);

var app = builder.Build();

await app.RunAsync();
