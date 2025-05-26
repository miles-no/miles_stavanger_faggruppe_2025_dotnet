var builder = DistributedApplication.CreateBuilder(args);

var backend = builder.AddProject<Projects.GrafanaWorkshop_Backend>("backend");

builder.AddNpmApp("react", "../GrafanaWorkshop.Frontend")
    .WithReference(backend)
    .WaitFor(backend)
    .WithEnvironment("BROWSER", "none") // Disable opening browser on npm start
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();
