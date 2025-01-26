using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace SafeticaTask;

public class TeamsActions
{
    private static readonly string TeamsUrl = "https://teams.microsoft.com/v2/";
    private static readonly string DefaultLogin = "qa@safeticaservices.onmicrosoft.com";
    private static readonly string DefaultPassword = "automation.Safetica2004";

    private static readonly string LoginFieldId = "i0116";
    private static readonly string PasswordFieldId = "i0118";
    private static readonly string SubmitButtonId = "idSIButton9";
    private static readonly string DefaultChatName = "Safetica QA";


    public WebDriver WebDriver { get; }
    public TestLogger Logger { get; }
    public WebDriverWait Wait { get; }

    public TeamsActions(WebDriver webDriver, TestLogger logger)
    {
        WebDriver = webDriver;
        Logger = logger;
        Wait = new WebDriverWait(WebDriver, TimeSpan.FromSeconds(10));
    }


    public void LogIn(string login, string password)
    {
        Logger.LogAction($"Navigating to {TeamsUrl}");
        WebDriver.Navigate().GoToUrl(TeamsUrl);

        Logger.LogAction("Filling in a login and going to next page");
        FillField(LoginFieldId, login);
        ClickButton(SubmitButtonId);

        Logger.LogAction("Filling password and going to next page");
        FillField(PasswordFieldId, password);
        string url = WebDriver.Url;
        ClickButton(SubmitButtonId);

        Logger.LogAction("Confirming to stay logged in");
        Wait.Until(_ =>
            url != WebDriver.Url); // Wait for "stay logged in?" screen before clicking button with the same id
        ClickButton(SubmitButtonId);
    }

    public void LogIn()
    {
        LogIn(DefaultLogin, DefaultPassword);
    }

    public void SelectChat(string chatName)
    {
        Logger.LogAction($"Selecting chat '{chatName}'");
        string chatXPath = $"//*[starts-with(@title, '{chatName}')]";

        var chatElement = Wait.Until(driver =>
        {
            var element = driver.FindElement(By.XPath(chatXPath));
            return element.Displayed ? element : null;
        });
        
        // Use instead of chatElement.Click() to avoid ElementClickInterceptedException
        ((IJavaScriptExecutor)WebDriver).ExecuteScript("arguments[0].click();", chatElement);
    }

    public void SelectChat()
    {
        SelectChat(DefaultChatName);
    }

    private void FillField(string id, string text)
    {
        IWebElement element = WaitForDisplayed(id);

        element.Clear();
        element.SendKeys(text);
    }

    private void ClickButton(string id)
    {
        IWebElement button = WaitForDisplayed(id);
        button.Click();
    }

    private IWebElement WaitForDisplayed(string id)
    {
        return Wait.Until(driver =>
        {
            var element = driver.FindElement(By.Id(id));
            return element.Displayed ? element : null;
        });
    }
}