using ToDo.Data;
using ToDo.Migrations.Workers;
using ToDo.ServiceDefaults;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.AddSqlServerDbContext<ApplicationDbContext>(connectionName: "todo-db");

builder.Services.AddHostedService<MigrationWorker>();

var host = builder.Build();
host.Run();
