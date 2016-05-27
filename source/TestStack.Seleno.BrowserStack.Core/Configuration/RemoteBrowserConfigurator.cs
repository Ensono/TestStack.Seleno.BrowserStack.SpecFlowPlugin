using System;
using Castle.Core.Internal;
using TestStack.Seleno.BrowserStack.Core.Capabilities;
using TestStack.Seleno.BrowserStack.Core.Enums;
using TestStack.Seleno.BrowserStack.Core.Exceptions;

namespace TestStack.Seleno.BrowserStack.Core.Configuration
{
    public class RemoteBrowserConfigurator 
    {
        private readonly IBrowserHostFactory _browserHostFactory;
        private readonly IBrowserConfigurationParser _parser;
        private readonly ICapabilitiesBuilder _capabilitiesBuilder;
        private readonly IConfigurationProvider _configurationProvider;

        private const string InvalidBrowserConfigurationErrorMessage =
            "useLocalBrowser - local browser configuration must be one of the following Chrome, Firefox, InternetExplorer, PhantomJs, Safari";

        public RemoteBrowserConfigurator(IBrowserHostFactory browserHostFactory, IBrowserConfigurationParser parser,
            ICapabilitiesBuilder capabilitiesBuilder, IConfigurationProvider configurationProvider)
        {
            _browserHostFactory = browserHostFactory;
            _parser = parser;
            _capabilitiesBuilder = capabilitiesBuilder;
            _configurationProvider = configurationProvider;
        }

        public IBrowserHost CreateAndConfigure(TestSpecification testSpecification, string browser = null)
        {
            var builder = _capabilitiesBuilder.WithTestSpecification(testSpecification);
            BrowserConfiguration browserConfiguration = null;

            if (browser != null)
            {
                browserConfiguration = _parser.Parse(browser);
                builder.WithBrowserConfiguration(browserConfiguration);
            }

            if (_configurationProvider.UseLocalBrowser.IsNullOrEmpty())
            {
                return _browserHostFactory.CreateWithCapabilities(builder.Build(), browserConfiguration);
            }
            else
            {
                BrowserEnum result;

                if (Enum.IsDefined(typeof(BrowserEnum), _configurationProvider.UseLocalBrowser))
                {
                    Enum.TryParse(_configurationProvider.UseLocalBrowser.Replace(" ", string.Empty), out result);                    
                }
                else
                {
                    throw new InvalidBrowserConfigurationException(InvalidBrowserConfigurationErrorMessage);
                }

                return _browserHostFactory.CreateLocalWebDriver(result, browserConfiguration);
            }
        }
    }
}