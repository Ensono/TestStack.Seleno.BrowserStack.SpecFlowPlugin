using TestStack.Seleno.BrowserStack.Core.Capabilities;

namespace TestStack.Seleno.BrowserStack.Core.Configuration
{
    public class RemoteBrowserConfigurator 
    {
        private readonly IBrowserHostFactory _browserHostFactory;
        private readonly IBrowserConfigurationParser _parser;
        private readonly ICapabilitiesBuilder _capabilitiesBuilder;
        private readonly IConfigurationProvider _configurationProvider;

        public RemoteBrowserConfigurator()
            : this(new BrowserHostFactory(), new BrowserConfigurationParser(), new CapabilitiesBuilder(), new ConfigurationProvider())
        {

        }

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
            var builder = _capabilitiesBuilder
                            .WithCredentials(_configurationProvider.UserName, _configurationProvider.AccessKey)
                            .WithTestSpecification(testSpecification);

            if (browser != null)
            {
                builder.WithBrowserConfiguration(_parser.Parse(browser));
            }
                
            return _browserHostFactory.CreateWithCapabilities(builder.Build());
        }
    }
}