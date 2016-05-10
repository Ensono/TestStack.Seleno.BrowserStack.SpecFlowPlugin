using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using OpenQA.Selenium;
using TestStack.Seleno.BrowserStack.Core.Extensions;

namespace TestStack.Seleno.Browserstack.UnitTests.Core.Extensions
{
    [TestFixture]
    public class WebDriverExtensionsSpecs
    {
        private IWebDriver _sut;
        private IOptions _options;
        private IWindow _windows;

        [SetUp]
        public void SetUp()
        {
            _sut = Substitute.For<IWebDriver>();
            _options = Substitute.For<IOptions>();
            _windows = Substitute.For<IWindow>();

            _sut.Manage().Returns(_options);
            _options.Window.Returns(_windows);
        }

        [Test]
        public void IsDisplayedOnNarrowedWidth_ShouldReturnFalseWhenWebDriverIsNull()
        {
            // Arrange
            IWebDriver sut = null;
            const int anyWidthBreakPoint = 0;

            // Act 
            var isDisplayedOnNarrowedWidth = sut.IsDisplayedOnNarrowedWidth(anyWidthBreakPoint);

            //Assert
            isDisplayedOnNarrowedWidth.Should().BeFalse();
        }

 

        [TestCase(1024,768, 1024)]
        [TestCase(1024,768, 1200)]
        public void IsDisplayedOnNarrowedWidth_ShouldReturnTrueWhenDeviceIsNotMobileAndResolutionWithBelowSpecifiedNarrowdWidthThreshold(int width, int length, int narrowedWidthThreshold)
        {
            // Arrange
            _windows.Size.Returns(new Size(width, length));

            // Act
            var isDisplayedOnNarrowedWidth = _sut.IsDisplayedOnNarrowedWidth(narrowedWidthThreshold);

            // Assert
            isDisplayedOnNarrowedWidth.Should().BeTrue();
        }
    }
}