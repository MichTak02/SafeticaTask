using System.Collections.ObjectModel;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SafeticaTask.Exceptions;

namespace SafeticaTask;

public class TeamsActions
{
    private static readonly TimeSpan DefaultWaitTimeout = TimeSpan.FromSeconds(15);
    private static readonly string TeamsUrl = "https://teams.microsoft.com/v2/";
    private static readonly string DefaultLogin = "qa@safeticaservices.onmicrosoft.com";
    private static readonly string DefaultPassword = "automation.Safetica2004";

    private static readonly string LoginFieldId = "i0116";
    private static readonly string PasswordFieldId = "i0118";
    private static readonly string SubmitButtonId = "idSIButton9";
    private static readonly string DefaultChatName = "Safetica QA";
    private static readonly string DefaultFileName1 = "PdfFile.pdf";
    private static readonly string DefaultFileName2 = "TxtFile.txt";

    private static readonly string PlusSymbolName = "message-extension-flyout-command";
    private static readonly string FlyoutListDataTid = "flyout-list-item";
    private static readonly string AttachFromCloudDataTid = "file-attach-from-onedrive";
    private static readonly string FileSelectPopupAttribute = "aria-label";
    private static readonly string MyFilesClassName = "navLink_26dbef85";
    private static readonly string PrimaryButtonClassName = "ms-Button--primary";
    private static readonly string SelectedClassName = "navLinkSelected_26dbef85";
    
    private static readonly string MessageFieldDataTid = "ckeditor";
    private static readonly string SendButtonDataTid = "sendMessageCommands-send";
    private static readonly string SendButtonWithFileDataTid = "newMessageCommands-send";
    
    public WebDriver WebDriver { get; }
    public TestLogger Logger { get; }
    public WebDriverWait Wait { get; }
    public Actions Actions { get; }
    public bool LoggedIn { get; private set; }

    public TeamsActions(WebDriver webDriver, TestLogger logger) : this(webDriver, logger, DefaultWaitTimeout)
    {
    }

    public TeamsActions(WebDriver webDriver, TestLogger logger, TimeSpan waitTimeout)
    {
        WebDriver = webDriver;
        Logger = logger;
        Wait = new WebDriverWait(WebDriver, waitTimeout);
        Actions = new Actions(WebDriver);
        LoggedIn = false;
    }

    public void LogIn(string login, string password)
    {
        Logger.LogAction($"Navigating to {TeamsUrl}");
        WebDriver.Navigate().GoToUrl(TeamsUrl);

        Logger.LogAction("Filling in a login and going to next page");
        FillField(By.Id(LoginFieldId), login);
        ClickButton(By.Id(SubmitButtonId));

        Logger.LogAction("Filling password and going to next page");
        FillField(By.Id(PasswordFieldId), password);
        string url = WebDriver.Url;
        ClickButton(By.Id(SubmitButtonId));

        Logger.LogAction("Confirming to stay logged in");
        Wait.Until(_ =>
            url != WebDriver.Url); // Wait for "stay logged in?" screen before clicking button with the same id
        ClickButton(By.Id(SubmitButtonId));
        
        Wait.Until(driver => driver.Url == "https://teams.microsoft.com/v2/");
        LoggedIn = true;
    }

    public void LogIn()
    {
        LogIn(DefaultLogin, DefaultPassword);
    }

    public void SelectChat(string chatName)
    {
        CheckLoginStatus("select chat");
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

    public void SendFile(string fileName)
    {
        SendFiles([fileName]);
    }

    public void SendFile()
    {
        SendFile(DefaultFileName1);
    }

    public void SendFiles(List<string> fileNames)
    {
        CheckLoginStatus("send files");
        OpenFileSelectPopup();
        
        // Select My files
        Logger.LogAction("Selecting My files");
        Wait.Until(driver => driver.FindElement(By.ClassName(MyFilesClassName))).Click();
        // Wait for tab to be selected to avoid StaleElementReferenceException
        Wait.Until(driver => driver.FindElement(By.CssSelector($".{MyFilesClassName}.{SelectedClassName}")));
        
        // Select Files
        Logger.LogAction("Selecting file");
        SelectMultipleItems(fileNames);
        
        // Click on attach file
        Logger.LogAction("Clicking on Attach file button");
        WaitForDisplayed(By.ClassName(PrimaryButtonClassName)).Click();

        WebDriver.SwitchTo().DefaultContent();
        Logger.LogAction("Sending file(s)");
        ClickButton(ByDataTid(SendButtonWithFileDataTid));
    }

    public void SendFiles()
    {
        SendFiles([DefaultFileName1, DefaultFileName2]);
    }

    public void SendMessage(string message)
    {
        CheckLoginStatus("send message");
        Logger.LogAction("Sending message");
        FillField(ByDataTid(MessageFieldDataTid), message);
        ClickButton(ByDataTid(SendButtonDataTid));
    }

    private void OpenFileSelectPopup()
    {
        // Click plus symbol
        Logger.LogAction("Clicking on plus symbol to add file");
        WaitForDisplayed(By.Name(PlusSymbolName)).Click();
        
        // Select attach file
        Logger.LogAction("Selecting to attach file");
        var flyoutListItems = GetMultipleElements(ByDataTid(FlyoutListDataTid), 3);
        
        var attachFileId = flyoutListItems.Select(item => item.GetAttribute("id")).First();
        ClickButton(By.Id(attachFileId));

        // Select attach cloud file
        Logger.LogAction("Selecting to attach cloud file");
        WaitForDisplayed(ByDataTid(AttachFromCloudDataTid)).Click();
        
        // Switch to popup window
        var iframes = GetMultipleElements(By.TagName("iframe"), 2);
        var iframe = iframes.First(frame => frame.GetAttribute(FileSelectPopupAttribute) is not null);
        WebDriver.SwitchTo().Frame(iframe);
    }
    
    private void FillField(By by, string text)
    {
        IWebElement element = WaitForDisplayed(by);
        element.SendKeys(text);
    }
    
    private void ClickButton(By by)
    {
        IWebElement button = WaitForDisplayed(by);
        button.Click();
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

    private void SelectMultipleItems(List<string> itemNames)
    {
        Actions.KeyDown(Keys.Control).Perform();
        itemNames.ForEach(itemName => ClickButton(By.XPath($"//*[text()='{itemName}']")));
        Actions.KeyUp(Keys.Control).Perform();
    }

    private void CheckLoginStatus(string operation)
    {
        if (!LoggedIn)
        {
            throw new NotLoggedInException($"Cannot {operation} when not logged in");
        }
    } 
}