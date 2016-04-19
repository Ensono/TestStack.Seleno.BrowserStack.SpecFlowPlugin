using System;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Tracing;
using TestStack.Seleno.BrowserStack.Core.Configuration;
using TestStack.Seleno.BrowserStack.Core.Services.TestSession;

namespace TestStack.Seleno.BrowserStack.Core
{
    [Binding]
    public class TestResultNotifier
    {
        private readonly IBrowserHost _browser;
        private readonly IBrowserStackService _browserStackService;
        private readonly ITraceListener _traceListener;

        public TestResultNotifier(IBrowserHost browser, IBrowserStackService browserStackService,
            ITraceListener traceListener)
        {
            _browser = browser;
            _browserStackService = browserStackService;
            _traceListener = traceListener;
        }

        [AfterScenario]
        public void SendFailingNotificationTestResult()
        {
            var sessionId = _browser.SessionId;
            if (string.IsNullOrWhiteSpace(sessionId)) return;


            if (TestException != null)
            {
                _browserStackService.UpdateTestStatus(sessionId, SessionStatus.Error, TestException.Message);
            }
            LogTestSessionDetails(sessionId);
        }

        public virtual Exception TestException
        {
            get
            {
                var scenarioContext = ScenarioContext.Current;
                Exception result = null;
                if (scenarioContext != null)
                {
                    result = scenarioContext.TestError;
                }
                return result;
            }
        }

        private void LogTestSessionDetails(string sessionId)
        {
            var session = _browserStackService.GetSessionDetail(sessionId);
            _traceListener.WriteTestOutput("---------------------------------------------------------------------------");
            _traceListener.WriteToolOutput("browser stack session detail: " + session.BrowserUrl);
            if (!string.IsNullOrWhiteSpace(session.VideoUrl))
            {
                _traceListener.WriteToolOutput("video url: " + session.VideoUrl);
            }
        }
    }
}
