namespace TestStack.Seleno.BrowserStack.Core.Configuration
{
    public interface IBrowserConfigurationParser
    {
        BrowserConfiguration Parse(string value);
    }
}