using System.Collections.ObjectModel;
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
    private static readonly string DeafultFileName = "PdfFile.pdf";

    private static readonly string PlusSymbolName = "message-extension-flyout-command";
    private static readonly string FlyoutListDataTid = "flyout-list-item";
    private static readonly string AttachFromCloudDataTid = "file-attach-from-onedrive";
    private static readonly string FileSelectPopupAttribute = "aria-label";
    private static readonly string MyFilesClassName = "navLink_26dbef85";
    private static readonly string PrimaryButtonClassName = "ms-Button--primary";
    

    public WebDriver WebDriver { get; }
    public TestLogger Logger { get; }
    public WebDriverWait Wait { get; }

    public TeamsActions(WebDriver webDriver, TestLogger logger)
    {
        WebDriver = webDriver;
        Logger = logger;
        Wait = new WebDriverWait(WebDriver, TimeSpan.FromSeconds(15));
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

        var chatElement = WaitForDisplayed(By.XPath(chatXPath));
        
        // Use instead of chatElement.Click() to avoid ElementClickInterceptedException
        ((IJavaScriptExecutor)WebDriver).ExecuteScript("arguments[0].click();", chatElement);
    }

    public void SelectChat()
    {
        SelectChat(DefaultChatName);
    }
    
    public void AttachFile(string fileName)
    {
        // Click plus symbol
        WaitForDisplayed(By.Name(PlusSymbolName)).Click();
        
        // Select attach file
        var flyoutListItems = GetMultipleElements(ByDataTid(FlyoutListDataTid), 3);
        
        var attachFileId = flyoutListItems.Select(item => item.GetAttribute("id")).First();
        ClickButton(attachFileId);

        // Select attach cloud file
        WaitForDisplayed(ByDataTid(AttachFromCloudDataTid)).Click();

        // Switch to popup window
        var iframes = GetMultipleElements(By.TagName("iframe"), 2);
        var iframe = iframes.First(frame => frame.GetAttribute(FileSelectPopupAttribute) is not null);
        WebDriver.SwitchTo().Frame(iframe);

        // Select My files
        Wait.Until(driver => driver.FindElement(By.ClassName(MyFilesClassName))).Click();
        
        // Select File
        WaitForDisplayed(By.XPath($"//*[text()='{fileName}']")).Click();

        // Click on attach file
        WaitForDisplayed(By.ClassName(PrimaryButtonClassName)).Click();
        
        WebDriver.SwitchTo().DefaultContent();
    }

    public void AttachFile()
    {
        AttachFile(DeafultFileName);
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
        return WaitForDisplayed(By.Id(id));
    }
    
    private IWebElement WaitForDisplayed(By by)
    {
        return Wait.Until(driver =>
        {
            var element = driver.FindElement(by);
            return element.Displayed ? element : null;
        });
    }

    private ReadOnlyCollection<IWebElement> GetMultipleElements(By by, int number)
    {
        return Wait.Until(driver =>
        {
            var elements = driver.FindElements(by);
            return elements.Count == number ? elements : null;
        });
    }

    private By ByDataTid(string value)
    {
        return By.XPath($"//*[@data-tid='{value}']");
    }
}