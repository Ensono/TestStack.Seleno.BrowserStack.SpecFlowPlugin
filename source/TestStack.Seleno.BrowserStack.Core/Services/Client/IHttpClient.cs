using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;

namespace TestStack.Seleno.BrowserStack.Core.Services.Client
{
    public interface IHttpClient : IDisposable
    {
        Task<HttpResponseMessage> GetAsync(string requestUri);

        Task<HttpResponseMessage> PutAsJsonAsync<T>(string requestUri, T data);

        MediaTypeFormatter[] GetFormatters();
    }
}