using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using System.ComponentModel;

// Build and configure the MCP server host
var builder = Host.CreateApplicationBuilder(args);

// Configure logging to output to stderr (required for MCP)
builder.Logging.AddConsole(consoleLogOptions =>
{
    consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
});

// Add MCP server with stdio transport and auto-discover tools
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

// Run the server
await builder.Build().RunAsync();

/// <summary>
/// Simple MCP tool that provides current date and time information
/// </summary>
[McpServerToolType]
public static class DateTimeTool
{
    [McpServerTool]
    [Description("Gets the current date and time in multiple formats (standard, ISO 8601, and Unix timestamp)")]
    public static string GetCurrentDateTime()
    {
        var now = DateTime.Now;
        return $"Current date and time: {now:yyyy-MM-dd HH:mm:ss}\n" +
               $"ISO 8601 format: {now:O}\n" +
               $"Unix timestamp: {new DateTimeOffset(now).ToUnixTimeSeconds()}";
    }
}
