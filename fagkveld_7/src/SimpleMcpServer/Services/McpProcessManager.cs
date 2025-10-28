using System.Diagnostics;

namespace SimpleMcpServer.Services;

/// <summary>
/// Manages the lifecycle of the MCP server process.
/// Handles starting, stopping, and managing the stdin/stdout/stderr streams.
/// </summary>
public class McpProcessManager : IDisposable
{
    private readonly ILogger<McpProcessManager> _logger;
    private Process? _mcpProcess;
    private StreamWriter? _stdin;
    private StreamReader? _stdout;
    private StreamReader? _stderr;
    private Task? _stderrReaderTask;

    // Timeout constants with explanatory comments
    private const int ServerStartupDelayMs = 500;  // Give the server time to initialize
    private const int ProcessKillTimeoutMs = 5000; // Wait up to 5 seconds for graceful shutdown

    public McpProcessManager(ILogger<McpProcessManager> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Gets the standard input stream for writing JSON-RPC requests to the MCP server.
    /// </summary>
    public StreamWriter? StandardInput => _stdin;

    /// <summary>
    /// Gets the standard output stream for reading JSON-RPC responses from the MCP server.
    /// </summary>
    public StreamReader? StandardOutput => _stdout;

    /// <summary>
    /// Indicates whether the MCP server process is currently running.
    /// </summary>
    public bool IsRunning => _mcpProcess != null && !_mcpProcess.HasExited;

    /// <summary>
    /// Starts the MCP server process.
    /// The server is started as a separate dotnet process that communicates via stdin/stdout.
    /// </summary>
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting MCP Server process...");

        try
        {
            // Locate the MCP server DLL
            var mcpServerPath = Path.Combine(AppContext.BaseDirectory, "McpServer", "McpServer.dll");
            _logger.LogInformation("MCP Server path: {Path}", mcpServerPath);

            if (!File.Exists(mcpServerPath))
            {
                throw new FileNotFoundException(
                    $"MCP Server not found at: {mcpServerPath}. " +
                    "Make sure the McpServer project is built and copied to the output directory.");
            }

            // Configure and start the process
            _mcpProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"exec \"{mcpServerPath}\"",
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    WorkingDirectory = Path.Combine(AppContext.BaseDirectory, "McpServer")
                }
            };

            _mcpProcess.Start();

            // Capture the streams for communication
            _stdin = _mcpProcess.StandardInput;
            _stdout = _mcpProcess.StandardOutput;
            _stderr = _mcpProcess.StandardError;

            // Start a background task to read and log stderr
            // This helps with debugging by showing server errors in the application logs
            _stderrReaderTask = Task.Run(async () =>
            {
                while (!_mcpProcess.HasExited && _stderr != null)
                {
                    var line = await _stderr.ReadLineAsync();
                    if (!string.IsNullOrEmpty(line))
                    {
                        _logger.LogDebug("MCP Server stderr: {Line}", line);
                    }
                }
            }, cancellationToken);

            // Give the server a moment to start up before we try to communicate
            await Task.Delay(ServerStartupDelayMs, cancellationToken);

            _logger.LogInformation("MCP Server process started successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start MCP Server process");
            throw;
        }
    }

    /// <summary>
    /// Stops the MCP server process gracefully.
    /// </summary>
    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Stopping MCP Server process...");

        try
        {
            // Close stdin to signal the server to shut down
            _stdin?.Close();

            // Kill the process if it doesn't exit gracefully
            if (_mcpProcess != null && !_mcpProcess.HasExited)
            {
                _mcpProcess.Kill();
                _mcpProcess.WaitForExit(ProcessKillTimeoutMs);
            }

            // Wait for stderr reader task to complete
            if (_stderrReaderTask != null)
            {
                await _stderrReaderTask;
            }

            _logger.LogInformation("MCP Server process stopped");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error stopping MCP Server process");
        }
    }

    public void Dispose()
    {
        _stdin?.Dispose();
        _stdout?.Dispose();
        _stderr?.Dispose();
        _mcpProcess?.Dispose();
    }
}
