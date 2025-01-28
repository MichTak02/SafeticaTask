using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SafeticaTask.Exceptions;
using SafeticaTask.Models;
using SafeticaTask.Utils;

namespace SafeticaTask.Actions;


/// <summary>
/// Class for performing actions on MS Teams webpage using selenium
/// </summary>
/// <param name="webDriver">Browser web driver</param>
/// <param name="logger">Logger for logging actions</param>
/// <param name="wait">WebDriverWait instance to wait for elements during operations</param>
/// <param name="genericActions">GenericActions class for performing generic tasks</param>
public class TeamsActions(WebDriver webDriver, TestLogger logger, WebDriverWait wait, GenericActions genericActions)
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

    private WebDriver WebDriver { get; } = webDriver;
    private TestLogger Logger { get; } = logger;
    private WebDriverWait Wait { get; } = wait;
    private bool LoggedIn { get; set; }
    private GenericActions GenericActions { get; } = genericActions;

    /// <summary>
    /// Logs user into a MS Teams account
    /// </summary>
    /// <param name="login">account to use</param>
    /// <param name="password">password to use</param>
    public void LogIn(string login = DefaultLogin, string password = DefaultPassword)
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

    /// <summary>
    /// Selects MS Teams chat by chat name
    /// </summary>
    /// <param name="chatName">chat name</param>
    public void SelectChat(string chatName = DefaultChatName)
    {
        CheckLoginStatus("select chat");
        Logger.LogAction($"Selecting chat '{chatName}'");
        string chatXPath = $"//*[starts-with(@title, '{chatName}')]";

        var chatElement = GenericActions.WaitForDisplayed(By.XPath(chatXPath));
        
        // Use instead of chatElement.Click() to avoid ElementClickInterceptedException
        ((IJavaScriptExecutor)WebDriver).ExecuteScript("arguments[0].click();", chatElement);
    }

    /// <summary>
    /// Sends file from OneDrive to selected chat
    /// </summary>
    /// <param name="fileName">File name</param>
    public void SendFile(string fileName = DefaultFileName1)
    {
        SendFiles([fileName]);
    }

    /// <summary>
    /// Sends multiple files from OneDrive to selected chat
    /// </summary>
    /// <param name="fileNames">List of file names</param>
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

    /// <summary>
    /// Overloaded method for SendFiles with default file names
    /// </summary>
    public void SendFiles()
    {
        SendFiles([DefaultFileName1, DefaultFileName2]);
    }

    /// <summary>
    /// Sends text message to selected chat
    /// </summary>
    /// <param name="message">text message</param>
    public void SendMessage(string message)
    {
        CheckLoginStatus("send message");
        Logger.LogAction($"Sending message '{message}'");
        GenericActions.FillField(ByExtensions.ByDataTid(MessageFieldDataTid), message);

        DateTime beforeSentTime = DateTime.Now;
        GenericActions.ClickButton(ByExtensions.ByDataTid(SendButtonDataTid));
        WaitForSent(beforeSentTime);
    }

    /// <summary>
    /// Gets most recently posted messages
    /// </summary>
    /// <param name="count">Number of messages</param>
    /// <returns>List of messages</returns>
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
    
    /// <summary>
    /// Opens popup for selecting files from OneDrive
    /// </summary>
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

    /// <summary>
    /// Checks if user is logged in
    /// </summary>
    /// <param name="operation">Operation to do after checking login status</param>
    /// <exception cref="NotLoggedInException">Thrown if user is not logged in</exception>
    private void CheckLoginStatus(string operation)
    {
        if (!LoggedIn)
        {
            throw new NotLoggedInException($"Cannot {operation} when not logged in");
        }
    }

    /// <summary>
    /// Waits for message to be sent
    /// </summary>
    /// <param name="minimalDateTime">Lowest time the message was sent at</param>
    private void WaitForSent(DateTime minimalDateTime)
    {
        Wait.Until(driver =>
        {
            var timeElements = driver.FindElements(By.TagName("time"));
            return timeElements.Any(element => DateTime.Parse(element.GetAttribute("datetime")) > minimalDateTime);
        });
    }
}