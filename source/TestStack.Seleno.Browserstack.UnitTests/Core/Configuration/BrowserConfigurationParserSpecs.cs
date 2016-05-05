using System;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using TestStack.Seleno.BrowserStack.Core.Configuration;
using TestStack.Seleno.BrowserStack.Core.Exceptions;
using TestStack.Seleno.BrowserStack.Core.Services.TestSession;

namespace TestStack.Seleno.Browserstack.UnitTests.Core.Configuration
{
    [TestFixture]
    public class BrowserConfigurationParserSpecs
    {
        private BrowserConfigurationParser _sut;
        private IBrowserStackService _browserStackService;

        [SetUp]
        public void SetUp()
        {
            _browserStackService = Substitute.For<IBrowserStackService>();
            _sut = new BrowserConfigurationParser(_browserStackService);
        }

        [TestCase("")]
        [TestCase("  ")]
        [TestCase(null)]
        public void Parse_ShouldReturnThrowAnInvalidBrowserConfigurationExceptionWhenBrowserConfigurationIsNullOrEmpty(string nullOrBlankBrowserConfiguration)
        {
            // Act
            Action attemptToParseEmptyBrowserConfiguration = () => _sut.Parse(nullOrBlankBrowserConfiguration);

            // Act & Assert
            attemptToParseEmptyBrowserConfiguration
                .ShouldThrow<InvalidBrowserConfigurationException>()
                .WithMessage("No browser configuration was specified");
        }

        [TestCase("firefox,43.0,Windows")]
        [TestCase("firefox,43.0,OS_X,Lion,1024x768,blah!")]
        [TestCase("android,Samsung_Galaxy_S5,45")]
        public void Parse_ShouldThrowAnInvalidBrowserConfigurationExceptionWhenTheBrowserConfigurationNumberOfParametersIsUnSupported(string invalidBrowserConfig)
        {
            // Arrange
            Action attemptToParseInvalidBrowserConfiguration = () => _sut.Parse(invalidBrowserConfig);
            const string errorMessage = "Browser configuration is expected to contain browser name, browser version, " +
                                        "operating system and operating system version or browser name or platform name and device name";

            // Act & Assert
            attemptToParseInvalidBrowserConfiguration
                    .ShouldThrow<InvalidBrowserConfigurationException>()
                    .WithMessage(errorMessage);
        }

        [Test]
        public void Parse_ShouldThrownAnInvalidBrowserConfigurationExceptionWhenThereIsOnlyOneParameterWhichIsDesktopResolution()
        {
            // Arrange
            Action attemptToParseInvalidBrowserConfiguration = () => _sut.Parse("1024x768");

            // Act & Assert
            attemptToParseInvalidBrowserConfiguration
                    .ShouldThrow<InvalidBrowserConfigurationException>()
                    .WithMessage("First and only parameter cannot be a desktop resolution");
        }

        [TestCase("firefox,43.0,OS_X,Lion,Other")]
        [TestCase("firefox,43.0,OS_X,Lion,AxB")]
        [TestCase("firefox,43.0,OS_X,Lion,1024")]
        [TestCase("firefox,43.0,OS_X,Lion,1024x")]
        [TestCase("firefox,43.0,OS_X,Lion,")]
        public void Parse_ShouldThrowAnInvalidDesktopResolutionWhenTheBrowserConfigurationthParameterIsNotExpectedResolutionFormat(string invalidBrowserConfig)
        {
            // Arrange
            Action attemptToParseInvalidBrowserConfiguration = () => _sut.Parse(invalidBrowserConfig);
            var resolution = invalidBrowserConfig.Split(',')[4];
            var errorMessage =$"{resolution} is not a valid resolution. A valid one looks like 1024x768";

            // Act & Assert
            attemptToParseInvalidBrowserConfiguration
                    .ShouldThrow<InvalidDesktopResolution>()
                    .WithMessage(errorMessage);
        }

        [TestCase("firefox,wrong_version,OS_X,Lion")]
        [TestCase("firefox,46abc,OS_X,Lion")]
        [TestCase("firefox,0xsada,OS_X,Lion")]
        public void Parse_ShouldThrowAnInvalidBrowserConfigurationExceptionWhenTheBrowserConfigurationHasFourPartsButIncorrectBrowserVersion(string invalidBrowserConfig)
        {
            // Arrange
            Action attemptToParseInvalidBrowserConfiguration = () => _sut.Parse(invalidBrowserConfig);
            const string errorMessage = "Browser version must be numeric";

            // Act & Assert
            attemptToParseInvalidBrowserConfiguration
                    .ShouldThrow<InvalidBrowserConfigurationException>()
                    .WithMessage(errorMessage);
        }

        [TestCase("chrome")]
        [TestCase("firefox,43.0,OS_X,Lion")]
        [TestCase("chrome,50.0,Windows,10,1280x800")]
        [TestCase("chrome,1280x1024")]
        public void Parse_ShouldReturnDesktopBrowserConfiguration(string desktopBrowserConfiguration)
        {
            // Arrange
            var parts = desktopBrowserConfiguration.Split(',').Select(x => x.Replace("_", " ")).ToList();
            var browserName = parts[0];
            var browserVersion = "ANY";
            var osName = "ANY";
            var osVersion = "ANY";
            var resolution = "1024x768";
            if (parts.Count >= 4)
            {
                browserVersion = parts[1];
                osName = parts[2];
                osVersion = parts[3];
                if (parts.Count == 5)
                {
                    resolution = parts[4];
                }
            }
            else if (parts.Count == 2)
            {
                resolution = parts[1];
            }

            // Act 
            var result = _sut.Parse(desktopBrowserConfiguration);

            //Assert
            result.Should().Match(DesktopBrowserConfiguration(browserName, browserVersion, osName, osVersion, resolution));
        }

        [Test]
        public void Parse_ShouldThrowUnsupportedBrowserConfigurationExceptionWhenBrowserStackServiceIsSupportedReturnsFalse()
        {
            // Arrange

            BrowserConfiguration browserConfiguration = null;
            _browserStackService.IsNotSupported(Arg.Do<BrowserConfiguration>(x => browserConfiguration = x)).Returns(true);

            Action attemptToParseUnSupportedBrowserConfiguration = () => _sut.Parse("unknown,some device");

            //Assert
            var exception = attemptToParseUnSupportedBrowserConfiguration
                .ShouldThrow<UnsupportedBrowserException>()
                .WithMessage("some device is not supported").Which;

            exception.Browser.Should().BeSameAs(browserConfiguration);
        }

        [Test]
        public void Parse_ShouldReturnMobileBrowserConfiguration()
        {
            // Arrange
            const string browserName = "iPhone";
            const string device = "iPhone 6S Plus";
            var mobileBrowserConfiguration =$"{browserName},{device.Replace(" ","_")}";

            // Act 
            var result = _sut.Parse(mobileBrowserConfiguration);

            //Assert
            result.Should().Match(MobileBrowserConfiguration(browserName, device));
        }

        private static Expression<Func<BrowserConfiguration, bool>> DesktopBrowserConfiguration(string browserName, string browserVersion, string osName, string osVersion, string resolution = "1024x768")
        {
            return
                x =>
                    x.IsMobileDevice == false && x.Name == browserName && x.Version == browserVersion &&
                    x.OsName == osName && x.OsVersion == osVersion && x.Resolution == resolution;
        }

        private static Expression<Func<BrowserConfiguration, bool>> MobileBrowserConfiguration(string browserName, string device)
        {
            return
                x =>
                    x.IsMobileDevice && x.Name == browserName && x.Device == device;
        }
    }
}