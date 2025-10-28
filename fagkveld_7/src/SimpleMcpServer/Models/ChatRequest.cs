namespace SimpleMcpServer.Models;

/// <summary>
/// Represents an incoming chat request from the web client.
/// </summary>
public record ChatRequest(
    List<Message> Messages,
    string? ApiKey = null
);
