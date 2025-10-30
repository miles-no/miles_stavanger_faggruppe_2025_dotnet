namespace McpWebClient.Models;

/// <summary>
/// Represents a tool available from the MCP server.
/// These tools can be called by the AI assistant to perform actions.
/// </summary>
public class McpTool
{
    /// <summary>
    /// The unique name of the tool (e.g., "get_current_time")
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Description explaining what the tool does
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// JSON schema defining the tool's input parameters
    /// </summary>
    public required string InputSchema { get; set; }
}
