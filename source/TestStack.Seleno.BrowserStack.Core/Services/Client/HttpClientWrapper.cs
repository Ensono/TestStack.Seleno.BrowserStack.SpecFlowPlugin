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
        internal HttpMessageInvoker Client { get; }

        public virtual MediaTypeFormatter[] GetFormatters()
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
            Client = client;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && Client != null)
            {
                Client.Dispose();
            }

            base.Dispose(disposing);
        }


        public Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            return Client.SendAsync(CreateRequestMessage(HttpMethod.Get, requestUri), CancellationToken.None);
        }

        public virtual HttpRequestMessage CreateRequestMessage(HttpMethod httpMethod, string requestUri)
        {
            return new HttpRequestMessage(httpMethod, requestUri);
        }

        public Task<HttpResponseMessage> PutAsJsonAsync<T>(string requestUri, T data)
        {
            var putRequestMessage = CreateRequestMessage(HttpMethod.Put, requestUri);
            var formatter = GetFormatters()[0];

            putRequestMessage.Content = new ObjectContent<T>(data, formatter);

            return Client.SendAsync(putRequestMessage, CancellationToken.None);
        }
    }
}