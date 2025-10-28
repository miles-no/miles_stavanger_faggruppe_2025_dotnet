using OpenAI.Chat;
using SimpleMcpServer.Models;

namespace SimpleMcpServer.Services;

/// <summary>
/// Converts MCP tool schemas to Azure OpenAI ChatTool format.
/// This is needed because MCP and Azure OpenAI use different formats for tool definitions.
/// </summary>
public class ToolSchemaConverter
{
    /// <summary>
    /// Converts a list of MCP tools to Azure OpenAI ChatTool format.
    /// </summary>
    /// <param name="mcpTools">The tools from the MCP server</param>
    /// <returns>A list of ChatTool objects that Azure OpenAI can understand</returns>
    public List<ChatTool> ConvertToAzureOpenAITools(List<McpTool> mcpTools)
    {
        var tools = new List<ChatTool>();

        foreach (var mcpTool in mcpTools)
        {
            // Convert MCP tool schema to Azure OpenAI format
            // If no input schema is provided, use an empty object schema
            var functionParameters = string.IsNullOrEmpty(mcpTool.InputSchema)
                ? BinaryData.FromString("{\"type\":\"object\",\"properties\":{},\"required\":[]}")
                : BinaryData.FromString(mcpTool.InputSchema);

            tools.Add(ChatTool.CreateFunctionTool(
                functionName: mcpTool.Name,
                functionDescription: mcpTool.Description,
                functionParameters: functionParameters
            ));
        }

        return tools;
    }
}
