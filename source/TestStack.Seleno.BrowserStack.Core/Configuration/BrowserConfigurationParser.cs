using System.Linq;
using TestStack.Seleno.BrowserStack.Core.Exceptions;
using TestStack.Seleno.BrowserStack.Core.Services.TestSession;

namespace TestStack.Seleno.BrowserStack.Core.Configuration
{
    public class BrowserConfigurationParser : IBrowserConfigurationParser
    {
        private readonly IBrowserStackService _browserStackService;
        public const char ConfigurationItemSeparator = ',';
        public const string ItemSpaceSeparator = "_";

        private const string InvalidBrowserConfigurationErrorMessage =
            "Browser configuration is expected to contain browser name, browser version, operating system " +
            "and operating system version or browser name or platform name and device name";

        private const string UnSpecfiedBrowserConfiguratioErrorMessage = "No browser configuration was specified";

        private const string BrowserVersionMustBeNumericErrorMessage = "Browser version must be numeric";

        public BrowserConfigurationParser(IBrowserStackService browserStackService)
        {
            _browserStackService = browserStackService;
        }

        public BrowserConfiguration Parse(string value)
        {
            bool isMobileDevice;
            var parameters = ValidateAndExtractBrowserConfiguration(value, out isMobileDevice);
            BrowserConfiguration result;

            if (isMobileDevice)
            {
                result =  new BrowserConfiguration(parameters[0], parameters[1].Replace(ItemSpaceSeparator, " "));
            }
            else if (parameters.Length == 1)
            {
                result = new BrowserConfiguration(parameters[0]);
            }
            else
            {
                result = new BrowserConfiguration(parameters[0], parameters[1],
                    parameters[2].Replace(ItemSpaceSeparator, " "),
                    parameters[3], ParseDesktopResolution(parameters));
            }

            if (_browserStackService.IsNotSupported(result))
            {
                throw new UnsupportedBrowserException(result);
            }

            return result;
        }

        private static string ParseDesktopResolution(string[] parameters)
        {
            var resolution = parameters.Length == 5 ? parameters[4] : BrowserConfiguration.DefaultDesktopResolution;
            var resolutionParts = resolution.Split('x');
            int resolutionH, resolutionV;
            if (resolutionParts.Length != 2 || !int.TryParse( resolutionParts[0], out resolutionH)  || !int.TryParse(resolutionParts[1], out resolutionV))
            {
                throw new InvalidDesktopResolution(resolution);
            }
            return resolution;
        }

        private static string[] ValidateAndExtractBrowserConfiguration(string value, out bool isMobileDevice)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidBrowserConfigurationException(UnSpecfiedBrowserConfiguratioErrorMessage);
            }
            var parameters = value.Split(ConfigurationItemSeparator);
            var supportedNumberOfParameters = new[] {1, 2, 4, 5};

            if (!supportedNumberOfParameters.Contains(parameters.Length))
            {
                throw new InvalidBrowserConfigurationException(InvalidBrowserConfigurationErrorMessage);
            }

            isMobileDevice = parameters.Length == 2;

            if (!isMobileDevice && parameters.Length > 1)
            {
                decimal browserVersion;
                if (!decimal.TryParse(parameters[1], out browserVersion))
                {
                    throw new InvalidBrowserConfigurationException(BrowserVersionMustBeNumericErrorMessage);
                }
            }
            return parameters;
        }
    }
}