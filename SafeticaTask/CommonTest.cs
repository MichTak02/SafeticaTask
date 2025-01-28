using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SafeticaTask.Actions;
using SafeticaTask.Utils;

namespace SafeticaTask;

public class CommonTest
{
    protected static readonly TimeSpan WaitTimeOut = TimeSpan.FromSeconds(15);
    protected TestLogger? TestLogger;
    protected string TestName => TestContext.CurrentContext.Test.FullName;
    protected WebDriver? Driver;
    protected TeamsActions? TeamsActions;
    protected WebDriverWait? Wait;
    protected GenericActions? GenericActions;

    [SetUp]
    public void Setup()
    {
        string logFileName = $"{DateTime.Now:yyyy-MM-ddTHH-mm-ss}-{TestName}.txt";
        TestLogger = new TestLogger(logFileName);
        TestLogger.LogTestStart(TestName);
    }

    [TearDown]
    public void TearDown()
    {
        Driver?.Quit();
        
        var testResult = TestContext.CurrentContext.Result.Outcome;

        if (testResult.Equals(ResultState.Success))
        {
            TestLogger?.LogTestPassed(TestName);
            return;
        }

        var failMessage = TestContext.CurrentContext.Result.Message ?? "";
        TestLogger?.LogTestFailed(TestName, failMessage);
    }
}