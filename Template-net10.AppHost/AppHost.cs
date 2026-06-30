var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Template_net10>("template-net10");

builder.Build().Run();
