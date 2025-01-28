using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SafeticaTask.Exceptions;
using SafeticaTask.Models;
using SafeticaTask.Utils;

namespace SafeticaTask.Actions;

public class TeamsActions
{
    private const string TeamsUrl = "https://teams.microsoft.com/v2/";
    private const string DefaultLogin = "qa@safeticaservices.onmicrosoft.com";
    private const string DefaultPassword = "automation.Safetica2004";

    private const string LoginFieldId = "i0116";
    private const string PasswordFieldId = "i0118";
    private const string SubmitButtonId = "idSIButton9";
    private const string DefaultChatName = "Safetica QA";
    public const string DefaultFileName1 = "PdfFile.pdf";
    public const string DefaultFileName2 = "TxtFile.txt";

    private const string PlusSymbolName = "message-extension-flyout-command";
    private const string FlyoutListDataTid = "flyout-list-item";
    private const string AttachFromCloudDataTid = "file-attach-from-onedrive";
    private const string FileSelectPopupAttribute = "aria-label";
    private const string MyFilesClassName = "navLink_26dbef85";
    private const string PrimaryButtonClassName = "ms-Button--primary";
    private const string SelectedClassName = "navLinkSelected_26dbef85";

    private const string MessageFieldDataTid = "ckeditor";
    private const string SendButtonDataTid = "sendMessageCommands-send";
    private const string SendButtonWithFileDataTid = "newMessageCommands-send";

    private const string MessageDataTestId = "message-wrapper";
    private const string AttachmentHeaderCassName = "ui-attachment__header";

    public WebDriver WebDriver { get; }
    public TestLogger Logger { get; }
    public WebDriverWait Wait { get; }
    public bool LoggedIn { get; private set; }
    public GenericActions GenericActions { get; }

    public TeamsActions(WebDriver webDriver, TestLogger logger, WebDriverWait wait, GenericActions genericActions)
    {
        WebDriver = webDriver;
        Logger = logger;
        Wait = wait;
        LoggedIn = false;
        GenericActions = genericActions;
    }

    public void LogIn(string login, string password)
    {
        Logger.LogAction($"Navigating to {TeamsUrl}");
        WebDriver.Navigate().GoToUrl(TeamsUrl);

        Logger.LogAction("Filling in a login and going to next page");
        GenericActions.FillField(By.Id(LoginFieldId), login);
        GenericActions.ClickButton(By.Id(SubmitButtonId));

        Logger.LogAction("Filling password and going to next page");
        GenericActions.FillField(By.Id(PasswordFieldId), password);
        string url = WebDriver.Url;
        GenericActions.ClickButton(By.Id(SubmitButtonId));

        Logger.LogAction("Confirming to stay logged in");
        Wait.Until(_ =>
            url != WebDriver.Url); // Wait for "stay logged in?" screen before clicking button with the same id
        GenericActions.ClickButton(By.Id(SubmitButtonId));
        
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

        var chatElement = GenericActions.WaitForDisplayed(By.XPath(chatXPath));
        
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
        fileNames.ForEach(fileName => GenericActions.ClickButton(By.XPath($"//*[@title='{fileName}']"), false));
        
        // Click on attach file
        Logger.LogAction("Clicking on Attach file button");
        GenericActions.WaitForDisplayed(By.ClassName(PrimaryButtonClassName)).Click();

        WebDriver.SwitchTo().DefaultContent();
        Logger.LogAction("Sending file(s)");
        DateTime beforeSentTime = DateTime.Now;
        GenericActions.ClickButton(ByExtensions.ByDataTid(SendButtonWithFileDataTid));
        WaitForSent(beforeSentTime);
    }

    public void SendFiles()
    {
        SendFiles([DefaultFileName1, DefaultFileName2]);
    }

    public void SendMessage(string message)
    {
        CheckLoginStatus("send message");
        Logger.LogAction($"Sending message '{message}'");
        GenericActions.FillField(ByExtensions.ByDataTid(MessageFieldDataTid), message);

        DateTime beforeSentTime = DateTime.Now;
        GenericActions.ClickButton(ByExtensions.ByDataTid(SendButtonDataTid));
        WaitForSent(beforeSentTime);
    }

    public List<TeamsMessage> GetLastMessages(int count)
    {
        List<TeamsMessage> messages = [];
        var allMessageElements = GenericActions.GetMultipleElementsRange(ByExtensions.ByDataTestId(MessageDataTestId), count, -1);
        var messageElements = allMessageElements.TakeLast(count);
        
        foreach (var messageElement in messageElements)
        {
            var files = Wait.Until(_ => messageElement.FindElements(By.ClassName(AttachmentHeaderCassName)));
            var filenames = files.Select(file => file.Text).ToList();
            var stringTime = messageElement.FindElement(By.TagName("time")).GetAttribute("datetime");
            var time = DateTime.Parse(stringTime);
            var message = "";

            try
            {
                message = messageElement.FindElement(By.TagName("p")).Text;
            }
            catch (NoSuchElementException)
            {
            }
            
            messages.Add(new TeamsMessage(message, filenames, time));
        }
        
        return messages;
    }
    
    private void OpenFileSelectPopup()
    {
        // Click plus symbol
        Logger.LogAction("Clicking on plus symbol to add file");
        GenericActions.WaitForDisplayed(By.Name(PlusSymbolName)).Click();
        
        // Select attach file
        Logger.LogAction("Selecting to attach file");
        var flyoutListItems = GenericActions.GetMultipleElementsRange(ByExtensions.ByDataTid(FlyoutListDataTid), 2, 3);
        
        var attachFileId = flyoutListItems.Select(item => item.GetAttribute("id")).First();
        GenericActions.ClickButton(By.Id(attachFileId));

        // Select attach cloud file
        Logger.LogAction("Selecting to attach cloud file");
        GenericActions.WaitForDisplayed(ByExtensions.ByDataTid(AttachFromCloudDataTid)).Click();
        
        // Switch to popup window
        var iframes = GenericActions.GetMultipleElements(By.TagName("iframe"), 2, false);
        var iframe = iframes.First(frame => frame.GetAttribute(FileSelectPopupAttribute) is not null);
        WebDriver.SwitchTo().Frame(iframe);
    }

    private void CheckLoginStatus(string operation)
    {
        if (!LoggedIn)
        {
            throw new NotLoggedInException($"Cannot {operation} when not logged in");
        }
    }

    private void WaitForSent(DateTime minimalDateTime)
    {
        Wait.Until(driver =>
        {
            var timeElements = driver.FindElements(By.TagName("time"));
            return timeElements.Any(element => DateTime.Parse(element.GetAttribute("datetime")) > minimalDateTime);
        });
    }
}