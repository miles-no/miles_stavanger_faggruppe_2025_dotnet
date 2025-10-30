using McpWebClient.Models;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace McpWebClient.Services;

/// <summary>
/// Handles JSON-RPC communication with the MCP server.
/// Implements the MCP (Model Context Protocol) client-side protocol.
/// </summary>
public class McpClient
{
    private readonly McpProcessManager _processManager;
    private readonly ILogger<McpClient> _logger;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private int _requestId = 0;
    private List<McpTool>? _cachedTools;

    public McpClient(
        McpProcessManager processManager,
        ILogger<McpClient> logger)
    {
        _processManager = processManager;
        _logger = logger;
    }

    /// <summary>
    /// Initializes the MCP connection by sending the initialize handshake.
    /// This must be called before any other MCP operations.
    /// </summary>
    public async Task InitializeAsync()
    {
        _logger.LogInformation("Initializing MCP connection...");

        // Step 1: Send initialize request
        // This tells the server about our client capabilities and protocol version
        var request = new
        {
            jsonrpc = "2.0",
            id = ++_requestId,
            method = "initialize",
            @params = new
            {
                protocolVersion = "2024-11-05",
                capabilities = new { },
                clientInfo = new
                {
                    name = "AspNetMcpClient",
                    version = "1.0.0"
                }
            }
        };

        var response = await SendRequestAsync(request);

        if (response == null || !response.RootElement.TryGetProperty("result", out _))
        {
            throw new Exception("Failed to initialize MCP connection - no result in response");
        }

        // Step 2: Send initialized notification
        // This completes the handshake and tells the server we're ready
        var notification = new
        {
            jsonrpc = "2.0",
            method = "notifications/initialized"
        };

        await SendNotificationAsync(notification);

        _logger.LogInformation("MCP connection initialized successfully");
    }

    /// <summary>
    /// Lists all available tools from the MCP server.
    /// Results are cached after the first call.
    /// </summary>
    public async Task<List<McpTool>> ListToolsAsync()
    {
        // Return cached tools if available
        if (_cachedTools != null)
        {
            return _cachedTools;
        }

        _logger.LogInformation("Fetching available tools from MCP server...");

        var request = new
        {
            jsonrpc = "2.0",
            id = ++_requestId,
            method = "tools/list",
            @params = new { }
        };

        var response = await SendRequestAsync(request);

        if (response == null)
        {
            throw new Exception("Failed to list tools - no response from server");
        }

        var tools = new List<McpTool>();

        // Parse the tools from the response
        if (response.RootElement.TryGetProperty("result", out var result) &&
            result.TryGetProperty("tools", out var toolsArray))
        {
            foreach (var tool in toolsArray.EnumerateArray())
            {
                var name = tool.GetProperty("name").GetString() ?? "";
                var description = tool.TryGetProperty("description", out var desc)
                    ? desc.GetString()
                    : "";
                var inputSchema = tool.TryGetProperty("inputSchema", out var schema)
                    ? schema.GetRawText()
                    : "{}";

                tools.Add(new McpTool
                {
                    Name = name,
                    Description = description ?? "",
                    InputSchema = inputSchema
                });
            }
        }

        // Cache the tools for future requests
        _cachedTools = tools;
        _logger.LogInformation("Found {ToolCount} tools from MCP server", tools.Count);

        return tools;
    }

    /// <summary>
    /// Calls a tool on the MCP server with the given arguments.
    /// </summary>
    /// <param name="toolName">The name of the tool to call</param>
    /// <param name="arguments">The arguments to pass to the tool (can be null for parameterless tools)</param>
    /// <returns>The text result from the tool</returns>
    public async Task<string> CallToolAsync(string toolName, JsonNode? arguments = null)
    {
        _logger.LogDebug("Calling MCP tool: {ToolName}", toolName);

        // Prepare arguments (use empty object if null)
        object argumentsObj = arguments != null ? arguments : new { };

        var request = new
        {
            jsonrpc = "2.0",
            id = ++_requestId,
            method = "tools/call",
            @params = new
            {
                name = toolName,
                arguments = argumentsObj
            }
        };

        var response = await SendRequestAsync(request);

        if (response == null)
        {
            throw new Exception($"Failed to call tool '{toolName}' - no response from server");
        }

        // Check for errors in the response
        if (response.RootElement.TryGetProperty("error", out var error))
        {
            var errorMessage = error.TryGetProperty("message", out var msg)
                ? msg.GetString()
                : "Unknown error";
            throw new Exception($"MCP tool '{toolName}' error: {errorMessage}");
        }

        // Extract the result text from the response
        // MCP tools return content as an array with text objects
        if (response.RootElement.TryGetProperty("result", out var result))
        {
            if (result.TryGetProperty("content", out var content))
            {
                var contentArray = content.EnumerateArray().ToList();
                if (contentArray.Any())
                {
                    var firstContent = contentArray.First();
                    if (firstContent.TryGetProperty("text", out var text))
                    {
                        var resultText = text.GetString() ?? "";
                        _logger.LogDebug("Tool '{ToolName}' returned: {Result}", toolName, resultText);
                        return resultText;
                    }
                }
            }
        }

        _logger.LogWarning("Tool '{ToolName}' returned empty result", toolName);
        return "";
    }

    /// <summary>
    /// Gets the cached list of available tools.
    /// </summary>
    public List<McpTool> GetCachedTools()
    {
        return _cachedTools ?? new List<McpTool>();
    }

    /// <summary>
    /// Sends a JSON-RPC request to the MCP server and waits for a response.
    /// Thread-safe using a semaphore to prevent concurrent requests.
    /// </summary>
    private async Task<JsonDocument?> SendRequestAsync(object request)
    {
        // Acquire lock to ensure only one request is sent at a time
        // This is necessary because we're reading/writing to stdin/stdout sequentially
        await _lock.WaitAsync();
        try
        {
            var stdin = _processManager.StandardInput;
            var stdout = _processManager.StandardOutput;

            if (stdin == null || stdout == null)
            {
                throw new Exception("MCP connection not established - streams are null");
            }

            // Serialize and send the request
            var json = JsonSerializer.Serialize(request);
            _logger.LogTrace("Sending MCP request: {Json}", json);

            await stdin.WriteLineAsync(json);
            await stdin.FlushAsync();

            // Read the response
            var responseLine = await stdout.ReadLineAsync();

            if (string.IsNullOrEmpty(responseLine))
            {
                _logger.LogWarning("Received empty response from MCP server");
                return null;
            }

            _logger.LogTrace("Received MCP response: {Response}", responseLine);

            return JsonDocument.Parse(responseLine);
        }
        finally
        {
            _lock.Release();
        }
    }

    /// <summary>
    /// Sends a JSON-RPC notification to the MCP server.
    /// Notifications don't expect a response.
    /// </summary>
    private async Task SendNotificationAsync(object notification)
    {
        var stdin = _processManager.StandardInput;

        if (stdin == null)
        {
            throw new Exception("MCP connection not established - stdin is null");
        }

        var json = JsonSerializer.Serialize(notification);
        _logger.LogTrace("Sending MCP notification: {Json}", json);

        await stdin.WriteLineAsync(json);
        await stdin.FlushAsync();
    }

    public void Dispose()
    {
        _lock?.Dispose();
    }
}
