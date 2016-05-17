
using BoDi;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using TestStack.Seleno.BrowserStack.Core;
using TestStack.Seleno.BrowserStack.Core.Extensions;
using TestStack.Seleno.BrowserStack.Core.Pages;

namespace TestStack.Seleno.Browserstack.UnitTests.Core
{
    [TestFixture]
    public class PageProviderSpecs
    {
        public class DummyPage : Page { }

        [TestCase(null)]
        [TestCase("namedPage")]
        public void GetPage_ShouldResolveAndReturnPageSpecifiedPageType(string name)
        {
            // Arrange
            var container = Substitute.For<IObjectContainer>();
            var expectedPage = new DummyPage();
            container.Resolve<DummyPage>(name).Returns(expectedPage);
            PageProvider.Container = container;

            // Act
            var result = PageProvider.GetPage<DummyPage>(name);

            // Assert
            result.Should().BeSameAs(expectedPage);
        }

        [Test]
        public void AndRegister_ShouldRegisterThePageAndItsBaseType()
        {
            // Arrange
            var container = Substitute.For<IObjectContainer>();
            var sut = new DummyPage();
            PageProvider.Container = container;

            // Act
            sut.AndRegister();

            // Assert
            container.Received(1).RegisterInstanceAs(sut, typeof(DummyPage));
            container.Received(1).RegisterInstanceAs(sut, typeof(Page));
        }
    }
}