var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Template_net10_API>("template-net10-api");

builder.Build().Run();
