using FluentAssertions;
using NUnit.Framework;
using TestStack.Seleno.BrowserStack.Core.Configuration;

namespace TestStack.Seleno.Browserstack.UnitTests.Core.Configuration
{
    [TestFixture]
    public class BrowserConfigurationSpecs
    {
        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void IsMobileDevice_ShouldReturnFalseWhenDeviceIsNullOrEmptyOrWhiteSpace(string device)
        {
            // Arrange
            const string anyBrowserName = "chrome";
            var sut = new BrowserConfiguration(anyBrowserName, device);

            // Act && Assert
            sut.IsMobileDevice.Should().BeFalse();
        }


        [TestCase("samsung")]
        [TestCase("anything")]
        public void IsMobileDevice_ShouldReturnTrueWhenDeviceNotIsNullOrEmptyOrWhiteSpace(string device)
        {
            // Arrange
            const string anyBrowserName = "chrome";
            var sut = new BrowserConfiguration(anyBrowserName, device);

            // Act && Assert
            sut.IsMobileDevice.Should().BeTrue();
        }
    }
}