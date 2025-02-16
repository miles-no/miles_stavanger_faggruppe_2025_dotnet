var builder = DistributedApplication.CreateBuilder(args);

var backend = builder.AddProject<Projects.SignalRWorkshop_Backend>("backend");

builder.AddNpmApp("react", "../SignalRWorkshop.Frontend")
    .WithReference(backend)
    .WaitFor(backend)
    .WithEnvironment("BROWSER", "none") // Disable opening browser on npm start
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();
