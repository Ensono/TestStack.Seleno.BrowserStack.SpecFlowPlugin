using System;
using System.Security.Authentication;
using NSubstitute;
using NUnit.Framework;
using TechTalk.SpecFlow.Tracing;
using TestStack.Seleno.BrowserStack.Core;
using TestStack.Seleno.BrowserStack.Core.Configuration;
using TestStack.Seleno.BrowserStack.Core.Services.BrowserStack;
using TestStack.Seleno.BrowserStack.Core.Services.TestSession;

namespace TestStack.Seleno.Browserstack.UnitTests.Core
{
    [TestFixture]
    public class TestResultNotifierSpecs
    {
        private IBrowserHost _browser;
        private IBrowserStackService _browserStackService;
        private ITraceListener _traceListener;
        private TestResultNotifier _sut;
        private IConfigurationProvider _configurationProvider;

        [SetUp]
        public void SetUp()
        {
            _browser = Substitute.For<IBrowserHost>();
            _browserStackService = Substitute.For<IBrowserStackService>();
            _traceListener = Substitute.For<ITraceListener>();
            _configurationProvider = Substitute.For<IConfigurationProvider>();
            _sut = Substitute.ForPartsOf<TestResultNotifier>(_browser, _browserStackService, _traceListener, _configurationProvider);
        }

        [TestCase("")]
        [TestCase("  ")]
        [TestCase(null)]
        public void SendFailingNotificationTestResult_ShouldDoNothingWhenSessionIdIsNullOrEmpty(string emptySessionId)
        {
            // Arrange
            var sessionId = Guid.NewGuid().ToString();
            _browser.SessionId.Returns(emptySessionId);
            
            // Act
            _sut.SendFailingNotificationTestResult();

            // Assert

            _browserStackService.DidNotReceive().UpdateTestStatus(emptySessionId, SessionStatus.Error, "");
            _browserStackService.DidNotReceive().GetSessionDetail(emptySessionId);
            _traceListener.DidNotReceive().WriteToolOutput(Arg.Any<string>());
            _traceListener.DidNotReceive().WriteTestOutput(Arg.Any<string>());
        }

        [Test]
        public void SendFailingNotificationTestResult_ShouldNotCallUpdateTestStatusWhenNoTestExceptionOccurred()
        {
            // Arrange
            var sessionId = Guid.NewGuid().ToString();
            _browser.SessionId.Returns(sessionId);
            _sut.TestException.Returns(null as Exception);
            _browserStackService.GetSessionDetail(sessionId).Returns(new AutomationSession());
            
            // Act
            _sut.SendFailingNotificationTestResult();

            // Assert

            _browserStackService.DidNotReceive().UpdateTestStatus(sessionId, SessionStatus.Error, "");
        }

        [Test]
        public void SendFailingNotificationTestResult_ShouldUpdateFailinhTestStatusWhenTestExceptionOccurred()
        {
            // Arrange
            var sessionId = Guid.NewGuid().ToString();
            const string errorMessage = "wrong credentials";
            _browser.SessionId.Returns(sessionId);
            
            _sut.TestException.Returns(new InvalidCredentialException(errorMessage));
            _browserStackService.GetSessionDetail(sessionId).Returns(new AutomationSession());

            // Act
            _sut.SendFailingNotificationTestResult();

            // Assert

            _browserStackService.Received(1).UpdateTestStatus(sessionId, SessionStatus.Error, errorMessage);
        }

        [Test]
        public void SendFailingNotificationTestResult_ShouldAlwaysOutputBrowserStackSessionDetails()
        {
            // Arrange
            var sessionId = Guid.NewGuid().ToString();
            const string sessionUrl = "http://some/session/url";
            var session = new AutomationSession { BrowserUrl = sessionUrl };
            
            _browser.SessionId.Returns(sessionId);
            _browserStackService.GetSessionDetail(sessionId).Returns(session);

            // Act
            _sut.SendFailingNotificationTestResult();

            // Assert
            _traceListener.Received().WriteToolOutput("browser stack session detail: " + sessionUrl);
            _traceListener.DidNotReceive().WriteTestOutput("video url: " + session.VideoUrl);
        }

        [Test]
        public void SendFailingNotificationTestResult_ShouldOutputBrowserStackVideoUrlWhenAvailable()
        {
            // Arrange
            var sessionId = Guid.NewGuid().ToString();
            const string videoUrl = "http://some/session/video/url";
            var session = new AutomationSession { VideoUrl = "http://some/session/video/url" };
            
            _browser.SessionId.Returns(sessionId);
            _browserStackService.GetSessionDetail(sessionId).Returns(session);

            // Act
            _sut.SendFailingNotificationTestResult();

            // Assert
            _traceListener.DidNotReceive().WriteTestOutput("video url: " + videoUrl);
        }
    }
}