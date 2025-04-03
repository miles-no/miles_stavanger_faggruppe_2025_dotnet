using ToDo.Business.Services;
using ToDo.Business.Services.Interfaces;
using ToDo.Data;
using ToDo.Data.Repositories;
using ToDo.Data.Repositories.Interfaces;
using ToDo.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.AddSqlServerDbContext<ApplicationDbContext>(connectionName: "todo-db");

builder.Services.AddScoped<ITodoService, TodoService>();
builder.Services.AddScoped<ITodoRepository, TodoRepository>();

builder.Services.AddControllers();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
