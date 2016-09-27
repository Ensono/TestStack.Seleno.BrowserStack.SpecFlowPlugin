using System;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using TestStack.Seleno.BrowserStack.Core.Configuration;
using TestStack.Seleno.BrowserStack.Core.Enums;
using TestStack.Seleno.BrowserStack.Core.Exceptions;

namespace TestStack.Seleno.Browserstack.UnitTests.Core.Configuration
{
    [TestFixture]
    public class  ConfigurationProviderSpecs
    {
        [Test]
        public void LocalBrowser_ShouldThrowExceptionWhenEnumCannotBeParsed()
        {
            // Arrange
            var sut = Substitute.ForPartsOf<ConfigurationProvider>();
            BrowserEnum? result;
            string browserName;

            sut.BrowserName.Returns("Unsupported");
            sut.When(x => browserName = x.BrowserName).DoNotCallBase();

            Action unsupportedActionBrowser = () => result = sut.LocalBrowser;

            // Act && Assert
            unsupportedActionBrowser
                .ShouldThrow<InvalidBrowserConfigurationException>()
                .WithMessage("useLocalBrowser - local browser configuration must be one of the following Chrome, Firefox, InternetExplorer, PhantomJs, Safari");
        }

        [Test]
        public void LocalBrowser_ShouldReturnNullWhenNoBrowserNameSpecified()
        {
            // Arrange
            var sut = Substitute.ForPartsOf<ConfigurationProvider>();
            string browserName;

            sut.BrowserName.Returns(null as string);
            sut.When(x => browserName = x.BrowserName).DoNotCallBase();

            var result = sut.LocalBrowser;

            // Act && Assert
            result.Should().BeNull();
        }

        [TestCase("Chrome", BrowserEnum.Chrome)]
        [TestCase("PhantomJs", BrowserEnum.PhantomJs)]
        [TestCase("Firefox", BrowserEnum.Firefox)]
        [TestCase("InternetExplorer", BrowserEnum.InternetExplorer)]
        [TestCase("Safari", BrowserEnum.Safari)]
        public void LocalBrowser_ShouldReturnSpecifiedBrowserType(string browserName, BrowserEnum expectedBrowserType)
        {
            // Arrange
            var sut = Substitute.ForPartsOf<ConfigurationProvider>();
            var t = string.Empty;

            sut.BrowserName.Returns(browserName);
            sut.When(x => t = x.BrowserName).DoNotCallBase();

            var result = sut.LocalBrowser;

            // Act && Assert
            result.Should().Be(expectedBrowserType);
        }

    }
}