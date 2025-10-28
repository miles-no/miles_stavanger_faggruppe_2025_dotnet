namespace McpWebClient.Models;

/// <summary>
/// Represents the response from a chat request.
/// </summary>
public record ChatResponse(
    string Message,
    string FinishReason
);
