namespace SafeticaTask;

/// <summary>
/// Class for logging test actions
/// </summary>
/// <param name="logFilePath">Path to file for logging</param>
public class TestLogger(string logFilePath)
{
    private string LogFilePath { get; } = logFilePath;

    /// <summary>
    /// Method for writing messages associated with current time to log file
    /// </summary>
    /// <param name="message">Message to write in test file</param>
    public void LogAction(string message)
    {
        using StreamWriter writer = new StreamWriter(LogFilePath, append: true);
        writer.WriteLine($"{DateTime.Now:s} {message}");
    }
}