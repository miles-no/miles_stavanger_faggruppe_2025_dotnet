using ToDo.Business.Services;
using ToDo.Business.Services.Interfaces;
using ToDo.Data;
using ToDo.Data.Repositories;
using ToDo.Data.Repositories.Interfaces;
using ToDo.ServiceDefaults;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.AddSqlServerDbContext<ApplicationDbContext>(connectionName: "todo-db");

builder.Services.AddScoped<ITodoService, TodoService>();
builder.Services.AddScoped<ITodoRepository, TodoRepository>();

builder.Services.AddScoped<IUserStatsService, UserStatsService>();
builder.Services.AddScoped<IUserStatsRepository, UserStatsRepository>();

builder.Services.AddControllers();

// Add Swagger services
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ToDo API", Version = "v1" });
});

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Enable middleware to serve generated Swagger as a JSON endpoint.
app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ToDo API v1");
    c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
});

app.Run();