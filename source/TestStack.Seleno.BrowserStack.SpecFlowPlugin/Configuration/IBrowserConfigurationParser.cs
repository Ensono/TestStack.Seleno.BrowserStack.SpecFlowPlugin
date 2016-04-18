namespace TestStack.Seleno.BrowserStack.SpecFlowPlugin.Configuration
{
    public interface IBrowserConfigurationParser
    {
        BrowserConfiguration Parse(string value);
    }
}