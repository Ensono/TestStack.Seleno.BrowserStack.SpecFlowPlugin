using System;
using Castle.Core.Internal;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Tracing;
using TestStack.Seleno.BrowserStack.Core.Configuration;
using TestStack.Seleno.BrowserStack.Core.Services.BrowserStack;
using TestStack.Seleno.BrowserStack.Core.Services.TestSession;

namespace TestStack.Seleno.BrowserStack.Core
{
    [Binding]
    public class TestResultNotifier
    {
        private readonly IBrowserHost _browser;
        private readonly IBrowserStackService _browserStackService;
        private readonly ITraceListener _traceListener;
        private readonly IConfigurationProvider _configurationProvider;

        public TestResultNotifier(IBrowserHost browser, IBrowserStackService browserStackService,
            ITraceListener traceListener, IConfigurationProvider configurationProvider)
        {
            _browser = browser;
            _browserStackService = browserStackService;
            _traceListener = traceListener;
            _configurationProvider = configurationProvider;
        }

        [AfterScenario]
        public void SendFailingNotificationTestResult()
        {
            if (!_configurationProvider.UseLocalBrowser.IsNullOrEmpty()) return;
            
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
