using System;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NSubstitute;
using NUnit.Framework;
using TestStack.Seleno.BrowserStack.Core.Services.Client;
using TestStack.Seleno.BrowserStack.Core.Services.TestSession;

#pragma warning disable 4014

namespace TestStack.Seleno.Browserstack.UnitTests.Core.Services.Client
{
    [TestFixture]
    public class HttpClientWrapperSpecs
    {
        private HttpMessageInvoker _client;
        private HttpClientWrapper _sut;

        [SetUp]
        public void SetUp()
        {
            _client = Substitute.For<HttpMessageInvoker>(Substitute.For<HttpMessageHandler>());
            _sut = new HttpClientWrapper(_client);
        }

        [Test]
        public void Client_ShouldBeSameAsClientInjectedWhenConstructed()
        {
            _sut.Client.Should().BeSameAs(_client);
        }


        [Test]
        public void Formatters_ShouldReturnOnlyConfiguredJsonMediaTypeFormatter()
        {
            // Act
            var formatters = _sut.GetFormatters();

            // Assert
            formatters
                .Should()
                .OnlyContain(OneJsonMediaTypeFormatterWithCamelCasePropertyNamesContractResolverAndIgnoreNullValue);
        }


        [Test]
        public async Task GetAsync_ShouldSendAsyncGetRequestMessage()
        {
            // Arrange
            const string requestUri = "http://some/url";
            var getRequest = new HttpRequestMessage(HttpMethod.Get, requestUri);

            var sut = Substitute.ForPartsOf<HttpClientWrapper>(_client);
            var responseMessage = new HttpResponseMessage();

            sut.WhenForAnyArgs(x => x.CreateRequestMessage(HttpMethod.Get, requestUri)).DoNotCallBase();
            sut.CreateRequestMessage(Arg.Is(HttpMethod.Get), Arg.Is(requestUri)).Returns(getRequest);
            _client.SendAsync(getRequest, CancellationToken.None).Returns(Task.FromResult(responseMessage));

            // Act
            var result   = await sut.GetAsync(requestUri);

            // Assert
            result.Should().BeSameAs(responseMessage);
        }

        [Test]
        public async Task PutAsJsonAsync_ShouldSendAsyncPutRequestMessage()
        {
            // Arrange
            const string requestUri = "http://some/url";
            var putRequest = new HttpRequestMessage(HttpMethod.Put, requestUri);

            var sut = Substitute.ForPartsOf<HttpClientWrapper>(_client);
            var responseMessage = new HttpResponseMessage();
            var data = new SessionUpdate(SessionStatus.Error, "something bad happened!");
            sut.WhenForAnyArgs(x => x.CreateRequestMessage(HttpMethod.Put, requestUri)).DoNotCallBase();
            sut.CreateRequestMessage(Arg.Is(HttpMethod.Put), Arg.Is(requestUri)).Returns(putRequest);
            var formatter = new JsonMediaTypeFormatter();
            sut.GetFormatters().Returns(new MediaTypeFormatter[] {formatter});
            _client.SendAsync(putRequest, CancellationToken.None).Returns(Task.FromResult(responseMessage));

            // Act
            var result = await sut.PutAsJsonAsync(requestUri, data);

            // Assert
            result.Should().BeSameAs(responseMessage);
            var content = putRequest.Content.Should().BeOfType<ObjectContent<SessionUpdate>>().Subject;
            content.Value.Should().BeSameAs(data);
            content.Formatter.Should().BeSameAs(formatter);
        }

        [Test]
        public void CreateRequestMessage_ShouldCreateHttpRequestMessageWithSpecifiedHttpMethodAndRequestUri()
        {
            const string requestUri = "http://some/url";
            var httpMethod = HttpMethod.Get;

            // Act
            var result = _sut.CreateRequestMessage(httpMethod, requestUri);

            // Assert
            result.ShouldBeEquivalentTo(new HttpRequestMessage(httpMethod, requestUri));
        }


        private Expression<Func<MediaTypeFormatter, bool>> OneJsonMediaTypeFormatterWithCamelCasePropertyNamesContractResolverAndIgnoreNullValue
        {
            get
            {
                return
                    x => x is JsonMediaTypeFormatter && !((JsonMediaTypeFormatter)x).UseDataContractJsonSerializer &&
                         ((JsonMediaTypeFormatter)x).SerializerSettings.ContractResolver is CamelCasePropertyNamesContractResolver &&
                         ((JsonMediaTypeFormatter)x).SerializerSettings.NullValueHandling == NullValueHandling.Ignore;

            }
        }
    }
}