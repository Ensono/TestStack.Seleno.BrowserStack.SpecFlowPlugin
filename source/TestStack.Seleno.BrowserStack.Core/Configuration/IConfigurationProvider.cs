using System.Collections.Generic;
using TestStack.Seleno.BrowserStack.Core.Enums;

namespace TestStack.Seleno.BrowserStack.Core.Configuration
{
    public interface IConfigurationProvider  
    {
        string UserName { get; }
        string AccessKey { get; }
        string Encoded64Token { get; }
        string RemoteUrl { get; }
        string BuildNumber { get; }
        string BrowserStackApiUrl { get; }
        BrowserEnum? LocalBrowser { get; }
        IDictionary<string, object> Capabilities { get; }

        bool RunTestLocally { get; }
    }
}