using Newtonsoft.Json;

namespace TestStack.Seleno.BrowserStack.SpecFlowPlugin.Services.TestSession
{
    public class SessionDetail
    {
        public SessionDetail()
        {
            AutomationSession = new AutomationSession();
        }

        [JsonProperty("automation_session")]
        public AutomationSession AutomationSession { get; set; }
    }
}