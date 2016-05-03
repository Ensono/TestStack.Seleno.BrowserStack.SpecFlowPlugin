using System;
using Newtonsoft.Json;

namespace TestStack.Seleno.BrowserStack.Core.Configuration
{
    public class BrowserConfiguration
    {
        public const string Any = "ANY";
        public const string DefaultDesktopResolution = "1024x768";

        [JsonProperty("browser")]
        public string Name { get; set; }

        public string Device { get; set; }

        [JsonProperty("browser_version")]
        public string Version { get; set; }

        [JsonProperty("os")]
        public string OsName { get; set; }

        [JsonProperty("os_version")]
        public string OsVersion { get; set; }

        public string Resolution { get; set; }

        public bool IsMobileDevice
        {
            get { return !string.IsNullOrWhiteSpace(Device); }
        }

        [JsonConstructor]
        public BrowserConfiguration() : this(Any)
        {
        }

        public BrowserConfiguration(string name, string version = Any, string osName = Any,
            string osVersion = Any, string resolution = DefaultDesktopResolution)
        {
            Name = name;
            Version = version;
            OsName = osName;
            OsVersion = osVersion;
            Resolution = resolution;
        }

        public BrowserConfiguration(string name, string device)
        {
            Name = name;
            Device = device;
        }

        public override string ToString()
        {
            var version = Version != Any ? " " + Version : string.Empty;
            var osName = OsName != Any ? $" on {OsName}": string.Empty;
            var osVersion = OsVersion != Any ? " " + OsVersion : string.Empty;

            return IsMobileDevice
                ? Device
                : $"{Name}{version}{osName}{osVersion}";
        }

        public override bool Equals(object obj)
        {
            var another = obj as BrowserConfiguration;
            if (another == null) return false;

            if (IsMobileDevice && another.IsMobileDevice)
            {
                return string.Equals(Device, another.Device, StringComparison.InvariantCultureIgnoreCase);
            }

            return
                string.Equals(Name, another.Name, StringComparison.InvariantCultureIgnoreCase) &&
                (another.Version == Any   || Version == Any   || string.Equals(Version, another.Version, StringComparison.InvariantCultureIgnoreCase)) &&
                (another.OsName == Any    || OsName == Any    || string.Equals(OsName, another.OsName, StringComparison.InvariantCultureIgnoreCase)) &&
                (another.OsVersion == Any || OsVersion == Any || string.Equals(OsVersion, another.OsVersion, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}