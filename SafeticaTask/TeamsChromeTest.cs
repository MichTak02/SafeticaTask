using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SafeticaTask.Actions;

namespace SafeticaTask;

public class TeamsChromeTest : CommonTest
{
    [SetUp]
    public void ChromeSetup()
    {
        Driver = new ChromeDriver();
        Wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(15));
        GenericActions = new GenericActions(Wait);
        TeamsActions = new TeamsActions(Driver, TestLogger, Wait, GenericActions);
    }

    [Test]
    public void SendSingleOneDriveFile()
    {
        string filename = TeamsActions.DefaultFileName1;
        TeamsActions?.LogIn();
        TeamsActions?.SelectChat();
        DateTime beginSendTime = DateTime.Now;
        TeamsActions?.SendFile(filename);
        DateTime afterSendTime = DateTime.Now;
        var message = TeamsActions?.GetLastMessages(1)[0];
        
        Assert.That(message?.TimeSent, Is.GreaterThanOrEqualTo(beginSendTime).And.LessThanOrEqualTo(afterSendTime));
        Assert.That(message?.Files.Count, Is.EqualTo(1));
        Assert.That(message?.Files[0], Is.EqualTo(filename));
    }
}