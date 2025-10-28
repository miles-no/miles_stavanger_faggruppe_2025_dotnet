# MCP Chat Solution

A complete web-based chat application demonstrating the Model Context Protocol (MCP) with Azure AI Foundry. This solution includes both a web interface and an MCP server, showing how AI assistants can interact with external tools.

## What is MCP?

Model Context Protocol (MCP) is an open protocol that enables AI assistants to connect to external tools and data sources. This project demonstrates:
- **Web-based chat UI** for interacting with Azure OpenAI
- **MCP Server** providing tools the AI can call
- **Clean architecture** designed for educational purposes

## 🚀 Quick Start

```bash
# Build the solution
dotnet build McpChatSolution.sln

# Run the application
cd src/McpWebClient
ASPNETCORE_ENVIRONMENT=Development dotnet run

# Open browser to http://localhost:5000
```

## Prerequisites

- .NET 9 SDK
- Azure AI Foundry API key
- Web browser

## 📁 Project Structure

```
fagkveld_7/
├── McpChatSolution.sln          # Solution file
├── README.md                     # This file
└── src/
    ├── McpWebClient/             # Web application (chat interface)
    │   ├── Program.cs            # Application entry point (78 lines)
    │   ├── appsettings.json      # Base configuration
    │   ├── appsettings.Development.json  # Development settings with API key
    │   ├── McpWebClient.csproj
    │   ├── Models/               # Data models
    │   │   ├── AzureOpenAISettings.cs
    │   │   ├── ChatRequest.cs
    │   │   ├── ChatResponse.cs
    │   │   ├── Message.cs
    │   │   └── McpTool.cs
    │   ├── Services/             # Business logic (clean architecture)
    │   │   ├── ChatService.cs           # Main orchestration
    │   │   ├── AzureOpenAIService.cs    # Azure AI integration
    │   │   ├── McpClientService.cs      # MCP connection manager
    │   │   ├── McpClient.cs             # JSON-RPC protocol
    │   │   ├── McpProcessManager.cs     # Process lifecycle
    │   │   └── ToolSchemaConverter.cs   # Schema conversion
    │   └── wwwroot/              # Frontend files
    │       ├── index.html        # Clean HTML structure
    │       ├── styles.css        # All styling
    │       └── chat.js           # Client-side logic
    └── McpServer/                # MCP server (provides tools)
        ├── Program.cs            # Tool implementations
        └── McpServer.csproj
```

## ⚙️ Configuration

Edit `src/McpWebClient/appsettings.Development.json`:

```json
{
  "AzureOpenAI": {
    "Endpoint": "https://your-endpoint.openai.azure.com/",
    "DeploymentName": "gpt-4.1",
    "ApiKey": "YOUR_API_KEY_HERE"
  }
}
```

**Security Note**: Never commit API keys to source control. Use environment variables for production.

## 🏗️ Architecture

### How It Works

1. **User sends message** → Web UI (`index.html`)
2. **HTTP POST** → `ChatService` receives request
3. **Tool discovery** → `McpClient` gets available tools from MCP server
4. **Send to Azure AI** → `AzureOpenAIService` with tool definitions
5. **AI decides** → Calls tools if needed
6. **Execute tool** → `McpClient` calls MCP server via JSON-RPC
7. **Get response** → Final answer sent back to user

### Available Tools

**MCP Server provides:**
- `get_current_date_time` - Returns current date/time in multiple formats

### Clean Code Organization

Each component has a single, clear responsibility:

- **ChatService.cs** (src/McpWebClient/Services/ChatService.cs:30-105)
  - Orchestrates the full chat flow
  - Coordinates between Azure AI and MCP tools

- **AzureOpenAIService.cs**
  - Creates Azure OpenAI clients
  - Handles chat completions

- **McpClient.cs**
  - Implements JSON-RPC protocol
  - Discovers and calls tools

- **McpProcessManager.cs**
  - Manages MCP server process lifecycle
  - Handles stdin/stdout communication

## 🧪 Testing

Test the API endpoints directly:

```bash
# Test endpoint
curl http://localhost:5000/api/test

# List available MCP tools
curl http://localhost:5000/api/mcp/tools

# Send a chat message
curl -X POST http://localhost:5000/api/chat \
  -H "Content-Type: application/json" \
  -d '{
    "messages": [
      {"role": "user", "content": "What time is it?"}
    ]
  }'
```

## 🔧 Troubleshooting

### "API key is not configured" error
- Check `src/McpWebClient/appsettings.Development.json` has the API key
- Ensure you're running with `ASPNETCORE_ENVIRONMENT=Development`
- Verify the JSON format is correct (no trailing commas)

### MCP Server not starting
- Build the solution first: `dotnet build McpChatSolution.sln`
- Check that `src/McpServer/bin/Debug/net9.0/McpServer.dll` exists
- Look for errors in the console output

### Build errors
- Verify .NET 9 SDK: `dotnet --version`
- Restore packages: `dotnet restore McpChatSolution.sln`
- Clean and rebuild: `dotnet clean && dotnet build`

## 🎓 Learning Notes

This project demonstrates:

**Clean Architecture:**
- Clear separation of concerns
- Each service has a single responsibility
- Easy to test and maintain

**Educational Design:**
- Extensive XML documentation
- Inline comments explaining MCP concepts
- Simple, readable code structure

**Best Practices:**
- Configuration management with appsettings.json
- Proper error handling
- Async/await throughout
- Logging at appropriate levels

## 📚 Resources

- [MCP Documentation](https://modelcontextprotocol.io) - Official MCP specification
- [Azure OpenAI Documentation](https://learn.microsoft.com/en-us/azure/ai-services/openai/)
- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)

## 📝 License

This is a learning example. Use freely for educational purposes.
