using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using TestStack.Seleno.BrowserStack.Core.Configuration;
using TestStack.Seleno.BrowserStack.Core.Services.Client;
using TestStack.Seleno.BrowserStack.Core.Services.TestSession;

namespace TestStack.Seleno.Browserstack.UnitTests.Core.TestSession
{
    [TestFixture]
    public class BrowserStackServiceSpecs
    {
        private BrowserStackService _sut;
        private IConfigurationProvider _configurationProvider;
        private IHttpClientFactory _clientFactory;

        private static readonly ObjectContent<List<BrowserConfiguration>> SupportedBrowsersContent =
            new ObjectContent<List<BrowserConfiguration>>(new List<BrowserConfiguration>
            {
                new BrowserConfiguration("iPad", "iPad Mini") { OsName = "ios", OsVersion = "9.0"},
                new BrowserConfiguration("chrome", "48.0", "Windows", "10"),
                new BrowserConfiguration("iphone", "iPhone 6S Plus"),
                new BrowserConfiguration("Android", "Samsumg S5 "),
                new BrowserConfiguration("firefox", "ANY","Windows","XP")
            }, new JsonMediaTypeFormatter());


        private static readonly BrowserConfiguration[] SupportedBrowsersTestCase =
        {
            new BrowserConfiguration("iPad", "iPad Mini"),
            new BrowserConfiguration("chrome", "48.0", "Windows", "10"),
            new BrowserConfiguration("firefox")
        };

        private static readonly BrowserConfiguration[] UnSupportedBrowsersTestCase =
        {
            new BrowserConfiguration("iPad", "IPad super holographic pro!"),
            new BrowserConfiguration("3DInternetExplorer", "1.0","VirtualWindows","1.0b"),
            new BrowserConfiguration("opera"),
        };



        [SetUp]
        public void SetUp()
        {
            _configurationProvider = Substitute.For<IConfigurationProvider>();
            _clientFactory = Substitute.For<IHttpClientFactory>();
            _sut = new BrowserStackService(_configurationProvider, _clientFactory);
        }

        [Test]
        public void GetSessionDetail_ShouldGetBrowserStackSessionDetailWhenStatusIsOk()
        {
            // Arrange
            var sessionId = Guid.NewGuid().ToString();
            const string baseAddress = "http://some/address";
            var client = Substitute.For<IHttpClient>();

            _clientFactory.Create(baseAddress).Returns(client);
            var session = new SessionDetail {  AutomationSession = new AutomationSession {Name = "blah"}};
            var content = new ObjectContent<SessionDetail>(session, new JsonMediaTypeFormatter());

            _configurationProvider.BrowserStackApiUrl.Returns(baseAddress);
            client
                .GetAsync($"sessions/{sessionId}.json")
                .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = content }));


            // Act
            var result = _sut.GetSessionDetail(sessionId);

            // Assert
            result.Should().BeSameAs(session.AutomationSession);
            client.Received().GetFormatters();
        }

        [TestCase(HttpStatusCode.Continue)]
        [TestCase(HttpStatusCode.SwitchingProtocols)]
        [TestCase(HttpStatusCode.Ambiguous)]
        [TestCase(HttpStatusCode.Unauthorized)]
        [TestCase(HttpStatusCode.BadRequest)]
        public void GetSessionDetail_ShouldGetBrowserStackSessionDetailWhenStatusIsNotOk(HttpStatusCode badStatusCode)
        {
            // Arrange
            var sessionId = Guid.NewGuid().ToString();
            const string baseAddress = "http://some/address";
            var client = Substitute.For<IHttpClient>();

            _clientFactory.Create(baseAddress).Returns(client);

            _configurationProvider.BrowserStackApiUrl.Returns(baseAddress);
            client
                .GetAsync($"sessions/{sessionId}.json")
                .Returns(Task.FromResult(new HttpResponseMessage(badStatusCode)));


            // Act
            var result = _sut.GetSessionDetail(sessionId);

            // Assert
            result.ShouldBeEquivalentTo(new AutomationSession());
            client.DidNotReceive().GetFormatters();
        }

        [TestCase(SessionStatus.Done)]
        [TestCase(SessionStatus.Error)]
        [TestCase(SessionStatus.Running)]
        public void UpdateTestStatus_ShouldUpdateSessionStatus(SessionStatus sessionStatus)
        {
            // Arrange
            const string baseAddress = "http://some/address";
            var sessionId = Guid.NewGuid().ToString();
            const string errorMessage = "Something bad happened!";
            var client = Substitute.For<IHttpClient>();

            _clientFactory.Create(baseAddress).Returns(client);
            _configurationProvider.BrowserStackApiUrl.Returns(baseAddress);


            // Act
            _sut.UpdateTestStatus(sessionId, sessionStatus, errorMessage);

            // Assert

            client.Received()
                .PutAsJsonAsync($"sessions/{sessionId}.json",
                                Arg.Is<SessionUpdate>(s => s.Status == sessionStatus.ToString().ToLower() && s.Reason == errorMessage));

        }

        [TestCaseSource(nameof(SupportedBrowsersTestCase))]
        public void IsNotSupported_ShouldReturnFalseWhenBrowserConfigurationCouldBeFound(BrowserConfiguration supportedBrowserConfiguration)
        {
            // Arrange
            const string baseAddress = "http://some/address";
            var client = Substitute.For<IHttpClient>();

            _clientFactory.Create(baseAddress).Returns(client);
            _configurationProvider.BrowserStackApiUrl.Returns(baseAddress);
            client.GetAsync("browsers.json")
                .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) {Content = SupportedBrowsersContent }));
            // Act

            var result = _sut.IsNotSupported(supportedBrowserConfiguration);

            // Assert
            result.Should().BeFalse();
        }

        [TestCaseSource(nameof(UnSupportedBrowsersTestCase))]
        public void IsNotSupported_ShouldReturnTrueWhenBrowserConfigurationCouldNotBeFound(BrowserConfiguration unsupportedBrowserConfiguration)
        {
            // Arrange
            const string baseAddress = "http://some/address";
            var client = Substitute.For<IHttpClient>();

            _clientFactory.Create(baseAddress).Returns(client);
            _configurationProvider.BrowserStackApiUrl.Returns(baseAddress);
            client
                .GetAsync("browsers.json")
                .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = SupportedBrowsersContent }));
            // Act

            var result = _sut.IsNotSupported(unsupportedBrowserConfiguration);

            // Assert
            result.Should().BeTrue();
        }

        [TestCase(HttpStatusCode.Continue)]
        [TestCase(HttpStatusCode.SwitchingProtocols)]
        [TestCase(HttpStatusCode.Ambiguous)]
        [TestCase(HttpStatusCode.Unauthorized)]
        [TestCase(HttpStatusCode.BadRequest)]
        public void IsNotSupported_ShouldReturnTrueWhenServiceResponseIsNotOk(HttpStatusCode httpStatusCode)
        {
            // Arrange
            const string baseAddress = "http://some/address";
            var client = Substitute.For<IHttpClient>();
            var anyBrowserConfiguration = new BrowserConfiguration();

            _clientFactory.Create(baseAddress).Returns(client);
            _configurationProvider.BrowserStackApiUrl.Returns(baseAddress);
            client
                .GetAsync("browsers.json")
                .Returns(Task.FromResult(new HttpResponseMessage(httpStatusCode)));
            // Act

            var result = _sut.IsNotSupported(anyBrowserConfiguration);

            // Assert
            result.Should().BeTrue();
        }
    }
}