using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace TestStack.Seleno.BrowserStack.Core.Services.Client
{
    public class HttpClientWrapper : Disposable, IHttpClient
    {
        private readonly HttpMessageInvoker _client;

        public MediaTypeFormatter[] GetFormatters()
        {
            return new[]
            {
                new JsonMediaTypeFormatter
                {
                    UseDataContractJsonSerializer = false,
                    SerializerSettings = new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver(),
                        NullValueHandling = NullValueHandling.Ignore,
                    }
                }
            };
        }


        public HttpClientWrapper(HttpMessageInvoker client)
        {
            _client = client;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _client != null)
            {
                _client.Dispose();
            }

            base.Dispose(disposing);
        }


        public Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            return _client.SendAsync(CreateRequestMessage(HttpMethod.Get, requestUri), CancellationToken.None);
        }

        public virtual HttpRequestMessage CreateRequestMessage(HttpMethod httpMethod, string requestUri)
        {
            return new HttpRequestMessage(HttpMethod.Get, requestUri);
        }

        public Task<HttpResponseMessage> PutAsJsonAsync<T>(string requestUri, T data)
        {
            var putRequestMessage = CreateRequestMessage(HttpMethod.Put, requestUri);
            var formatter = GetFormatters()[0];

            putRequestMessage.Content = new ObjectContent<T>(data, formatter);

            return _client.SendAsync(putRequestMessage, CancellationToken.None);
        }
    }
}