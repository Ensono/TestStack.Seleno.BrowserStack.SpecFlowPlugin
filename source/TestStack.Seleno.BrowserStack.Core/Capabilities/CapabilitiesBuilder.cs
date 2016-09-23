using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using TestStack.Seleno.BrowserStack.Core.Configuration;

namespace TestStack.Seleno.BrowserStack.Core.Capabilities
{
    public class CapabilitiesBuilder : ICapabilitiesBuilder
    {
        private readonly IConfigurationProvider _configurationProvider;
        private TestSpecification _testSpecification = new TestSpecification(string.Empty, string.Empty);
        private string _userName = string.Empty;
        private string _accessKey = string.Empty;
        private BrowserConfiguration _browserConfiguration = new BrowserConfiguration();
        private string _buildNumber = string.Empty;
        private readonly Dictionary<string, object> _additionalCapabilities = new Dictionary<string, object>();

        public CapabilitiesBuilder(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
            WithAdditionalCapabilities(configurationProvider.Capabilities);
        }

        public ICapabilitiesBuilder WithTestSpecification(TestSpecification testSpecification)
        {
            _testSpecification = testSpecification;
            return this;
        }

        public ICapabilitiesBuilder WithCredentials(string userName, string accessKey)
        {
            _userName = userName;
            _accessKey = accessKey;
            return this;
        }

        public ICapabilitiesBuilder WithBrowserConfiguration(BrowserConfiguration browserConfiguration)
        {
            _browserConfiguration = browserConfiguration;
            return this;
        }

        public ICapabilitiesBuilder WithBuildNumber(string buildNumber)
        {
            _buildNumber = buildNumber;
            return this;
        }

        public ICapabilitiesBuilder WithAdditionalCapabilities(IDictionary<string, object> additionalCapabilities)
        {
            foreach (var additionalCapability in additionalCapabilities)
            {
                if (!_additionalCapabilities.ContainsKey(additionalCapability.Key))
                {
                    _additionalCapabilities.Add(additionalCapability.Key, additionalCapability.Value);
                }
            }
            return this;
        }

        public ICapabilities Build()
        {
            var capabilities = new DesiredCapabilities(_additionalCapabilities);
            
            SetCredentials(capabilities);

            SetTestConfiguration(capabilities);

            SetBrowserConfiguration(capabilities);

            return capabilities;
        }

        private void SetBrowserConfiguration(DesiredCapabilities capabilities)
        {
            capabilities.SetCapability(RemoteCapabilityType.Default.BrowserName, _browserConfiguration.Name);

            if (_browserConfiguration.IsMobileDevice)
            {
                capabilities.SetCapability(RemoteCapabilityType.BrowserStack.Device, _browserConfiguration.Device);
            }
            else
            {
                capabilities.SetCapability(RemoteCapabilityType.Default.Version, _browserConfiguration.Version);
                capabilities.SetCapability(RemoteCapabilityType.BrowserStack.Os, _browserConfiguration.OsName);
                capabilities.SetCapability(RemoteCapabilityType.BrowserStack.OsVersion, _browserConfiguration.OsVersion);

                PlatformType platformType;
                if (Enum.TryParse(_browserConfiguration.OsName, true, out platformType))
                {
                    var platform = new Platform(platformType);
                    capabilities.SetCapability(RemoteCapabilityType.Default.Platform, platform);
                }
                capabilities.SetCapability(RemoteCapabilityType.BrowserStack.ScreenResolution, _browserConfiguration.Resolution);
            }
        }

        private void SetTestConfiguration(DesiredCapabilities capabilities)
        {
            capabilities.SetCapability(RemoteCapabilityType.BrowserStack.TestName, _testSpecification.ScenarioTitle);
            capabilities.SetCapability(RemoteCapabilityType.BrowserStack.Project, _testSpecification.FeatureTitle);
            capabilities.SetCapability(RemoteCapabilityType.BrowserStack.Build, _buildNumber);
        }

        private void SetCredentials(DesiredCapabilities capabilities)
        {
            capabilities.SetCapability(RemoteCapabilityType.BrowserStack.UserName,
                string.IsNullOrWhiteSpace(_userName) ? _configurationProvider.UserName : _userName);
            capabilities.SetCapability(RemoteCapabilityType.BrowserStack.AccessKey,
                string.IsNullOrWhiteSpace(_accessKey) ? _configurationProvider.AccessKey : _accessKey);
        }
    }
}