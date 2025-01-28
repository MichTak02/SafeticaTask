using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using SafeticaTask.Actions;

namespace SafeticaTask;

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
}