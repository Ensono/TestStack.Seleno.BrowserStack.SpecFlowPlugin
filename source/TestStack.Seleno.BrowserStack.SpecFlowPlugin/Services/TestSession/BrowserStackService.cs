using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TestStack.Seleno.BrowserStack.SpecFlowPlugin.Configuration;

namespace TestStack.Seleno.BrowserStack.SpecFlowPlugin.Services.TestSession
{
    public class BrowserStackService : IBrowserStackService
    {
        private readonly IConfigurationProvider _configurationProvider;

        public BrowserStackService(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public virtual AutomationSession GetSessionDetail(string sessionId)
        {
            var result = new SessionDetail();
            using (var client = CreateAuthenticatedClient())
            {
                var response = client.GetAsync(GetRequestUri(sessionId)).Result;
                if (response.IsSuccessStatusCode)
                {
                    result = response.Content.ReadAsAsync<SessionDetail>(Formatters).Result;
                }
            }

            return result.AutomationSession;
        }

        private static JsonMediaTypeFormatter[] Formatters
        {
            get
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
        }

        public virtual void UpdateTestStatus(string sessionId, SessionStatus status, string reason)
        {
            using (var client = CreateAuthenticatedClient())
            {
                client.PutAsJsonAsync(GetRequestUri(sessionId), new {status = status.ToString().ToLower(), reason}).Wait();
            }
        }

        private HttpClient CreateAuthenticatedClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                _configurationProvider.Encoded64Token);

            return client;
        }

        private static string GetRequestUri(string sessionId)
        {
            return string.Format("https://www.browserstack.com/automate/sessions/{0}.json", sessionId);
        }
    }

}