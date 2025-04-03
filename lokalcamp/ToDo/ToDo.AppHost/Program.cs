var builder = DistributedApplication.CreateBuilder(args);

var db= builder.AddSqlServer("sql", port: 14329)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithEndpoint(name: "sqlEndpoint", targetPort: 14330)
    .AddDatabase("todo-db");

builder.AddProject<Projects.ToDo_Api>("todo-api")
    .WithReference(db)
    .WaitFor(db);

builder.AddProject<Projects.ToDo_Migrations>("todo-migrations")
    .WithReference(db)
    .WaitFor(db);

builder.Build().Run();
