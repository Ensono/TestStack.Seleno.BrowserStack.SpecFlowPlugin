using System;
using System.Collections.Generic;
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

            var capabilities = builder.WithRunTestLocally(_configurationProvider.RunTestLocally).Build();

            return _configurationProvider.LocalBrowser.HasValue
                ? _browserHostFactory.CreateLocalWebDriver(_configurationProvider.LocalBrowser.Value, browserConfiguration)
                : _browserHostFactory.CreateWithCapabilities(capabilities, browserConfiguration);
        }
    }
}