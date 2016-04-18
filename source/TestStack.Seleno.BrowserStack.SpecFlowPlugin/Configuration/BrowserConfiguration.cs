namespace TestStack.Seleno.BrowserStack.SpecFlowPlugin.Configuration
{
    public class BrowserConfiguration
    {
        public string Name { get; private set; }
        public string Device { get; private set; }
        public string Version { get; private set; }

        public string OsName { get; private set; }

        public string OsVersion { get; private set; }

        public bool IsMobileDevice { get; private set; }


        public BrowserConfiguration(string name = "ANY", string version = "ANY", string osName = "ANY",
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
    }
}