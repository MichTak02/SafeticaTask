using OpenQA.Selenium;

namespace SafeticaTask.Utils;

public static class ByExtensions
{
    public static By ByDataTid(string value)
    {
        return By.XPath($"//*[@data-tid='{value}']");
    }

    public static By ByDataTestId(string value)
    {
        return By.XPath($"//*[@data-testid='{value}']");
    }
}