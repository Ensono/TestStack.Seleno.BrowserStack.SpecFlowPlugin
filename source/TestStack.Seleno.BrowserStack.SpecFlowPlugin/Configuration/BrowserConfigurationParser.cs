using TestStack.Seleno.BrowserStack.SpecFlowPlugin.Exceptions;

namespace TestStack.Seleno.BrowserStack.SpecFlowPlugin.Configuration
{
    public class BrowserConfigurationParser : IBrowserConfigurationParser
    {
        public const char Separator = ',';

        private const string InvalidBrowserConfigurationErrorMessage =
            "Browser configuration is expected to contain browser name, browser version, operating system " +
            "and operating system version or browser and device name";

        private const string UnSpecfiedBrowserConfiguratioErrorMessage = "No browser configuration was specified";

        private const string BrowserVersionMustBeNumericErrorMessage = "Browser version must be numeric";

        public BrowserConfiguration Parse(string value)
        {
            bool isMobileDevice;
            var parameters = ValidateAndExtractBrowserConfiguration(value, out isMobileDevice);

            if (isMobileDevice)
            {
                return new BrowserConfiguration(parameters[0], parameters[1]);
            }


            return new BrowserConfiguration(parameters[0], parameters[1], parameters[2].Replace("_", " "),
                parameters[3]);
        }

        private static string[] ValidateAndExtractBrowserConfiguration(string value, out bool isMobileDevice)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidBrowserConfigurationException(UnSpecfiedBrowserConfiguratioErrorMessage);
            }
            var parameters = value.Split(Separator);

            if (parameters.Length != 2 && parameters.Length != 4)
            {
                throw new InvalidBrowserConfigurationException(InvalidBrowserConfigurationErrorMessage);
            }

            isMobileDevice = parameters.Length == 2;

            if (!isMobileDevice)
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