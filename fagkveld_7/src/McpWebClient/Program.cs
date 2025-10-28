using McpWebClient.Models;
using McpWebClient.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure Azure OpenAI settings from appsettings.json
builder.Services.Configure<AzureOpenAISettings>(
    builder.Configuration.GetSection("AzureOpenAI"));

// Register MCP services
// McpClientService is both a singleton and a hosted service
// This ensures it starts when the app starts and can be injected into other services
builder.Services.AddSingleton<McpClientService>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<McpClientService>());

// Register application services
builder.Services.AddSingleton<AzureOpenAIService>();
builder.Services.AddSingleton<ToolSchemaConverter>();
builder.Services.AddSingleton<ChatService>();

// Add CORS for local development
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Enable developer exception page for better error messages
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors();
app.UseDefaultFiles();
app.UseStaticFiles();

// === API Endpoints ===

// Test endpoint to verify API is working
app.MapGet("/api/test", () => new
{
    status = "ok",
    message = "API is working!"
});

// Shows available MCP tools
app.MapGet("/api/mcp/tools", (McpClientService mcpClient) =>
{
    var tools = mcpClient.GetCachedTools();
    return Results.Ok(new { tools });
});

// Main chat endpoint - processes messages and returns AI responses
app.MapPost("/api/chat", async (ChatRequest request, ChatService chatService) =>
{
    try
    {
        var response = await chatService.ProcessChatAsync(request);
        return Results.Ok(response);
    }
    catch (Exception ex)
    {
        return Results.Problem(
            detail: ex.Message,
            statusCode: 500,
            title: "Error processing chat request"
        );
    }
});

app.Run();
