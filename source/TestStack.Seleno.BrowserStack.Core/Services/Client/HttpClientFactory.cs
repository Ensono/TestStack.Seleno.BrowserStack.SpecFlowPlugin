using System;
using System.Net.Http;
using System.Net.Http.Headers;
using TestStack.Seleno.BrowserStack.Core.Configuration;

namespace TestStack.Seleno.BrowserStack.Core.Services.Client
{
    public class HttpClientFactory : IHttpClientFactory
    {
        private readonly IConfigurationProvider _configurationProvider;

        public const string AuthorizationScheme = "Basic";
        public const string JsonMediaType = "application/json";

        public HttpClientFactory(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public IHttpClient Create(string baseAddress)
        {
            var client = new HttpClient { BaseAddress = new Uri(baseAddress)};
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(JsonMediaType));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthorizationScheme,
                _configurationProvider.Encoded64Token);
            return new HttpClientWrapper(client);
        }
    }
}