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
        WebDriver.Navigate().GoToUrl(TeamsUrl);
        
        FillField(LoginFieldId, login);
        ClickButton(SubmitButtonId);
        
        FillField(PasswordFieldId, password);
        string url = WebDriver.Url;
        ClickButton(SubmitButtonId);
        Wait.Until(_ => url != WebDriver.Url);
        
        ClickButton(SubmitButtonId);
    }

    public void LogIn()
    {
        LogIn(DefaultLogin, DefaultPassword);
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