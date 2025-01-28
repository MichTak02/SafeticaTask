using OpenQA.Selenium;

namespace SafeticaTask.Utils;

/// <summary>
/// Class to extend By functionality
/// </summary>
public static class ByExtensions
{
    /// <summary>
    /// Allows element to by selected by data-tid
    /// </summary>
    /// <param name="value">data-tid value</param>
    /// <returns>By instance allowing to search data-tid</returns>
    public static By ByDataTid(string value)
    {
        return By.XPath($"//*[@data-tid='{value}']");
    }

    /// <summary>
    /// Allows element to by selected by data-testid
    /// </summary>
    /// <param name="value">data-testid value</param>
    /// <returns>By instance allowing to search data-testid</returns>
    public static By ByDataTestId(string value)
    {
        return By.XPath($"//*[@data-testid='{value}']");
    }
}