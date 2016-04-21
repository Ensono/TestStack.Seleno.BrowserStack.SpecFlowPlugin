namespace TestStack.Seleno.BrowserStack.Core.Services.Client
{
    public interface IHttpClientFactory
    {
        IHttpClient Create(string baseAddress);
    }
}