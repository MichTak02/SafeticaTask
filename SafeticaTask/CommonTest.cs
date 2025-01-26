using NUnit.Framework.Interfaces;

namespace SafeticaTask;

public class CommonTest
{
    protected readonly TestLogger TestLogger = new(TestLogger.DefaultLogFilePath);
    private string TestName => TestContext.CurrentContext.Test.FullName;

    [SetUp]
    public void Setup()
    {
        TestLogger.LogTestStart(TestName);
    }

    [TearDown]
    public void TearDown()
    {
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