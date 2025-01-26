using NUnit.Framework.Interfaces;
using OpenQA.Selenium;

namespace SafeticaTask;

public class CommonTest
{
    protected readonly TestLogger TestLogger = new(TestLogger.DefaultLogFilePath);
    private string TestName => TestContext.CurrentContext.Test.FullName;
    protected WebDriver? Driver;

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