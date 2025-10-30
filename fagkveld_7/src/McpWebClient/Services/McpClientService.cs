using McpWebClient.Models;
using System.Text.Json.Nodes;

namespace McpWebClient.Services;

/// <summary>
/// Hosted service that manages the MCP (Model Context Protocol) connection.
/// This combines process management and protocol communication for easy integration.
/// </summary>
public class McpClientService : IHostedService, IDisposable
{
    private readonly ILogger<McpClientService> _logger;
    private readonly McpProcessManager _processManager;
    private readonly McpClient _client;

    public McpClientService(
        ILogger<McpClientService> logger,
        ILoggerFactory loggerFactory)
    {
        _logger = logger;

        // Create the process manager and client
        _processManager = new McpProcessManager(
            loggerFactory.CreateLogger<McpProcessManager>());

        _client = new McpClient(
            _processManager,
            loggerFactory.CreateLogger<McpClient>());
    }

    /// <summary>
    /// Starts the MCP server when the application starts.
    /// This is called automatically by ASP.NET Core's hosting infrastructure.
    /// </summary>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting MCP Server...");

        try
        {
            // Step 1: Start the MCP server process
            await _processManager.StartAsync(cancellationToken);

            // Step 2: Initialize the MCP protocol connection
            await _client.InitializeAsync();

            // Step 3: Discover available tools (this caches them for later use)
            var tools = await _client.ListToolsAsync();

            _logger.LogInformation("MCP Server started successfully with {Count} tools", tools.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start MCP Server");
            throw;
        }
    }

    /// <summary>
    /// Stops the MCP server when the application shuts down.
    /// This is called automatically by ASP.NET Core's hosting infrastructure.
    /// </summary>
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping MCP Server...");
        await _processManager.StopAsync(cancellationToken);
    }

    /// <summary>
    /// Calls a tool on the MCP server.
    /// </summary>
    public async Task<string> CallToolAsync(string toolName, JsonNode? arguments = null)
    {
        return await _client.CallToolAsync(toolName, arguments);
    }

    /// <summary>
    /// Gets the list of available tools (cached after first call).
    /// </summary>
    public List<McpTool> GetCachedTools()
    {
        return _client.GetCachedTools();
    }

    public void Dispose()
    {
        _client?.Dispose();
        _processManager?.Dispose();
    }
}
