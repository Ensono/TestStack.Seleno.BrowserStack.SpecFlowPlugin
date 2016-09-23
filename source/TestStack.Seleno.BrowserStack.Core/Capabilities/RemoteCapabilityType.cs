using System.Security.Cryptography.X509Certificates;
using OpenQA.Selenium.Remote;

namespace TestStack.Seleno.BrowserStack.Core.Capabilities
{
    public static class RemoteCapabilityType
    {
        public static class Default
        {
            public static readonly string BrowserName = CapabilityType.BrowserName;
            public static readonly string Version = CapabilityType.Version;
            public static readonly string Platform = CapabilityType.Platform;
        }

        public static class BrowserStack
        {
            public static readonly string UserName = "browserstack.user";
            public static readonly string AccessKey = "browserstack.key";
            public static readonly string BrowserVersion = "browser_version";
            public static readonly string Os = "os";
            public static readonly string OsVersion = "os_version";
            public static readonly string ScreenResolution = "resolution";
            public static readonly string Device = "device";
            public static readonly string Build = "build";
            public static readonly string Project = "project";
            public static readonly string TestName = "name";
            public static readonly string Debug = "browserstack.debug";
            public static readonly string Video = "browserstack.video";
            public static readonly string RunLocally = "browserstack.local";
        }
    }
}