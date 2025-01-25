using NUnit.Framework.Interfaces;

namespace SafeticaTask;

public class CommonTest
{
    private readonly TestLogger _testLogger = new(TestLogger.DefaultLogFilePath);
    private string TestName => TestContext.CurrentContext.Test.FullName;

    [SetUp]
    public void Setup()
    {
        _testLogger.LogTestStart(TestName);
    }

    [TearDown]
    public void TearDown()
    {
        var testResult = TestContext.CurrentContext.Result.Outcome;

        if (testResult.Equals(ResultState.Success))
        {
            _testLogger.LogTestPassed(TestName);
            return;
        }

        var failMessage = TestContext.CurrentContext.Result.Message ?? "";
        _testLogger.LogTestFailed(TestName, failMessage);
    }
}