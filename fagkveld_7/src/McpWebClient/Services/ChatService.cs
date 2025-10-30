using OpenAI.Chat;
using McpWebClient.Models;
using System.Text.Json.Nodes;

namespace McpWebClient.Services;

/// <summary>
/// Main service that orchestrates the chat flow.
/// Coordinates between Azure OpenAI, MCP tools, and the conversation logic.
/// </summary>
public class ChatService
{
    private readonly AzureOpenAIService _openAIService;
    private readonly McpClientService _mcpClient;
    private readonly ToolSchemaConverter _toolConverter;
    private readonly ILogger<ChatService> _logger;

    public ChatService(
        AzureOpenAIService openAIService,
        McpClientService mcpClient,
        ToolSchemaConverter toolConverter,
        ILogger<ChatService> logger)
    {
        _openAIService = openAIService;
        _mcpClient = mcpClient;
        _toolConverter = toolConverter;
        _logger = logger;
    }

    /// <summary>
    /// Processes a chat request from the user.
    /// This method handles the full conversation flow:
    /// 1. Get available MCP tools
    /// 2. Send conversation to Azure OpenAI
    /// 3. If AI wants to use tools, execute them via MCP
    /// 4. Get final response with tool results
    /// </summary>
    /// <param name="request">The chat request from the web client</param>
    /// <returns>The assistant's response</returns>
    public async Task<ChatResponse> ProcessChatAsync(ChatRequest request)
    {
        _logger.LogInformation("Processing chat request with {MessageCount} messages",
            request.Messages.Count);

        // Step 1: Create Azure OpenAI client
        var chatClient = _openAIService.CreateChatClient(request.ApiKey);

        // Step 2: Get available tools from MCP server and convert to Azure format
        var mcpTools = _mcpClient.GetCachedTools();
        var azureTools = _toolConverter.ConvertToAzureOpenAITools(mcpTools);

        _logger.LogDebug("Found {ToolCount} tools from MCP server", mcpTools.Count);

        // Step 3: Build conversation history
        var messages = BuildConversationHistory(request.Messages);

        // Step 4: Get initial response from Azure OpenAI
        var response = await _openAIService.GetChatCompletionAsync(
            chatClient,
            messages,
            azureTools);

        // Step 5: Check if the AI wants to call any tools
        if (response.Value.FinishReason == ChatFinishReason.ToolCalls)
        {
            _logger.LogInformation("AI requested {ToolCallCount} tool calls",
                response.Value.ToolCalls.Count);

            // Add the assistant's response (with tool calls) to conversation
            messages.Add(new AssistantChatMessage(response.Value));

            // Execute each tool call via MCP
            foreach (var toolCall in response.Value.ToolCalls)
            {
                var toolName = toolCall.FunctionName;
                var toolArgsString = toolCall.FunctionArguments.ToString();

                // Parse tool arguments (may be null for tools with no parameters)
                var toolArguments = string.IsNullOrEmpty(toolArgsString)
                    ? null
                    : JsonNode.Parse(toolArgsString);

                _logger.LogDebug("Calling MCP tool: {ToolName}", toolName);

                // Call the tool via MCP and get the result
                var toolResult = await _mcpClient.CallToolAsync(toolName, toolArguments);

                // Add tool result to conversation
                messages.Add(new ToolChatMessage(toolCall.Id, toolResult));
            }

            // Step 6: Get final response with tool results incorporated
            response = await _openAIService.GetChatCompletionAsync(
                chatClient,
                messages,
                azureTools);
        }

        // Return the final response
        return new ChatResponse(
            response.Value.Content[0].Text,
            response.Value.FinishReason.ToString()
        );
    }

    /// <summary>
    /// Builds the conversation history in Azure OpenAI format.
    /// Adds a system message that instructs the AI about its capabilities.
    /// </summary>
    private List<ChatMessage> BuildConversationHistory(List<Message> requestMessages)
    {
        var messages = new List<ChatMessage>();

        // Add system message explaining the AI's role and capabilities
        messages.Add(new SystemChatMessage(
            "You are a helpful assistant. You have access to tools that can help you answer questions."));

        // Convert user's message history to Azure OpenAI format
        foreach (var msg in requestMessages)
        {
            if (msg.Role == "user")
                messages.Add(new UserChatMessage(msg.Content));
            else if (msg.Role == "assistant")
                messages.Add(new AssistantChatMessage(msg.Content));
        }

        return messages;
    }
}
