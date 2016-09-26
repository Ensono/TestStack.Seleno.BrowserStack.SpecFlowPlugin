using System.Collections.Generic;
using System.Net.Http;
using TestStack.Seleno.BrowserStack.Core.Configuration;
using TestStack.Seleno.BrowserStack.Core.Services.Client;
using TestStack.Seleno.BrowserStack.Core.Services.TestSession;

namespace TestStack.Seleno.BrowserStack.Core.Services.BrowserStack
{
    public class BrowserStackService : IBrowserStackService
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IHttpClientFactory _clientFactory;

        public BrowserStackService(IConfigurationProvider configurationProvider, IHttpClientFactory clientFactory)
        {
            _configurationProvider = configurationProvider;
            _clientFactory = clientFactory;
        }

        public virtual AutomationSession GetSessionDetail(string sessionId)
        {
            var result = new SessionDetail();
            using (var client = _clientFactory.Create(_configurationProvider.BrowserStackApiUrl))
            {
                var response = client.GetAsync(GetSessionRelativeUrl(sessionId)).Result;
                if (response.IsSuccessStatusCode)
                {
                    result = response.Content.ReadAsAsync<SessionDetail>(client.GetFormatters()).Result;
                }
            }
            return result.AutomationSession;
        }

        public virtual void UpdateTestStatus(string sessionId, SessionStatus status, string reason)
        {
            using (var client = _clientFactory.Create(_configurationProvider.BrowserStackApiUrl))
            {
                client.PutAsJsonAsync(GetSessionRelativeUrl(sessionId), new SessionUpdate(status, reason)).Wait();
            }
        }

        public bool IsNotSupported(BrowserConfiguration browserConfiguration)
        {
            using (var client = _clientFactory.Create(_configurationProvider.BrowserStackApiUrl))
            {
                var response = client.GetAsync("browsers.json").Result;
                if (response.IsSuccessStatusCode)
                {
                    var supportedBrowsers =
                        response
                            .Content
                            .ReadAsAsync<List<BrowserConfiguration>>(client.GetFormatters()).Result;

                    return !supportedBrowsers.Exists(b => Equals(b, browserConfiguration));
                }
            }
            return true;
        }

        private static string GetSessionRelativeUrl(string sessionId)
        {
            return string.Format("sessions/{0}.json", sessionId);
        }

    }
}