namespace McpWebClient.Models;

/// <summary>
/// Represents a single message in the chat conversation.
/// </summary>
public record Message(
    string Role,    // "user" or "assistant"
    string Content
);
