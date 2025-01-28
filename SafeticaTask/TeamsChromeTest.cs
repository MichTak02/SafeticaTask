using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SafeticaTask.Actions;

namespace SafeticaTask;

public class TeamsChromeTest
{
    
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
}