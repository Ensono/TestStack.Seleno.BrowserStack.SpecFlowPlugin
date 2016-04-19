using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using TestStack.Seleno.BrowserStack.Core.Configuration;

namespace TestStack.Seleno.BrowserStack.Core.Capabilities
{
    public class CapabilitiesBuilder : ICapabilitiesBuilder
    {
        private TestSpecification _testSpecification = new TestSpecification(string.Empty, string.Empty);
        private string _userName = "automate@amido.com";
        private string _accessKey = "EN1jzb16rb26dett";
        private BrowserConfiguration _browserConfiguration = new BrowserConfiguration();
        private string _buildNumber = string.Empty;

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

        public ICapabilities Build()
        {
            var result = new DesiredCapabilities();
            
            SetCredentials(result);

            SetTestConfiguration(result);

            SetBrowserConfiguration(result);

            return result;
        }

        private void SetBrowserConfiguration(DesiredCapabilities result)
        {
            result.SetCapability(RemoteCapabilityType.Default.BrowserName, _browserConfiguration.Name);

            if (_browserConfiguration.IsMobileDevice)
            {
                result.SetCapability(RemoteCapabilityType.BrowserStack.Device, _browserConfiguration.Device);
            }
            else
            {
                result.SetCapability(RemoteCapabilityType.Default.Version, _browserConfiguration.Version);
                result.SetCapability(RemoteCapabilityType.BrowserStack.Os, _browserConfiguration.OsName);
                result.SetCapability(RemoteCapabilityType.BrowserStack.OsVersion, _browserConfiguration.OsVersion);

                PlatformType platformType;
                if (Enum.TryParse(_browserConfiguration.OsName, true, out platformType))
                {
                    var platform = new Platform(platformType);
                    result.SetCapability(RemoteCapabilityType.Default.Platform, platform);
                }
            }
        }

        private void SetTestConfiguration(DesiredCapabilities result)
        {
            result.SetCapability(RemoteCapabilityType.BrowserStack.TestName, _testSpecification.ScenarioTitle);
            result.SetCapability(RemoteCapabilityType.BrowserStack.Project, _testSpecification.FeatureTitle);
            result.SetCapability(RemoteCapabilityType.BrowserStack.Build, _buildNumber);
        }

        private void SetCredentials(DesiredCapabilities result)
        {
            result.SetCapability(RemoteCapabilityType.BrowserStack.UserName, _userName);
            result.SetCapability(RemoteCapabilityType.BrowserStack.AccessKey, _accessKey);
        }
    }
}