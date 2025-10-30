namespace McpWebClient.Models;

/// <summary>
/// Configuration settings for Azure OpenAI connection.
/// These are loaded from appsettings.json.
/// </summary>
public class AzureOpenAISettings
{
    /// <summary>
    /// The Azure OpenAI endpoint URL
    /// </summary>
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// The deployment name (model) to use (e.g., "gpt-4.1")
    /// </summary>
    public string DeploymentName { get; set; } = string.Empty;

    /// <summary>
    /// The API key for authentication.
    /// Should be set in appsettings.Development.json or environment variables.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;
}
