using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using TestStack.Seleno.BrowserStack.Core.Capabilities;
using TestStack.Seleno.BrowserStack.Core.Enums;
using TestStack.Seleno.BrowserStack.Core.Exceptions;

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

        public string BrowserStackApiUrl
        {
            get { return ConfigurationManager.AppSettings[Constants.BrowserStackApiUrl]; }
        }

        public BrowserEnum? LocalBrowser
        {
            get
            {
                if (string.IsNullOrWhiteSpace(BrowserName)) return null;
                
                if (!Enum.IsDefined(typeof(BrowserEnum), BrowserName))
                {
                    var supportedBrowserTypes = string.Join(", ", Enum.GetValues(typeof(BrowserEnum)).Cast<BrowserEnum>().Select(e => e.ToString()).OrderBy(n => n));
                    throw new InvalidBrowserConfigurationException($"useLocalBrowser - local browser configuration must be one of the following {supportedBrowserTypes}");
                }

                BrowserEnum result;
                return Enum.TryParse(BrowserName.Replace(" ", string.Empty), out result) ? result : null as BrowserEnum?;
            }
        }

       

        public IDictionary<string, object> Capabilities
        {
            get
            {
                return 
                    CapabilityCollection
                        .Cast<string>()
                        .ToDictionary(key => key, key => (object)((NameValueCollection)ConfigurationManager.GetSection(Constants.Capabilities))[key]);
            }
        }

        public bool RunTestLocally
        {
            get
            {
                bool result;
                return bool.TryParse(ConfigurationManager.AppSettings[Constants.RunTestLocally], out result) && result && (LocalBrowser == null);
            }
        }

        internal virtual NameValueCollection CapabilityCollection
        {
            get { return ((NameValueCollection)ConfigurationManager.GetSection(Constants.Capabilities)); }
        }

        internal virtual string BrowserName
        {
            get { return ConfigurationManager.AppSettings[Constants.UseLocalBrowser]; }
        }
    }
}