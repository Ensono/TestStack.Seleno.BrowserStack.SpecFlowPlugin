namespace TestStack.Seleno.BrowserStack.SpecFlowPlugin.Configuration
{
    public class TestSpecification
    {
        public string ScenarioTitle { get; private set; }

        public string FeatureTitle { get; private set; }

        public TestSpecification(string scenarioTitle, string featureTitle)
        {
            ScenarioTitle = scenarioTitle;
            FeatureTitle = featureTitle;
        }


    }
}