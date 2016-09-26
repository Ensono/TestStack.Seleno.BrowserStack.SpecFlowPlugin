using System;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using TestStack.Seleno.BrowserStack.Core.Configuration;
using TestStack.Seleno.BrowserStack.Core.Enums;
using TestStack.Seleno.Configuration.Contracts;
using TestStack.Seleno.Configuration.WebServers;

namespace TestStack.Seleno.Browserstack.UnitTests.Core.Configuration
{
    [TestFixture]
    public class BrowserHostFactorySpecs
    {
        private IConfigurationProvider _configurationProvider;

        [SetUp]
        public void SetUp()
        {
            _configurationProvider = Substitute.For<IConfigurationProvider>();
        }

        [Test]
        public void Constructor_ShoudlSetCommandTimeOuto5Minutes()
        {
            // Act
            var sut = new BrowserHostFactory(_configurationProvider);

            // Assert
            sut.CommandTimeOut.Should().Be(5.Minutes());
        }

        [Test]
        public void CreateWithCapabilities_ShouldCreateAndRunBrowserHostWithCapabilitiesAndWebServer()
        {
            // Arrange
            var sut = Substitute.ForPartsOf<BrowserHostFactory>(_configurationProvider);
            var browserHost = Substitute.For<IBrowserHost>();
            var capabilities = Substitute.For<ICapabilities>();
            var browserConfiguration = new BrowserConfiguration();
            Func <RemoteWebDriver> remoteWebDriverFactory = () => new RemoteWebDriver(capabilities);
            var webServer = Substitute.For<IWebServer>();
            const string remoteUrl = "http://some/remote/url";

            _configurationProvider.RemoteUrl.Returns(remoteUrl);
            
            sut.When(x => x.CreateBrowserHost(browserConfiguration)).DoNotCallBase();

            sut.CreateBrowserHost(browserConfiguration).Returns(browserHost);
            sut.CreateRemoteDriverWithCapabilities(Arg.Is(capabilities)).Returns(remoteWebDriverFactory);
            sut.CreateInternetWebServer(Arg.Is(remoteUrl)).Returns(webServer);

            // Act
            var result = sut.CreateWithCapabilities(capabilities, browserConfiguration);

            // Assert
            result.Should().BeSameAs(browserHost);
            browserHost.Received().Run(remoteWebDriverFactory, webServer);
        }

        [Test]
        public void CreatePrivateLocalServer_ShouldCreateAndRunBrowserHostWithCapabilitiesAndWebServer()
        {
            // Arrange
            var sut = Substitute.ForPartsOf<BrowserHostFactory>(_configurationProvider);
            var browserHost = Substitute.For<IBrowserHost>();
            var capabilities = Substitute.For<ICapabilities>();
            var browserConfiguration = new BrowserConfiguration();
            var localWebServer = Substitute.For<IWebServer>();

            Func<RemoteWebDriver> remoteWebDriverFactory = () => null;
            const string remoteUrl = "http://some/remote/url";

            _configurationProvider.RemoteUrl.Returns(remoteUrl);
            

            sut.When(x => x.CreateBrowserHost(browserConfiguration)).DoNotCallBase();
            sut.When(x => x.CreateLocalWebServer()).DoNotCallBase();

            sut.CreateBrowserHost(browserConfiguration).Returns(browserHost);
            sut.CreateLocalWebServer().Returns(localWebServer);

            sut.StartPrivateServerAndCreateRemoteDriverWithCapabilities(capabilities, localWebServer).Returns(remoteWebDriverFactory);

            // Act
            var result = sut.CreatePrivateLocalServer(capabilities, browserConfiguration);

            // Assert
            result.Should().BeSameAs(browserHost);
            browserHost.Received(1).Run(remoteWebDriverFactory, localWebServer);
        }

        [Test]
        public void CreateLocalWebDriver_ShouldCreateLocalBrowserBasedOnBrowserType()
        {
            // Arrange
            var sut = Substitute.ForPartsOf<BrowserHostFactory>(_configurationProvider);
            var browserHost = Substitute.For<IBrowserHost>();
            var capabilities = Substitute.For<ICapabilities>();
            var browserConfiguration = new BrowserConfiguration();
            Func<RemoteWebDriver> remoteWebDriverFactory = () => new RemoteWebDriver(capabilities);
            const string remoteUrl = "http://some/remote/url";
            var webServer = Substitute.For<IWebServer>();
            var browserType = BrowserEnum.Chrome;

            _configurationProvider.RemoteUrl.Returns(remoteUrl);

            browserHost.Configuration.Returns(browserConfiguration);
            sut.When(x => x.CreateBrowserHost(browserConfiguration)).DoNotCallBase();
            sut.CreateBrowserHost(browserConfiguration).Returns(browserHost);
            sut.LocalWebBrowser(browserType).Returns(remoteWebDriverFactory);
            sut.CreateInternetWebServer(Arg.Is(remoteUrl)).Returns(webServer);

            // Act
            var result = sut.CreateLocalWebDriver(browserType, browserConfiguration);

            // Assert
            browserConfiguration.IsLocalWebDriver.Should().BeTrue();
            browserHost.Should().BeSameAs(result);
            browserHost.Received().Run(remoteWebDriverFactory, webServer);
        }

        [Test]
        public void CreateLocalWebServer_ShouldCreateInstanceOfPrivateLocalServer()
        {
            // Arrange
            var sut = Substitute.ForPartsOf<BrowserHostFactory>(_configurationProvider);

            // Act
            var result = sut.CreateLocalWebServer();

            // Assert
            result.Should().BeOfType<PrivateLocalServer>();
        }

        [Test]
        public void CreateInternetWebServer_ShouldCreateInstanceOfPrivateLocalServer()
        {
            // Arrange
            var sut = Substitute.ForPartsOf<BrowserHostFactory>(_configurationProvider);
            const string remoteUrl = "http:/localhost/some/where";

            // Act
            var result = sut.CreateInternetWebServer(remoteUrl);

            // Assert
            result.Should().BeOfType<InternetWebServer>();

        }
    }
}