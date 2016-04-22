using System;
using Newtonsoft.Json;

namespace TestStack.Seleno.BrowserStack.Core.Configuration
{
    public class BrowserConfiguration
    {
        [JsonProperty("browser")]
        public string Name { get; set; }

        public string Device { get; set; }

        [JsonProperty("browser_version")]
        public string Version { get; set; }

        [JsonProperty("os")]
        public string OsName { get; set; }

        [JsonProperty("os_version")]
        public string OsVersion { get; set; }

        public bool IsMobileDevice { get; set; }

        [JsonConstructor]
        public BrowserConfiguration() : this("ANY")
        {
        }

        public BrowserConfiguration(string name, string version = "ANY", string osName = "ANY",
            string osVersion = "ANY")
        {
            Name = name;
            Version = version;
            OsName = osName;
            OsVersion = osVersion;
        }

        public BrowserConfiguration(string name, string device)
        {
            Name = name;
            Device = device;
            IsMobileDevice = true;
        }

        public override string ToString()
        {
            return IsMobileDevice
                ? Device
                : $"{Name} {Version} on {OsName} {OsVersion}";
        }

        public override bool Equals(object obj)
        {
            var another = obj as BrowserConfiguration;
            if (another == null) return false;

            return
                string.Equals(Name, another.Name, StringComparison.InvariantCultureIgnoreCase) &&
                string.Equals(Device, another.Device, StringComparison.InvariantCultureIgnoreCase) &&
                string.Equals(Version, another.Version, StringComparison.InvariantCultureIgnoreCase) &&
                string.Equals(OsName, another.OsName, StringComparison.InvariantCultureIgnoreCase) &&
                string.Equals(OsVersion, another.OsVersion, StringComparison.InvariantCultureIgnoreCase) &&
                IsMobileDevice == another.IsMobileDevice;
        }
    }
}