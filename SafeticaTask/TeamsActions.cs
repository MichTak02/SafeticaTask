using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace SafeticaTask;

public class TeamsActions(WebDriver webDriver, TestLogger logger)
{
    public static readonly string TeamsUrl = "https://teams.microsoft.com/v2/";
    public static readonly string DefaultLogin = "qa@safeticaservices.onmicrosoft.com";
    public static readonly string DefaultPassword = "automation.Safetica2004";
    
    public WebDriver WebDriver { get; } = webDriver;
    public TestLogger Logger { get; } = logger;
}