# Simple DateTime MCP Server

A minimal Model Context Protocol (MCP) server built with .NET 9 that provides a simple tool to get the current date and time.

## What is MCP?

Model Context Protocol (MCP) is an open protocol that enables AI assistants (like GitHub Copilot, Claude, etc.) to connect to external tools and data sources. This project demonstrates a basic MCP server that can be used with VSCode.

**Perfect for Learning Sessions**: This project uses VSCode extensions that connect to **your Azure AI Foundry** API keys, so each developer can have their own isolated setup without requiring GitHub Copilot subscriptions.

## Quick Start Summary

For your learning session, each developer needs to:
1. Install the **Cline** extension in VSCode (free)
2. Configure Cline with their Azure AI Foundry API key
3. Open this project folder in VSCode
4. Build the project (`dotnet build`)
5. Cline will automatically discover the MCP server
6. Start using the date/time tool through Cline!

## Preparation Checklist for Session Organizer

Before the session, ensure you have:
- [ ] Created individual API keys in Azure AI Foundry for each developer
- [ ] Documented the following for your team:
  - [ ] Azure Resource Name
  - [ ] Deployment Name (which model they'll use)
  - [ ] API Version (recommend `2024-10-01-preview`)
  - [ ] Endpoint URL format
- [ ] Shared this project repository with all developers
- [ ] Verified .NET 9 SDK is installed on developer machines (or provided installation instructions)
- [ ] Created a quick reference guide with the Cline configuration values

## Prerequisites

- .NET 9 SDK (or .NET 8 LTS)
- Visual Studio Code
- **Cline extension** (Install from VSCode Marketplace: `saoudrizwan.claude-dev`)
- Azure AI Foundry access with the following information for each developer:
  - API Key
  - Resource Name (e.g., `your-resource-name`)
  - Deployment Name (e.g., `gpt-4o`, `gpt-35-turbo`)
  - Endpoint URL (format: `https://YOUR-RESOURCE.openai.azure.com`)

## Project Structure

```
SimpleMcpServer/
├── Program.cs              # MCP server implementation
├── SimpleMcpServer.csproj  # Project file
├── .vscode/
│   └── mcp.json           # VSCode MCP configuration
└── README.md              # This file
```

## The Tool

This server exposes one simple tool:

- **`GetCurrentDateTime`**: Returns the current date and time in multiple formats:
  - Standard format (yyyy-MM-dd HH:mm:ss)
  - ISO 8601 format
  - Unix timestamp

## Setup Instructions

### Step 1: Build the Project

```bash
dotnet build
```

### Step 2: Test the Server (Optional)

You can test the server directly by running:

```bash
dotnet run
```

The server will start and listen for MCP protocol messages via stdin/stdout.

### Step 3: Configure VSCode

The `.vscode/mcp.json` file is already configured for you. This file tells VSCode how to start the MCP server.

**Important**: VSCode will automatically discover and load MCP servers configured in `.vscode/mcp.json` when you open this project folder.

### Step 4: Install and Configure Cline Extension

1. **Install Cline Extension**:
   - Open VSCode Extensions (Cmd+Shift+X or Ctrl+Shift+X)
   - Search for "Cline" (by saoudrizwan)
   - Click Install

2. **Configure Azure OpenAI**:
   - Click the Cline icon in the VSCode sidebar (opens the chat interface)
   - Click the settings gear icon
   - Select **"Azure OpenAI"** as your API provider
   - Enter your configuration:
     - **API Key**: Your Azure AI Foundry API key
     - **Resource Name**: Your Azure resource name
     - **Deployment Name**: Your deployment name (e.g., `gpt-4o`)
     - **API Version**: `2024-10-01-preview` (or latest)

### Step 5: Using the MCP Server with Cline

1. **Open VSCode** in the `SimpleMcpServer` folder
2. **Open Cline Chat**:
   - Click the Cline icon in the sidebar
   - OR press Cmd+Shift+P (Ctrl+Shift+P) and type "Cline: Open Chat"
3. **Verify MCP Server is detected**:
   - Cline automatically discovers MCP servers from `.vscode/mcp.json`
   - You should see your server loaded in the Cline output/logs
4. **Use the tool** by asking Cline:
   ```
   Can you use the available MCP tool to get the current date and time?
   ```
5. Cline will automatically call your `GetCurrentDateTime` tool and return the results!

## Alternative: Using with Continue.dev Extension

If you prefer Continue.dev over Cline, it also supports Azure OpenAI + MCP:

1. **Install Continue.dev** from VSCode Marketplace
2. **Configure Azure OpenAI** in `~/.continue/config.yaml`:
   ```yaml
   models:
     - provider: azure
       model: gpt-4o
       apiKey: YOUR_API_KEY
       endpoint: https://YOUR-RESOURCE.openai.azure.com
       apiVersion: "2024-10-01-preview"

   context:
     - name: mcp
       params:
         command: dotnet
         args:
           - run
           - --project
           - SimpleMcpServer.csproj
   ```
3. Use Continue's chat interface to interact with your MCP tools

## Troubleshooting

### Server not detected in VSCode
- Ensure the `.vscode/mcp.json` file exists in the project root
- Try reloading VSCode window (Cmd+Shift+P → "Reload Window")
- Check VSCode output panel for MCP-related errors

### Build errors
- Verify .NET SDK is installed: `dotnet --version`
- Ensure NuGet packages are restored: `dotnet restore`

### Tool not appearing in Cline
- Make sure the Cline extension is installed and configured with your Azure API key
- Verify the `.vscode/mcp.json` file exists in the project root
- Check Cline's output logs for MCP server connection messages
- Try restarting Cline (close and reopen the Cline sidebar)
- Try asking explicitly: "What MCP tools are available to you?"
- Check the Output panel (View → Output → "Cline" or "MCP") for errors

### Azure OpenAI configuration issues
- Verify your API key is correct
- Check your deployment name matches what's in Azure portal
- Ensure your Azure resource has the model deployed
- Try the API version `2024-10-01-preview` if others don't work

## Customization

To add more tools, simply add new methods to the `DateTimeTool` class (or create new classes) with the `[McpServerTool]` attribute:

```csharp
[McpServerToolType]
public static class MyCustomTools
{
    [McpServerTool]
    [Description("Your tool description here")]
    public static string MyNewTool(string parameter)
    {
        // Your implementation
        return "result";
    }
}
```

## Resources

### MCP and .NET
- [MCP Documentation](https://modelcontextprotocol.io)
- [.NET MCP SDK](https://github.com/modelcontextprotocol/csharp-sdk)
- [Microsoft MCP Quickstart](https://learn.microsoft.com/en-us/dotnet/ai/quickstarts/build-mcp-server)

### VSCode Extensions
- [Cline Extension](https://marketplace.visualstudio.com/items?itemName=saoudrizwan.claude-dev)
- [Cline Documentation](https://docs.cline.bot/)
- [Continue.dev](https://www.continue.dev/)
- [Continue.dev Documentation](https://docs.continue.dev/)

### Azure AI Foundry
- [Azure AI Foundry Documentation](https://learn.microsoft.com/en-us/azure/ai-foundry/)
- [Azure OpenAI Service](https://azure.microsoft.com/en-us/products/ai-services/openai-service)

## License

This is a learning example. Use freely for educational purposes.
