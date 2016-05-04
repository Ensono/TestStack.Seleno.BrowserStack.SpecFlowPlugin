using System.Web.Mvc;
using BoDi;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using OpenQA.Selenium;
using TestStack.Seleno.Browserstack.UnitTests.Core;
using TestStack.Seleno.PageObjects.Actions;

namespace TestStack.Seleno.Browserstack.UnitTests
{
    [TestFixture]
    public class PageSpecs
    {
        [Test]
        public void NavigateToUrl_ShouldNavigateToSpecifiedUrlCreateAndRegisterPage()
        {
            // Arrange
            var pageNavigator = Substitute.For<IPageNavigator>();
            var container = Substitute.For<IObjectContainer>();
            var sut= new TestPage(pageNavigator) { Container = container};
            const string url = "http://some/url";
            var page = new DummyPage();

            pageNavigator.To<DummyPage>(url).Returns(page);

            // Act

            sut.NavigateTo<DummyPage>(url);

            // Assert
            page.Container.Should().BeSameAs(container);
            container.RegisterInstanceAs(page, typeof(DummyPage));
        }

        [Test]
        public void NavigateBy_ShouldNavigateToSpecifiedSelectorCreateAndRegisterPage()
        {
            // Arrange
            var pageNavigator = Substitute.For<IPageNavigator>();
            var container = Substitute.For<IObjectContainer>();
            var sut = new TestPage(pageNavigator) { Container = container };
            const string url = "http://some/url";
            var bySomeClassName = By.ClassName("some-class-name");
            var page = new DummyPage();

            pageNavigator.To<DummyPage>(bySomeClassName).Returns(page);

            // Act

            sut.NavigateTo<DummyPage>(bySomeClassName);

            // Assert
            page.Container.Should().BeSameAs(container);
            container.RegisterInstanceAs(page, typeof(DummyPage));
        }


        [Test]
        public void NavigateByJQuery_ShouldNavigateToSpecifiedSelectorCreateAndRegisterPage()
        {
            // Arrange
            var pageNavigator = Substitute.For<IPageNavigator>();
            var container = Substitute.For<IObjectContainer>();
            var sut = new TestPage(pageNavigator) { Container = container };
            const string url = "http://some/url";
            var bySomeClassName = By.ClassName("some-class-name");
            var page = new DummyPage();
            var byJQuery = PageObjects.Locators.By.jQuery("li:contains('text')");

            pageNavigator.To<DummyPage>(byJQuery).Returns(page);

            // Act

            sut.NavigateTo<DummyPage>(byJQuery);

            // Assert
            page.Container.Should().BeSameAs(container);
            container.RegisterInstanceAs(page, typeof(DummyPage));
        }
    }
}