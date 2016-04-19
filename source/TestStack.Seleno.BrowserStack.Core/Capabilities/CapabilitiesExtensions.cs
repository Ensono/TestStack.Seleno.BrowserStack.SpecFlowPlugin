using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace TestStack.Seleno.BrowserStack.Core.Capabilities
{
    public static class CapabilitiesExtensions
    {
        public const string Any = "ANY";

        public static string BuildVersion(this ICapabilities capabilities)
        {
            var result = GetCapabilityAs<string>(capabilities, RemoteCapabilityType.BrowserStack.Build);
            return string.IsNullOrWhiteSpace(result) ? string.Empty : result;
        }

        public static string Project(this ICapabilities capabilities)
        {
            var result = GetCapabilityAs<string>(capabilities, RemoteCapabilityType.BrowserStack.Project);
            return string.IsNullOrWhiteSpace(result) ? string.Empty : result;
        }
      
        public static bool IsSimilarTo(this ICapabilities capabilities, ICapabilities another, string[] keys = null)
        {
            var thisCapabilities = capabilities as DesiredCapabilities;
            var anotherCapabilities = another as DesiredCapabilities;


            if (thisCapabilities == null || anotherCapabilities == null) return false;

            return
                thisCapabilities.BrowserName.Equals(anotherCapabilities.BrowserName,
                    StringComparison.InvariantCultureIgnoreCase) &&
                (thisCapabilities.Version.StartsWith(anotherCapabilities.Version) ||
                 anotherCapabilities.Version.StartsWith(thisCapabilities.Version)) &&
                thisCapabilities.Platform.IsPlatformType(anotherCapabilities.Platform.PlatformType) &&
                BuildVersion(thisCapabilities)
                    .Equals(BuildVersion(anotherCapabilities), StringComparison.InvariantCultureIgnoreCase) &&
                Project(thisCapabilities)
                    .Equals(Project(anotherCapabilities), StringComparison.InvariantCultureIgnoreCase);
        }


        public static T GetCapabilityAs<T>(this ICapabilities capabilities, string key) where T : class
        {
            if (capabilities == null)
            {
                return default(T);
            }
            return capabilities.GetCapability(key) as T;
        }
    }
}