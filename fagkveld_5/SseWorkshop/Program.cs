using SseWorkshop.Services;
using SseWorkshop.Workers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<NasaCloseApproachService>();
builder.Services.AddHostedService<NasaCloseApproachServiceWorker>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseDefaultFiles().UseStaticFiles();

app.MapGet("/objects", (NasaCloseApproachService objects, CancellationToken cancellationToken) =>
    // TODO: Implement server sent events
);

app.Run();
