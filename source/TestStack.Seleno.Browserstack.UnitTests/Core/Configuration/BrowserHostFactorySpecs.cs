using System;
using BoDi;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using TestStack.Seleno.BrowserStack.Core.Configuration;
using TestStack.Seleno.Configuration.Contracts;

namespace TestStack.Seleno.Browserstack.UnitTests.Core.Configuration
{
    [TestFixture]
    public class BrowserHostFactorySpecs
    {
        private IConfigurationProvider _configurationProvider;
        private IObjectContainer _container;

        [SetUp]
        public void SetUp()
        {
            _configurationProvider = Substitute.For<IConfigurationProvider>();
            _container = Substitute.For<IObjectContainer>();
        }

        [Test]
        public void Constructor_ShoudlSetCommandTimeOuto5Minutes()
        {
            // Act
            var sut = new BrowserHostFactory(_configurationProvider, _container);

            // Assert
            sut.CommandTimeOut.Should().Be(5.Minutes());
        }

        [Test]
        public void CreateWithCapabilities_ShouldCreateAndRunBrowserHostWithCapabilitiesAndWebServer()
        {
            // Arrange
            var sut = Substitute.ForPartsOf<BrowserHostFactory>(_configurationProvider, _container);
            var browserHost = Substitute.For<IBrowserHost>();
            var capabilities = Substitute.For<ICapabilities>();

            Func <RemoteWebDriver> remoteWebDriverFactory = () => new RemoteWebDriver(capabilities);
            var webServer = Substitute.For<IWebServer>();
            const string remoteUrl = "http://some/remote/url";

            _configurationProvider.RemoteUrl.Returns(remoteUrl);
            sut.When(x => x.CreateBrowserHost()).DoNotCallBase();

            sut.When(x => x.CreateBrowserHost()).DoNotCallBase();
            sut.CreateBrowserHost().Returns(browserHost);
            sut.CreateRemoteDriverWithCapabilities(Arg.Is(capabilities)).Returns(remoteWebDriverFactory);
            sut.CreateWebServer(Arg.Is(remoteUrl)).Returns(webServer);

            // Act
            var result = sut.CreateWithCapabilities(capabilities);

            // Assert
            result.Should().BeSameAs(browserHost);
            browserHost.Received().Run(remoteWebDriverFactory, webServer);
        }
    }
}