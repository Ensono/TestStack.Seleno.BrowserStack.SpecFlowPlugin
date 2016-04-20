using TestStack.Seleno.BrowserStack.Core.Capabilities;

namespace TestStack.Seleno.BrowserStack.Core.Configuration
{
    public class RemoteBrowserConfigurator 
    {
        private readonly IBrowserHostFactory _browserHostFactory;
        private readonly IBrowserConfigurationParser _parser;
        private readonly ICapabilitiesBuilder _capabilitiesBuilder;

        public RemoteBrowserConfigurator(IBrowserHostFactory browserHostFactory, IBrowserConfigurationParser parser,
            ICapabilitiesBuilder capabilitiesBuilder)
        {
            _browserHostFactory = browserHostFactory;
            _parser = parser;
            _capabilitiesBuilder = capabilitiesBuilder;
        }

        public IBrowserHost CreateAndConfigure(TestSpecification testSpecification, string browser = null)
        {
            var builder = _capabilitiesBuilder.WithTestSpecification(testSpecification);

            if (browser != null)
            {
                builder.WithBrowserConfiguration(_parser.Parse(browser));
            }
                
            return _browserHostFactory.CreateWithCapabilities(builder.Build());
        }
    }
}