using System;
using System.Linq.Expressions;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using OpenQA.Selenium;
using TestStack.Seleno.BrowserStack.Core.Capabilities;
using TestStack.Seleno.BrowserStack.Core.Configuration;

namespace TestStack.Seleno.Browserstack.UnitTests.Core.Capabilities
{
    [TestFixture]
    public class CapabilitiesBuilderSpecs
    {
        private CapabilitiesBuilder _sut;
        private IConfigurationProvider _configuration;

        [SetUp]
        public void SetUp()
        {
            _configuration = Substitute.For<IConfigurationProvider>();
            _sut = new CapabilitiesBuilder(_configuration);
        }

        [Test]
        public void Build_ShouldCreateDefaultBrowserCapabilities()
        {   
            // Arrange
            const string userName = "automation@host.com";
            var accessKey = Guid.NewGuid().ToString();

            _configuration.UserName.Returns(userName);
            _configuration.AccessKey.Returns(accessKey);

            // Act
            var result = _sut.Build();

            // Assert
            result.Should().Match(DefaultBrowserCapabilities(userName, accessKey));
        }
        
        [Test]
        public void Build_ShouldSetTestNameAndProjectAndBuildNumber()
        {
            //Arrange
            var buildNumber = Guid.NewGuid().ToString();
            var testSpecification = new TestSpecification("my scenario", "some feature");

            _sut
                .WithTestSpecification(testSpecification)
                .WithBuildNumber(buildNumber);

            // Act
            var result = _sut.Build();

            // Assert
            result.GetCapability("name").Should().Be(testSpecification.ScenarioTitle);
            result.GetCapability("project").Should().Be(testSpecification.FeatureTitle);
            result.GetCapability("build").Should().Be(buildNumber);
        }

        [Test]
        public void Build_ShouldOverrideCredentialsSpecifiedByConfigurationProvider()
        {
            //Arrange
            const string userName = "someUserName";
            var accessKey = Guid.NewGuid().ToString();

            _configuration.UserName.Returns(Guid.NewGuid().ToString());
            _configuration.AccessKey.Returns(Guid.NewGuid().ToString());

            _sut.WithCredentials(userName, accessKey);

            // Act
            var result = _sut.Build();

            // Assert
            result.GetCapability("browserstack.user").Should().Be(userName);
            result.GetCapability("browserstack.key").Should().Be(accessKey);

        }

        [Test]
        public void Build_ShouldOnlySetMobileBrowserCapabilities()
        {
            //Arrange
            const string browserName = "iPhone";
            const string device = "iPhone 6S Plus";
            _sut.WithBrowserConfiguration(new BrowserConfiguration(browserName, device));

            // Act
            var result = _sut.Build();

            // Assert
            result.Should().Match(HaveMobileCapabilities(browserName, device));
        }

        [Test]
        public void Build_ShouldOnlySetDesktopBrowserCapabilities()
        {
            //Arrange
            const string browserName = "chrome";
            const string version = "48.0";
            const string osName = "Windows";
            const string osVersion = "10";
            _sut.WithBrowserConfiguration(new BrowserConfiguration(browserName, version, osName, osVersion));

            // Act
            var result = _sut.Build();

            // Assert
            result
                .Should()
                .Match(HaveDesktopCapabilities(browserName, version, osName, osVersion, PlatformType.Windows));
        }

        private Expression<Func<ICapabilities, bool>> DefaultBrowserCapabilities(string userName, string accessKey)
        {
                return x => x.GetCapability("version").Equals("ANY") &&
                            x.GetCapability("os").Equals("ANY") &&
                            x.GetCapability("os_version").Equals("ANY") &&
                            x.GetCapabilityAs<Platform>("platform").PlatformType == PlatformType.Any &&
                            !x.HasCapability("device") &&
                            x.GetCapability("browserstack.user").Equals(userName) &&
                            x.GetCapability("browserstack.key").Equals(accessKey) &&
                            x.GetCapability("name").Equals(string.Empty) &&
                            x.GetCapability("project").Equals(string.Empty) &&
                            x.GetCapability("build").Equals(string.Empty);

        }

        private Expression<Func<ICapabilities, bool>> HaveMobileCapabilities(string browserName, string device)
        {
            return
                x => x.GetCapability("browserName").Equals(browserName) &&
                     x.GetCapability("device").Equals(device) &&
                     !x.HasCapability("version") &&
                     !x.HasCapability("os_version") &&
                     !x.HasCapability("platform") &&
                     !x.HasCapability("os");
        }

        private Expression<Func<ICapabilities, bool>> HaveDesktopCapabilities(string browserName, string version, string osName, string osVersion, PlatformType platformType)
        {
            return
                x => x.GetCapability("browserName").Equals(browserName) &&
                     !x.HasCapability("device") &&
                     x.GetCapability("version").Equals(version) &&
                     x.GetCapability("os_version").Equals(osVersion) &&
                     x.GetCapabilityAs<Platform>("platform").IsPlatformType(platformType)&&
                     x.GetCapability("os").Equals(osName);
        }
    }
}