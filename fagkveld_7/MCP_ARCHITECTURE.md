# MCP Architecture - How It Works

This application demonstrates a **true Model Context Protocol (MCP) implementation** with a web-based chat interface.

## Architecture Overview

```
┌─────────────────────────────────────────────────────────────┐
│                       Web Browser                           │
│                    (Chat Interface)                         │
└───────────────────────┬─────────────────────────────────────┘
                        │ HTTP POST /api/chat
                        ▼
┌─────────────────────────────────────────────────────────────┐
│              ASP.NET Web App (MCP Client)                   │
│  ┌──────────────────────────────────────────────────────┐   │
│  │  Program.cs - Main Web Application                   │   │
│  │  - Receives chat messages                            │   │
│  │  - Calls Azure AI Foundry (LLM)                      │   │
│  │  - When LLM requests a tool call...                  │   │
│  └──────────────────┬───────────────────────────────────┘   │
│                     │                                       │
│  ┌──────────────────▼───────────────────────────────────┐   │
│  │  McpClientService - MCP Protocol Client              │   │
│  │  - Manages MCP server process lifecycle              │   │
│  │  - Communicates via JSON-RPC over stdin/stdout       │   │
│  │  - Discovers tools dynamically (tools/list)          │   │
│  │  - Executes tools (tools/call)                       │   │
│  └──────────────────┬───────────────────────────────────┘   │
└────────────────────┼────────────────────────────────────────┘
                      │ JSON-RPC via stdin/stdout
                      ▼
┌─────────────────────────────────────────────────────────────┐
│              MCP Server (Separate Process)                  │
│  ┌──────────────────────────────────────────────────────┐   │
│  │  McpServer/Program.cs                                │   │
│  │  - Exposes tools via MCP protocol                    │   │
│  │  - DateTimeTool.GetCurrentDateTime()                 │   │
│  │  - Listens on stdin for JSON-RPC requests            │   │
│  │  - Responds on stdout with JSON-RPC responses        │   │
│  └──────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
```

## Project Structure

```
fagkveld_7/
├── Program.cs                 # Web app entry point & Azure AI integration
├── Services/
│   └── McpClientService.cs    # MCP client implementation (JSON-RPC)
├── McpServer/                 # Separate MCP server project
│   ├── McpServer.csproj
│   └── Program.cs             # MCP server with tools
├── wwwroot/
│   └── index.html             # ChatGPT-like interface
└── SimpleMcpServer.csproj     # Web app project file
```

## How the MCP Protocol Works

### 1. Initialization (On App Startup)

The `McpClientService` starts the MCP server as a child process:

```csharp
// Spawn MCP server process
_mcpProcess = new Process
{
    StartInfo = new ProcessStartInfo
    {
        FileName = "dotnet",
        Arguments = "exec McpServer.dll",
        RedirectStandardInput = true,
        RedirectStandardOutput = true,
        // ...
    }
};
_mcpProcess.Start();
```

Then performs the MCP handshake:

```json
// 1. Send initialize request
{
  "jsonrpc": "2.0",
  "id": 1,
  "method": "initialize",
  "params": {
    "protocolVersion": "2024-11-05",
    "clientInfo": { "name": "AspNetMcpClient", "version": "1.0.0" }
  }
}

// 2. Server responds with capabilities
{
  "jsonrpc": "2.0",
  "id": 1,
  "result": { "capabilities": {...}, "serverInfo": {...} }
}

// 3. Send initialized notification
{
  "jsonrpc": "2.0",
  "method": "notifications/initialized"
}
```

### 2. Tool Discovery

The client discovers available tools dynamically:

```json
// Request
{
  "jsonrpc": "2.0",
  "id": 2,
  "method": "tools/list",
  "params": {}
}

// Response
{
  "jsonrpc": "2.0",
  "id": 2,
  "result": {
    "tools": [
      {
        "name": "get_current_datetime",
        "description": "Gets the current date and time...",
        "inputSchema": { "type": "object", "properties": {}, "required": [] }
      }
    ]
  }
}
```

### 3. Chat Flow with Tool Calling

**User asks**: "What time is it?"

1. **Web App → Azure AI Foundry**
   - Sends user message + available tools (from MCP)
   - LLM decides to call `get_current_datetime`

2. **Web App → MCP Server** (via McpClientService)
   ```json
   {
     "jsonrpc": "2.0",
     "id": 3,
     "method": "tools/call",
     "params": {
       "name": "get_current_datetime",
       "arguments": {}
     }
   }
   ```

3. **MCP Server → Web App**
   ```json
   {
     "jsonrpc": "2.0",
     "id": 3,
     "result": {
       "content": [
         {
           "type": "text",
           "text": "Current date and time: 2025-10-28 16:47:00..."
         }
       ]
     }
   }
   ```

4. **Web App → Azure AI Foundry**
   - Sends tool result back to LLM
   - LLM generates natural language response

5. **Web App → User**
   - "The current time is 4:47 PM on October 28th, 2025"

## Key Components Explained

### McpClientService (Services/McpClientService.cs)

This is the heart of the MCP client implementation:

- **Process Management**: Spawns and manages the MCP server process
- **JSON-RPC Communication**: Sends/receives messages over stdin/stdout
- **Protocol Handling**:
  - `InitializeAsync()` - MCP handshake
  - `ListToolsAsync()` - Discovers available tools
  - `CallToolAsync()` - Executes tools via MCP
- **Thread Safety**: Uses `SemaphoreSlim` to ensure sequential message handling

### Program.cs (Main Web App)

- **Azure AI Integration**: Connects to Azure AI Foundry
- **Dynamic Tool Mapping**: Converts MCP tool schemas to Azure OpenAI format
- **Tool Execution Flow**: When LLM requests a tool, forwards the call to MCP server
- **Dependency Injection**: Registers `McpClientService` as a singleton hosted service

### McpServer/Program.cs (MCP Server)

- **ModelContextProtocol SDK**: Uses the official .NET SDK
- **Stdio Transport**: Communicates via standard input/output
- **Tool Auto-Discovery**: Uses `[McpServerTool]` attributes to expose methods
- **Isolated Process**: Runs separately from the web app

## Benefits of This Architecture

### For Your Workshop

1. **Educational**: Participants see both sides of MCP (client + server)
2. **Realistic**: Shows how MCP actually works in production
3. **Extensible**: Easy to add new tools without changing the web app
4. **Transparent**: JSON-RPC messages can be logged for debugging

### For Production Use

1. **Separation of Concerns**: Tools are isolated from the web app
2. **Dynamic Discovery**: Add/remove tools without recompiling the web app
3. **Multiple Servers**: Could connect to multiple MCP servers
4. **Standard Protocol**: Any MCP-compliant server/client can interoperate

## How to Run

```bash
# The web app automatically builds and starts the MCP server
ASPNETCORE_ENVIRONMENT=Development dotnet run

# Open browser to http://localhost:5000
# Try: "What time is it?"
```

## Adding New Tools

To add a new MCP tool, edit `McpServer/Program.cs`:

```csharp
[McpServerToolType]
public static class WeatherTool
{
    [McpServerTool]
    [Description("Gets the current weather for a location")]
    public static string GetWeather(string location)
    {
        // Your implementation
        return $"Weather in {location}: Sunny, 22°C";
    }
}
```

The web app will automatically discover and use the new tool!

## Debugging MCP Communication

Enable debug logging to see JSON-RPC messages:

```csharp
// In McpClientService.cs, uncomment the _logger.LogDebug lines
_logger.LogDebug("Sending MCP request: {Json}", json);
_logger.LogDebug("Received MCP response: {Response}", responseLine);
```

Then check the console output when running the app.

## References

- **MCP Specification**: https://spec.modelcontextprotocol.io/
- **MCP .NET SDK**: https://github.com/modelcontextprotocol/csharp-sdk
- **JSON-RPC 2.0**: https://www.jsonrpc.org/specification

---

## What Makes This "True MCP"?

Unlike simple function calling, this implementation:

✅ Uses the official Model Context Protocol specification
✅ Communicates via JSON-RPC 2.0 over stdio
✅ Performs proper MCP initialization handshake
✅ Dynamically discovers tools (not hardcoded)
✅ Runs MCP server as a separate process
✅ Could work with ANY MCP-compliant server
✅ Demonstrates real-world MCP architecture

This is exactly how VS Code extensions (like Cline, Copilot) connect to MCP servers!
