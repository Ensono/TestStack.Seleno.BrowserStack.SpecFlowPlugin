namespace TestStack.Seleno.BrowserStack.SpecFlowPlugin.Configuration
{
    public interface IConfigurationProvider  
    {
        string UserName { get; }
        string AccessKey { get; }
        string Encoded64Token { get; }
        string RemoteUrl { get; }
        string BuildNumber { get; }
    }
}