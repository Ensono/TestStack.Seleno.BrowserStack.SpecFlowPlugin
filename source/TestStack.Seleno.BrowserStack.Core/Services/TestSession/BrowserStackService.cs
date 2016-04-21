using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TestStack.Seleno.BrowserStack.Core.Configuration;
using TestStack.Seleno.BrowserStack.Core.Services.Client;

namespace TestStack.Seleno.BrowserStack.Core.Services.TestSession
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

        private static string GetSessionRelativeUrl(string sessionId)
        {
            return string.Format("sessions/{0}.json", sessionId);
        }

    }

}