# Project Structure Explained

## Clean Structure (Current)

```
fagkveld_7/                          # Solution root
├── McpChatSolution.sln             # Solution file (references both projects)
├── README.md                        # Main documentation
├── MCP_ARCHITECTURE.md             # Architecture details
└── src/                            # Source code folder
    ├── SimpleMcpServer/            # Project 1: Web Application (MCP Client)
    │   ├── SimpleMcpServer.csproj
    │   ├── Program.cs              # ASP.NET Core entry point (78 lines)
    │   ├── appsettings.json
    │   ├── appsettings.Development.json
    │   ├── Models/                 # Data models
    │   ├── Services/               # Business logic
    │   │   ├── ChatService.cs      # Orchestrates chat flow
    │   │   ├── AzureOpenAIService.cs
    │   │   ├── McpClientService.cs
    │   │   ├── McpClient.cs        # JSON-RPC protocol
    │   │   ├── McpProcessManager.cs
    │   │   └── ToolSchemaConverter.cs
    │   └── wwwroot/                # Frontend
    │       ├── index.html
    │       ├── styles.css
    │       └── chat.js
    └── McpServer/                  # Project 2: MCP Server (Console App)
        ├── McpServer.csproj
        └── Program.cs              # MCP server with tools
```

## What Each Project Does

### SimpleMcpServer (Web Application)

**Type:** ASP.NET Core Web App
**Role:** MCP **Client**
**Port:** 5000

**What it does:**
1. Serves the web UI (chat interface)
2. Provides REST API endpoints (`/api/chat`, `/api/test`)
3. Connects to Azure OpenAI
4. **Launches McpServer** as a child process
5. Communicates with McpServer to execute tools

**Key files:**
- `Program.cs` - Web app configuration and API endpoints
- `Services/ChatService.cs` - Main chat orchestration
- `Services/McpClient.cs` - JSON-RPC client for MCP protocol
- `Services/McpProcessManager.cs` - Manages the MCP server process

### McpServer (Console Application)

**Type:** .NET Console App
**Role:** MCP **Server**
**Communication:** stdin/stdout (JSON-RPC)

**What it does:**
1. Provides tools (functions) that the AI can call
2. Runs as a **separate process** (started by SimpleMcpServer)
3. Listens for JSON-RPC requests on stdin
4. Returns tool results via stdout

**Key files:**
- `Program.cs` - MCP server implementation with tool definitions

**Current tools:**
- `get_current_date_time` - Returns current date/time in multiple formats

## How They Work Together

```
┌─────────────────────────────────────────────────────────────┐
│                         User's Browser                       │
│                    (http://localhost:5000)                   │
└─────────────────────┬───────────────────────────────────────┘
                      │ HTTP
                      ▼
┌─────────────────────────────────────────────────────────────┐
│                    SimpleMcpServer                           │
│                   (ASP.NET Core Web App)                     │
│  ┌──────────────────────────────────────────────────────┐  │
│  │ ChatService.cs                                        │  │
│  │  1. Receives user message                            │  │
│  │  2. Discovers tools from MCP server                  │  │
│  │  3. Sends to Azure OpenAI                            │  │
│  │  4. If AI needs tools → calls McpClient              │  │
│  └───────────┬──────────────────────────────────────────┘  │
│              │                                               │
│  ┌───────────▼──────────────────────────────────────────┐  │
│  │ McpClient.cs (JSON-RPC)                              │  │
│  │  - Sends tool requests via stdin                     │  │
│  │  - Reads tool results from stdout                    │  │
│  └───────────┬──────────────────────────────────────────┘  │
└──────────────┼───────────────────────────────────────────────┘
               │ stdin/stdout
               │ (JSON-RPC over pipes)
               ▼
┌─────────────────────────────────────────────────────────────┐
│                       McpServer                              │
│                    (Console Application)                     │
│  ┌──────────────────────────────────────────────────────┐  │
│  │ MCP Server (ModelContextProtocol SDK)                │  │
│  │  - Listens on stdin for JSON-RPC requests           │  │
│  │  - Executes tools                                    │  │
│  │  - Returns results via stdout                        │  │
│  └──────────────────────────────────────────────────────┘  │
│                                                              │
│  Tools:                                                      │
│  - get_current_date_time()                                   │
│  - (add more here)                                           │
└─────────────────────────────────────────────────────────────┘
```

## Relationship Between Projects

**They are sibling projects, NOT nested!**

### ✅ Correct (Current Structure)
```
src/
├── SimpleMcpServer/    ← Separate folder
└── McpServer/          ← Separate folder (sibling)
```

### ❌ Wrong (Old Structure - Now Fixed)
```
SimpleMcpServer/
├── Program.cs
└── McpServer/          ← Nested inside! Confusing!
    └── Program.cs
```

## Build Relationship

Even though they're separate projects, SimpleMcpServer **depends on** McpServer:

**SimpleMcpServer.csproj** contains:
```xml
<Target Name="BuildMcpServer" BeforeTargets="Build">
  <Exec Command="dotnet build $(ProjectDir)../McpServer/McpServer.csproj" />
</Target>

<Target Name="CopyMcpServer" AfterTargets="Build">
  <!-- Copies McpServer.dll to SimpleMcpServer/bin/Debug/net9.0/McpServer/ -->
</Target>
```

**Why?**
- SimpleMcpServer needs to launch McpServer.dll at runtime
- The build process automatically copies McpServer to the right location
- Both projects are included in the solution for easy development

## Running the Application

**From solution root:**
```bash
# Build everything
dotnet build McpChatSolution.sln

# Run the web app (it will launch MCP server automatically)
cd src/SimpleMcpServer
ASPNETCORE_ENVIRONMENT=Development dotnet run
```

**What happens:**
1. SimpleMcpServer starts
2. Reads `appsettings.Development.json` for Azure config
3. Launches `McpServer.dll` as a child process
4. Opens HTTP server on port 5000
5. Ready to handle chat requests!

## Summary

| Aspect | SimpleMcpServer | McpServer |
|--------|-----------------|-----------|
| **Type** | Web Application | Console Application |
| **Role** | MCP Client | MCP Server |
| **Protocol** | HTTP (for web) + JSON-RPC (to MCP) | JSON-RPC (stdin/stdout) |
| **Runs** | Main process | Child process |
| **Location** | `src/SimpleMcpServer/` | `src/McpServer/` |
| **Purpose** | User interface & orchestration | Provides tools for AI |
| **Dependencies** | Depends on McpServer | Standalone |

Both are separate projects at the same level under `src/`, working together through process communication!
