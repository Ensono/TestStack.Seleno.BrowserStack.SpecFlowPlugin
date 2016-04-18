namespace TestStack.Seleno.BrowserStack.SpecFlowPlugin.Services.TestSession
{
    public interface IBrowserStackService
    {
        AutomationSession GetSessionDetail(string sessionId);
        void UpdateTestStatus(string sessionId, SessionStatus status, string reason);
    }
}