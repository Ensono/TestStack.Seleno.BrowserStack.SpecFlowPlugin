using System;
using System.Configuration;
using System.Text;
using TestStack.Seleno.BrowserStack.Core.Capabilities;

namespace TestStack.Seleno.BrowserStack.Core.Configuration
{
    public class ConfigurationProvider : IConfigurationProvider
    {
        public string UserName
        {
            get { return ConfigurationManager.AppSettings[RemoteCapabilityType.BrowserStack.UserName]; }
        }

        public string AccessKey
        {
            get { return ConfigurationManager.AppSettings[RemoteCapabilityType.BrowserStack.AccessKey]; }
        }

        public string Encoded64Token
        {
            get
            {
                return Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", UserName, AccessKey)));
            }
        }

        public string RemoteUrl
        {
            get { return ConfigurationManager.AppSettings[Constants.BrowserStackSeleniumHubUrl]; }
        }

        public string BuildNumber
        {
            get { return ConfigurationManager.AppSettings[Constants.BuildNumber]; }
        }
    }
}