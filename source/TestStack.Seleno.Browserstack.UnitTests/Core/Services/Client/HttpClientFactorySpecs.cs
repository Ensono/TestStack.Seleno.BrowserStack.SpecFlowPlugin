using System;
using System.Net.Http;
using System.Net.Http.Headers;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using TestStack.Seleno.BrowserStack.Core.Configuration;
using TestStack.Seleno.BrowserStack.Core.Services.Client;
using HttpClientFactory = TestStack.Seleno.BrowserStack.Core.Services.Client.HttpClientFactory;

namespace TestStack.Seleno.Browserstack.UnitTests.Core.Services.Client
{
    [TestFixture]
    public class HttpClientFactorySpecs
    {
        private IConfigurationProvider _configurationProvider;
        private HttpClientFactory _sut;

        [SetUp]
        public void SetUp()
        {
            _configurationProvider = Substitute.For<IConfigurationProvider>();
            _sut = new HttpClientFactory(_configurationProvider);

        }

        [Test]
        public void Create_ShouldReturnAnInstanceOfHttpClientWrapper()
        {
            // Arrange
            const string baseAddress = "http://base/address";
            var encodedAccessKey = Guid.NewGuid().ToString();
            _configurationProvider.Encoded64Token.Returns(encodedAccessKey);

            // Act
            var result = _sut.Create(baseAddress);

            // Assert
            var returnedClient = result.Should().NotBeNull().And.BeOfType<HttpClientWrapper>().Subject;
            var internalClient = returnedClient.Client.Should().BeOfType<HttpClient>().Subject;

            internalClient.DefaultRequestHeaders.Accept.Should().Contain(m => m.MediaType == "application/json");
            internalClient.DefaultRequestHeaders.Authorization.ShouldBeEquivalentTo(new AuthenticationHeaderValue("Basic", encodedAccessKey));
        }


    }
}