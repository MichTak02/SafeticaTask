using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using SafeticaTask.Actions;

namespace SafeticaTask.Tests;

public class TeamsFirefoxTest : CommonTest
{
    [SetUp]
    public void FirefoxSetup()
    {
        var options = new FirefoxOptions();
        options.SetPreference("network.cookie.cookieBehavior", 0);
        Driver = new FirefoxDriver(options);
        Wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(15));
        GenericActions = new GenericActions(Wait);
        TeamsActions = new TeamsActions(Driver, TestLogger, Wait, GenericActions);
    }
    
    [Test]
    public void SendTwoOneDriveFilesAtOnce()
    {
        string filename1 = TeamsActions.DefaultFileName1;
        string filename2 = TeamsActions.DefaultFileName2;
        
        TeamsActions?.LogIn();
        TeamsActions?.SelectChat();
        DateTime beginSendTime = DateTime.Now;
        TeamsActions?.SendFiles([filename1, filename2]);
        DateTime afterSendTime = DateTime.Now;
        var message = TeamsActions?.GetLastMessages(1)[0];
        
        Assert.That(message?.TimeSent, Is.GreaterThanOrEqualTo(beginSendTime).And.LessThanOrEqualTo(afterSendTime));
        Assert.That(message?.Files.Count, Is.EqualTo(2));
        Assert.That(message?.Files[0], Is.EqualTo(filename1));
        Assert.That(message?.Files[1], Is.EqualTo(filename2));
    }
    
    [Test]
    public void SendThreeChatMessages()
    {
        List<string> messagesToSend = new List<string>();

        for (int i = 0; i < 3; i++)
        {
            messagesToSend.Add($"{TestName}.{TestContext.CurrentContext.Test.ID}#{i+1}");
        }
        
        TeamsActions?.LogIn();
        TeamsActions?.SelectChat();
        DateTime beginSendTime = DateTime.Now;
        messagesToSend.ForEach(message => TeamsActions?.SendMessage(message));
        DateTime afterSendTime = DateTime.Now;

        var messages = TeamsActions?.GetLastMessages(3);
        Assert.That(messages, Is.Not.Null);
        Assert.That(messages.Count, Is.EqualTo(3));

        for (int i = 0; i < 3; i++)
        {
            Assert.That(messages[i].TimeSent, Is.GreaterThanOrEqualTo(beginSendTime).And.LessThanOrEqualTo(afterSendTime));
            Assert.That(messages[i].Files, Is.Empty);
            Assert.That(messages[i].Message, Is.EqualTo(messagesToSend[i]));
        }
    }
}