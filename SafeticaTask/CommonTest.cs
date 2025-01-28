using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SafeticaTask.Actions;

namespace SafeticaTask;

public class CommonTest
{
    protected readonly TestLogger TestLogger = new(TestLogger.DefaultLogFilePath);
    protected string TestName => TestContext.CurrentContext.Test.FullName;
    protected WebDriver? Driver;
    protected TeamsActions? TeamsActions;
    protected WebDriverWait? Wait;
    protected GenericActions? GenericActions;

    [SetUp]
    public void Setup()
    {
        TestLogger.LogTestStart(TestName);
    }

    [TearDown]
    public void TearDown()
    {
        Driver?.Quit();
        
        var testResult = TestContext.CurrentContext.Result.Outcome;

        if (testResult.Equals(ResultState.Success))
        {
            TestLogger.LogTestPassed(TestName);
            return;
        }

        var failMessage = TestContext.CurrentContext.Result.Message ?? "";
        TestLogger.LogTestFailed(TestName, failMessage);
    }
}