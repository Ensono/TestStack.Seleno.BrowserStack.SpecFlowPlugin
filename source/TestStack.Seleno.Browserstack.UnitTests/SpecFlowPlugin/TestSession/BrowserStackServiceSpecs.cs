using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NSubstitute;
using NUnit.Framework;
using TestStack.Seleno.BrowserStack.Core.Configuration;
using TestStack.Seleno.BrowserStack.Core.Services.Client;
using TestStack.Seleno.BrowserStack.Core.Services.TestSession;

namespace TestStack.Seleno.Browserstack.UnitTests.SpecFlowPlugin.TestSession
{
    [TestFixture]
    public class BrowserStackServiceSpecs
    {
        private BrowserStackService _sut;
        private IConfigurationProvider _configurationProvider;
        private IHttpClientFactory _clientFactory;

        [SetUp]
        public void SetUp()
        {
            _configurationProvider = Substitute.For<IConfigurationProvider>();
            _clientFactory = Substitute.For<IHttpClientFactory>();
            _sut = new BrowserStackService(_configurationProvider, _clientFactory);
        }

        //[Test]
        //public void Formatters_ShouldReturnOnlyConfiguredJsonMediaTypeFormatter()
        //{
        //    BrowserStackService
        //        .Formatters
        //        .Should()
        //        .HaveCount(1)
        //        .And
        //        .OnlyContain(
        //            x =>
        //                !x.UseDataContractJsonSerializer &&
        //                x.SerializerSettings.ContractResolver is CamelCasePropertyNamesContractResolver &&
        //                x.SerializerSettings.NullValueHandling == NullValueHandling.Ignore);
        //}

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



    }
}