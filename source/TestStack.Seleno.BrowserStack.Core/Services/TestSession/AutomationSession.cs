using Newtonsoft.Json;

namespace TestStack.Seleno.BrowserStack.Core.Services.TestSession
{
    public class AutomationSession
    {
        public string Browser { get; set; }

        public string Os { get; set; }

        public string Name { get; set; }

        public int? Duration { get; set; }

        public string Device { get; set; }

        [JsonProperty("project_name")]
        public string ProjectName { get; set; }

        public string Logs { get; set; }

        public SessionStatus Status { get; set; }

        [JsonProperty("hashed_id")]
        public string HashedId { get; set; }

        [JsonProperty("build_name")]
        public string  BuildName { get; set; }

        [JsonProperty("browser_url")]
        public string BrowserUrl { get; set; }

        [JsonProperty("video_url")]
        public string VideoUrl { get; set; }

        public string Reason { get; set; }

        [JsonProperty("browser_version")]
        public string BrowserVersion { get; set; }

        [JsonProperty("os_version")]
        public string OsVersion { get; set; }   
    }
}