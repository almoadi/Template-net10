using Template_net10.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Laravel-style split configuration: each concern lives in its own config/*.json file,
// with an optional per-environment override in config/{Environment}/*.json (e.g. config/Production/mail.json).
foreach (var file in new[] { "app", "database", "cache", "mail", "jwt", "queue", "logging", "cors", "storage", "features", "encryption", "idempotency" })
{
    builder.Configuration
        .AddJsonFile($"config/{file}.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"config/{builder.Environment.EnvironmentName}/{file}.json", optional: true, reloadOnChange: true);
}

builder.AddServiceDefaults();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAppLocalization(builder.Configuration);
builder.Services.AddClientCors(builder.Configuration, builder.Environment);
builder.Services.AddApiSwagger();
builder.Services.AddAuthRateLimiting();
builder.Services.AddApiAuthentication(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

app.UseFacades();

await app.Services.MigrateAndSeedAsync();

app.UseApiPipeline();

app.Run();
