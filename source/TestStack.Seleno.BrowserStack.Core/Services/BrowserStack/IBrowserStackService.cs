using TestStack.Seleno.BrowserStack.Core.Configuration;
using TestStack.Seleno.BrowserStack.Core.Services.TestSession;

namespace TestStack.Seleno.BrowserStack.Core.Services.BrowserStack
{
    public interface IBrowserStackService
    {
        AutomationSession GetSessionDetail(string sessionId);
        void UpdateTestStatus(string sessionId, SessionStatus status, string reason);
        bool IsNotSupported(BrowserConfiguration browserConfiguration);
    }
}