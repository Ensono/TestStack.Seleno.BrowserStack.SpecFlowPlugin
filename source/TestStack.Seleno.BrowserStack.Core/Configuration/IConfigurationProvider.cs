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
    }
}