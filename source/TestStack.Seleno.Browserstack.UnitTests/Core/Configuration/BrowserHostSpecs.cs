using System;
using System.Collections.ObjectModel;
using BoDi;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using TestStack.Seleno.BrowserStack.Core.Configuration;
using TestStack.Seleno.Configuration;
using TestStack.Seleno.Configuration.Contracts;

namespace TestStack.Seleno.Browserstack.UnitTests.Core.Configuration
{
    [TestFixture]
    public class BrowserHostSpecs
    {
        private IObjectContainer _container;
        private BrowserHost _sut;
        private SelenoHost _selenoHost;

        public interface IDummyWebDriver : IWebDriver, IHasSessionId { }

        [SetUp]
        public void SetUp()
        {
            _container = Substitute.For<IObjectContainer>();
            _selenoHost = new SelenoHost();
            _sut = Substitute.ForPartsOf<BrowserHost>(_selenoHost, _container);
        }

        [Test]
        public void NavigateToInitialPage_ShouldCreateAndNavigateToInitialPageAndRegisterThatPage()
        {
            // Arrange
            const string url = "http://localhost/somewhere";
            var page = new DummyPage();

            _sut.When(x => x.NavigateTo<DummyPage>(url)).DoNotCallBase();
            _sut.NavigateTo<DummyPage>(url).Returns(page);

            // Act
            _sut.NavigateToInitialPage<DummyPage>(url);

            // Assert
            _container.RegisterInstanceAs(page, typeof(DummyPage));
            page.Container.Should().BeSameAs(_container);
        }

        [Test]
        public void SessionId_ShouldReturnEmptyWhenApplicationIsNotInitialised()
        {
            // Act && Assert
            _sut.SessionId.Should().BeEmpty();
        }

        [Test]
        public void SessionId_ShouldReturnEmptyWhenDriverDoesNotImplementHasSessionId()
        {
            // Arrange
            var browser = Substitute.For<IWebDriver>();
            _selenoHost.Application = Substitute.For<ISelenoApplication>();
            _selenoHost.Application.Browser.Returns(browser);

            // Act && Assert
            _sut.SessionId.Should().BeEmpty();
        }

        [Test]
        public void SessionId_ShouldReturnRemotWebDriverSessionId()
        {
            // Arrange
            var sessionId = Guid.NewGuid().ToString();
            var browser = Substitute.For<IDummyWebDriver>();
            _selenoHost.Application = Substitute.For<ISelenoApplication>();
            _selenoHost.Application.Browser.Returns(browser);
            browser.SessionId.Returns(new SessionId(sessionId));


            // Act && Assert
            _sut.SessionId.Should().Be(sessionId);
        }
    }
}