using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Options;
using OpenAI.Chat;
using McpWebClient.Models;
using System.ClientModel;

namespace McpWebClient.Services;

/// <summary>
/// Service for interacting with Azure OpenAI.
/// Handles authentication, client creation, and chat completions.
/// </summary>
public class AzureOpenAIService
{
    private readonly AzureOpenAISettings _settings;
    private readonly ILogger<AzureOpenAIService> _logger;

    public AzureOpenAIService(
        IOptions<AzureOpenAISettings> settings,
        ILogger<AzureOpenAIService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    /// <summary>
    /// Creates a chat client for the configured Azure OpenAI deployment.
    /// Allows overriding the API key (useful for multi-tenant scenarios).
    /// </summary>
    /// <param name="apiKeyOverride">Optional API key to use instead of the configured one</param>
    /// <returns>A ChatClient ready to use</returns>
    public ChatClient CreateChatClient(string? apiKeyOverride = null)
    {
        var apiKey = apiKeyOverride ?? _settings.ApiKey;

        if (string.IsNullOrEmpty(apiKey))
        {
            throw new InvalidOperationException(
                "Azure OpenAI API key is not configured. " +
                "Please set it in appsettings.Development.json or as an environment variable.");
        }

        var endpoint = new Uri(_settings.Endpoint);
        var credential = new AzureKeyCredential(apiKey);
        var azureClient = new AzureOpenAIClient(endpoint, credential);

        _logger.LogDebug("Created Azure OpenAI client for deployment: {DeploymentName}",
            _settings.DeploymentName);

        return azureClient.GetChatClient(_settings.DeploymentName);
    }

    /// <summary>
    /// Sends a chat completion request to Azure OpenAI.
    /// </summary>
    /// <param name="chatClient">The chat client to use</param>
    /// <param name="messages">The conversation history</param>
    /// <param name="tools">Available tools the AI can use</param>
    /// <returns>The chat completion response</returns>
    public async Task<ClientResult<ChatCompletion>> GetChatCompletionAsync(
        ChatClient chatClient,
        List<ChatMessage> messages,
        List<ChatTool> tools)
    {
        var chatOptions = new ChatCompletionOptions();
        foreach (var tool in tools)
        {
            chatOptions.Tools.Add(tool);
        }

        _logger.LogInformation("Requesting chat completion with {ToolCount} tools available",
            tools.Count);

        return await chatClient.CompleteChatAsync(messages, chatOptions);
    }
}
