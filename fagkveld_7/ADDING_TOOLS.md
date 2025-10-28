# How to Add MCP Tools

This guide shows you how to add new tools that the AI can use.

## 🎯 Quick Summary

1. **Edit** `src/McpServer/Program.cs`
2. **Add** a new class with `[McpServerToolType]`
3. **Add** methods with `[McpServerTool]` and `[Description]`
4. **Build** and run - tools are auto-discovered!

## 📝 Step-by-Step Guide

### 1. Open the McpServer Project

File location: `src/McpServer/Program.cs`

This is where all MCP tools are defined.

### 2. Add a Tool Class

Create a new static class with the `[McpServerToolType]` attribute:

```csharp
[McpServerToolType]
public static class YourToolName
{
    // Your tool methods go here
}
```

### 3. Add Tool Methods

Add static methods with these attributes:

```csharp
[McpServerTool]
[Description("What your tool does")]
public static string MethodName()
{
    // Your implementation
    return "result";
}
```

### 4. Build and Run

```bash
dotnet build McpChatSolution.sln
cd src/SimpleMcpServer
dotnet run
```

That's it! The tool is automatically discovered and available to the AI.

## 🔧 Tool Examples

### Example 1: Simple Tool (No Parameters)

```csharp
[McpServerToolType]
public static class DateTimeTool
{
    [McpServerTool]
    [Description("Gets the current date and time")]
    public static string GetCurrentDateTime()
    {
        var now = DateTime.Now;
        return $"Current time: {now:yyyy-MM-dd HH:mm:ss}";
    }
}
```

**AI can call it:** "What time is it?"

### Example 2: Tool with Parameters

```csharp
[McpServerToolType]
public static class CalculatorTool
{
    [McpServerTool]
    [Description("Adds two numbers together")]
    public static string Add(
        [Description("First number")] double a,
        [Description("Second number")] double b)
    {
        var result = a + b;
        return $"{a} + {b} = {result}";
    }
}
```

**AI can call it:** "What is 5 plus 3?"

### Example 3: Tool with Complex Logic

```csharp
[McpServerToolType]
public static class WeatherTool
{
    [McpServerTool]
    [Description("Gets weather for a city")]
    public static string GetWeather(
        [Description("City name")] string city)
    {
        // In a real app, you'd call a weather API here
        // For demo, we'll return mock data

        var random = new Random();
        var temp = random.Next(15, 30);
        var conditions = new[] { "Sunny", "Cloudy", "Rainy" };
        var condition = conditions[random.Next(conditions.Length)];

        return $"Weather in {city}:\n" +
               $"Temperature: {temp}°C\n" +
               $"Conditions: {condition}";
    }
}
```

**AI can call it:** "What's the weather in Oslo?"

### Example 4: Multiple Tools in One Class

```csharp
[McpServerToolType]
public static class StringTools
{
    [McpServerTool]
    [Description("Converts text to uppercase")]
    public static string ToUpperCase(
        [Description("Text to convert")] string text)
    {
        return text.ToUpper();
    }

    [McpServerTool]
    [Description("Reverses a string")]
    public static string ReverseString(
        [Description("Text to reverse")] string text)
    {
        var chars = text.ToCharArray();
        Array.Reverse(chars);
        return new string(chars);
    }

    [McpServerTool]
    [Description("Counts words in text")]
    public static string CountWords(
        [Description("Text to analyze")] string text)
    {
        var words = text.Split(new[] { ' ', '\n', '\t' },
            StringSplitOptions.RemoveEmptyEntries);
        return $"Word count: {words.Length}";
    }
}
```

## 📋 Important Rules

### ✅ Do This:

- **Use static classes** with `[McpServerToolType]`
- **Use static methods** with `[McpServerTool]`
- **Add descriptions** with `[Description]` on the method
- **Add parameter descriptions** with `[Description]` on parameters
- **Return strings** (for simplicity, though other types are supported)
- **Keep methods simple** and focused

### ❌ Don't Do This:

- Don't use instance classes (must be static)
- Don't use instance methods (must be static)
- Don't forget the `[Description]` attribute
- Don't make tools too complex (keep them focused)
- Don't use async methods (not currently supported by MCP SDK)

## 🔍 How It Works

### Auto-Discovery

In `Program.cs`, this line discovers all your tools:

```csharp
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();  // ← Finds all [McpServerTool] methods
```

The MCP SDK:
1. **Scans** the assembly for `[McpServerToolType]` classes
2. **Finds** all methods with `[McpServerTool]`
3. **Generates** JSON schemas from method signatures
4. **Registers** them as available tools
5. **Routes** calls to the correct methods

### Schema Generation

The SDK automatically creates JSON schemas:

```csharp
[McpServerTool]
[Description("Adds two numbers")]
public static string Add(
    [Description("First number")] double a,
    [Description("Second number")] double b)
```

**Becomes:**
```json
{
  "name": "add",
  "description": "Adds two numbers",
  "inputSchema": {
    "type": "object",
    "properties": {
      "a": {
        "description": "First number",
        "type": "number"
      },
      "b": {
        "description": "Second number",
        "type": "number"
      }
    },
    "required": ["a", "b"]
  }
}
```

## 🧪 Testing Your Tools

### Method 1: Check Tool Discovery

```bash
cd src/SimpleMcpServer
dotnet run

# In another terminal:
curl http://localhost:5000/api/mcp/tools | python3 -m json.tool
```

You should see your new tool in the list!

### Method 2: Test via Chat UI

1. Open http://localhost:5000 in your browser
2. Ask the AI to use your tool:
   - "Calculate 5 plus 3" (for `Add` tool)
   - "What time is it?" (for `GetCurrentDateTime` tool)
   - "Reverse the text 'hello'" (for `ReverseString` tool)

The AI will automatically detect when to use your tools!

## 💡 Tips for Good Tools

### 1. Clear Descriptions

**Good:**
```csharp
[Description("Calculates the area of a circle given its radius")]
```

**Bad:**
```csharp
[Description("Circle area")]
```

### 2. Descriptive Parameter Names

**Good:**
```csharp
public static string CalculateCircleArea(
    [Description("Radius of the circle in meters")] double radius)
```

**Bad:**
```csharp
public static string CalculateCircleArea(double r)
```

### 3. Informative Return Values

**Good:**
```csharp
return $"The area of a circle with radius {radius}m is {area:F2} square meters";
```

**Bad:**
```csharp
return area.ToString();
```

### 4. Error Handling

```csharp
[McpServerTool]
[Description("Divides two numbers")]
public static string Divide(
    [Description("Numerator")] double a,
    [Description("Denominator")] double b)
{
    if (b == 0)
    {
        return "Error: Cannot divide by zero";
    }

    var result = a / b;
    return $"{a} ÷ {b} = {result}";
}
```

## 🎓 Real-World Tool Ideas

Here are some practical tools you could add:

### 1. File System Tools
```csharp
[McpServerTool]
[Description("Lists files in a directory")]
public static string ListFiles([Description("Directory path")] string path)
```

### 2. API Integration Tools
```csharp
[McpServerTool]
[Description("Gets latest Bitcoin price")]
public static string GetBitcoinPrice()
```

### 3. Data Processing Tools
```csharp
[McpServerTool]
[Description("Converts JSON to CSV format")]
public static string JsonToCsv([Description("JSON string")] string json)
```

### 4. Calculation Tools
```csharp
[McpServerTool]
[Description("Calculates mortgage payment")]
public static string CalculateMortgage(
    [Description("Loan amount")] double amount,
    [Description("Interest rate (annual %)")] double rate,
    [Description("Loan term (years)")] int years)
```

## 🔗 Summary

**Adding a tool is as simple as:**

```csharp
[McpServerToolType]
public static class MyTools
{
    [McpServerTool]
    [Description("Does something useful")]
    public static string DoSomething(
        [Description("Parameter description")] string parameter)
    {
        return "result";
    }
}
```

**Then build and run:**
```bash
dotnet build
cd src/SimpleMcpServer
dotnet run
```

Your tool is now available to the AI! 🎉

---

## 📚 Reference

- **MCP SDK Documentation:** https://github.com/modelcontextprotocol/csharp-sdk
- **MCP Protocol Spec:** https://modelcontextprotocol.io
- **Current Example:** See `src/McpServer/Program.cs` for working examples
