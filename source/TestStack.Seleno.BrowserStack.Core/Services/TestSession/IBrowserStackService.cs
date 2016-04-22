using TestStack.Seleno.BrowserStack.Core.Configuration;

namespace TestStack.Seleno.BrowserStack.Core.Services.TestSession
{
    public interface IBrowserStackService
    {
        AutomationSession GetSessionDetail(string sessionId);
        void UpdateTestStatus(string sessionId, SessionStatus status, string reason);
        bool IsNotSupported(BrowserConfiguration browserConfiguration);
    }
}