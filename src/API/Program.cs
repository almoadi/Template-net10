using Template_net10.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddSplitConfiguration();

builder.AddServiceDefaults();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAppLocalization(builder.Configuration);
builder.Services.AddClientCors(builder.Configuration, builder.Environment);
builder.Services.AddApiSwagger();
builder.Services.AddAuthRateLimiting();
builder.Services.AddApiAuthentication(builder.Configuration);
builder.Services.AddRealtime();
builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

app.UseFacades();

await app.Services.MigrateAndSeedAsync();

app.UseApiPipeline();

app.Run();
