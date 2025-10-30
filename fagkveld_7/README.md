# MCP Chat Solution

Web-based chat application with Azure AI Foundry and Model Context Protocol (MCP) server.

## Prerequisites

- .NET 9 SDK
- Azure AI Foundry API key

## Setup

1. **Configure API key** in `src/McpWebClient/appsettings.Development.json`:
```json
{
  "AzureOpenAI": {
    "Endpoint": "https://your-endpoint.openai.azure.com/",
    "DeploymentName": "gpt-4.1",
    "ApiKey": "YOUR_API_KEY_HERE"
  }
}
```

2. **Build and run**:
```bash
dotnet build McpChatSolution.sln
cd src/McpWebClient
ASPNETCORE_ENVIRONMENT=Development dotnet run
```

3. **Open browser** to http://localhost:5000

## Ideas to Try with MCP

1. **Spotify Integration** - Control music playback on your phone, create playlists based on mood, or get song recommendations
2. **Meme Generator** - Generate and post memes to Slack/Teams based on conversation context
3. **Office Jokes & Facts** - Random tech jokes, programming facts, or team trivia on demand
4. **GitHub Profile Stats** - Visualize contribution graphs, most used languages, and repo statistics
5. **Coffee Break Scheduler** - Coordinate team coffee breaks based on everyone's calendar availability

6. **File System Navigator** - Search files, read content, and get directory summaries in natural language
7. **Calculator & Unit Converter** - Complex calculations, currency conversion, timezone math
8. **Environment Info Tool** - Check system info, environment variables, running processes
9. **JSON/XML Formatter** - Validate, format, and query structured data
10. **Local Git Helper** - Check branch status, view recent commits, show file diffs

## Troubleshooting

- **"API key is not configured"**: Check `appsettings.Development.json` has valid API key
- **Build errors**: Run `dotnet restore && dotnet build`
- **MCP server not starting**: Ensure solution is built first
