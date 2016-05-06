using System;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using TestStack.Seleno.BrowserStack.Core.Configuration;
using TestStack.Seleno.Configuration;
using TestStack.Seleno.Configuration.Contracts;

namespace TestStack.Seleno.Browserstack.UnitTests.Core.Configuration
{
    [TestFixture]
    public class BrowserHostSpecs
    {
        private BrowserHost _sut;
        private SelenoHost _selenoHost;
        private BrowserConfiguration _browserConfiguration;

        public interface IDummyWebDriver : IWebDriver, IHasSessionId { }

        [SetUp]
        public void SetUp()
        {
            _selenoHost = new SelenoHost();
            _browserConfiguration = new BrowserConfiguration();
            _sut = Substitute.ForPartsOf<BrowserHost>(_selenoHost, _browserConfiguration);
        }

        [Test]
        public void Constructor_ShouldSetBrowserConfigurationProperty()
        {
            // Act
            _sut = Substitute.ForPartsOf<BrowserHost>(_selenoHost, _browserConfiguration);

            // Assert
            _sut.Configuration.Should().BeSameAs(_browserConfiguration);
        }

        [Test]
        public void Run_ShouldStartHostAndMaximizeBrowserWindow()
        {
            // Arrange
            Func<RemoteWebDriver> remoteWebDriverFactory = () => new RemoteWebDriver(new DesiredCapabilities());
            var webServer = Substitute.For<IWebServer>();
            _selenoHost.Application = Substitute.For<ISelenoApplication>();
            var browser = Substitute.For<IWebDriver>();
            var browserOptions = Substitute.For<IOptions>();
            var windows = Substitute.For<IWindow>();

            _sut.When(x => x.StartHost(remoteWebDriverFactory, webServer)).DoNotCallBase();
            _selenoHost.Application.Browser.Returns(browser);
            browser.Manage().Returns(browserOptions);
            browserOptions.Window.Returns(windows);

            // Act
            _sut.Run(remoteWebDriverFactory, webServer);

            // Assert
            _sut.Received(1).StartHost(remoteWebDriverFactory, webServer);
            windows.Received(1).Maximize();
        }


        [Test]
        public void SessionId_ShouldReturnEmptyWhenApplicationIsNotInitialised()
        {
            // Act && Assert
            _sut.SessionId.Should().BeEmpty();
        }

        [Test]
        public void SessionId_ShouldReturnEmptyWhenDriverDoesNotImplementHasSessionId()
        {
            // Arrange
            var browser = Substitute.For<IWebDriver>();
            _selenoHost.Application = Substitute.For<ISelenoApplication>();
            _selenoHost.Application.Browser.Returns(browser);

            // Act && Assert
            _sut.SessionId.Should().BeEmpty();
        }

        [Test]
        public void SessionId_ShouldReturnRemotWebDriverSessionId()
        {
            // Arrange
            var sessionId = Guid.NewGuid().ToString();
            var browser = Substitute.For<IDummyWebDriver>();
            _selenoHost.Application = Substitute.For<ISelenoApplication>();
            _selenoHost.Application.Browser.Returns(browser);
            browser.SessionId.Returns(new SessionId(sessionId));


            // Act && Assert
            _sut.SessionId.Should().Be(sessionId);
        }
    }
}