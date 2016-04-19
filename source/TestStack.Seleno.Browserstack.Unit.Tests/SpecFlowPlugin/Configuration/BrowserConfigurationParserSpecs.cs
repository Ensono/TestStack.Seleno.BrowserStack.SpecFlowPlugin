using System;
using System.Linq.Expressions;
using FluentAssertions;
using NUnit.Framework;
using TestStack.Seleno.BrowserStack.SpecFlowPlugin.Configuration;
using TestStack.Seleno.BrowserStack.SpecFlowPlugin.Exceptions;

namespace TestStack.Seleno.Browserstack.SpecFlowPlugin.Tests.Configuration
{
    [TestFixture]
    public class BrowserConfigurationParserSpecs
    {
        private BrowserConfigurationParser _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new BrowserConfigurationParser();
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

        [TestCase("chrome")]
        [TestCase("firefox,43.0,Windows")]
        [TestCase("firefox,43.0,OS_X,Lion,Other")]
        [TestCase("android,Samsung_Galaxy_S5,45")]
        [TestCase("android|Samsung_Galaxy_S5")]
        [TestCase("firefox 43.0 OS_X Lion")]
        public void Parse_ShouldThrowAnInvalidBrowserConfigurationExceptionWhenTheBrowserConfigurationHasNeitherTwoNorFourParts(string invalidBrowserConfig)
        {
            // Arrange
            Action attemptToParseInvalidBrowserConfiguration = () => _sut.Parse(invalidBrowserConfig);
            const string errorMessage = "Browser configuration is expected to contain browser name, browser version, " +
                                        "operating system and operating system version or browser and device name";

            // Act & Assert
            attemptToParseInvalidBrowserConfiguration
                    .ShouldThrow<InvalidBrowserConfigurationException>()
                    .WithMessage(errorMessage);
        }
        
        [TestCase("firefox,wrong_version,OS_X,Lion")]
        [TestCase("firefox,46abc,OS_X,Lion")]
        [TestCase("firefox,0x4564,OS_X,Lion")]
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

        [Test]
        public void Parse_ShouldReturnDesktopBrowserConfiguration()
        {
            // Arrange
            const string browserName = "firefox";
            const string browserVersion = "43.0";
            const string osName = "OS X";
            const string osVersion = "Lion";
            var desktopBrowserConfiguration =
                $"{browserName},{browserVersion},{osName.Replace(" ","_")},{osVersion}";

            // Act 
            var result = _sut.Parse(desktopBrowserConfiguration);

            //Assert
            result
                .Should()
                .Match(DesktopBrowserConfiguration(browserName, browserVersion, osName, osVersion));
        }

        [Test]
        public void Parse_ShouldReturnMobileBrowserConfiguration()
        {
            // Arrange
            const string browserName = "iphone";
            const string device = "iPhone 6S Plus";
            var mobileBrowserConfiguration =$"{browserName},{device}";

            // Act 
            var result = _sut.Parse(mobileBrowserConfiguration);

            //Assert
            result.Should().Match(MobileBrowserConfiguration(browserName, device));
        }

        private static Expression<Func<BrowserConfiguration, bool>> DesktopBrowserConfiguration(string browserName, string browserVersion, string osName, string osVersion)
        {
            return
                x =>
                    x.IsMobileDevice == false && x.Name == browserName && x.Version == browserVersion &&
                    x.OsName == osName && x.OsVersion == osVersion;
        }

        private static Expression<Func<BrowserConfiguration, bool>> MobileBrowserConfiguration(string browserName, string device)
        {
            return
                x =>
                    x.IsMobileDevice && x.Name == browserName && x.Device == device;
        }
    }
}