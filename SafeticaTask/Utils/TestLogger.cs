namespace SafeticaTask.Utils;

/// <summary>
/// Class for logging test actions
/// </summary>
public class TestLogger
{
    private static readonly string LogDir = Path.Join(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "logs");

    /// <summary>
    /// Class for logging test actions
    /// </summary>
    /// <param name="logFileName">Path to file for logging</param>
    public TestLogger(string logFileName)
    {
        LogFileName = logFileName;
        if (!Directory.Exists(LogDir))
        {
            Directory.CreateDirectory(LogDir);
        }
    }

    private string LogFileName { get; }

    /// <summary>
    /// Method for writing messages associated with current time to log file
    /// </summary>
    /// <param name="message">Message to write in test file</param>
    public void LogAction(string message)
    {
        using StreamWriter writer = new StreamWriter(Path.Join(LogDir, LogFileName), append: true);
        writer.WriteLine($"{DateTime.Now:s} {message}");
    }

    
    /// <summary>
    /// Logs start of test
    /// </summary>
    /// <param name="testName">name of current test</param>
    public void LogTestStart(string testName)
    {
        LogAction($"Starting test: {testName}");
    }

    /// <summary>
    /// Logs successful test
    /// </summary>
    /// <param name="testName">Name of current test</param>
    public void LogTestPassed(string testName)
    {
        LogAction($"Test {testName} passed");
    }

    /// <summary>
    /// Logs failed test
    /// </summary>
    /// <param name="testName">Name of current test</param>
    /// <param name="failMessage">Fail message</param>
    public void LogTestFailed(string testName, string failMessage)
    {
        LogAction($"Test {testName} failed: {failMessage}");
    }
}