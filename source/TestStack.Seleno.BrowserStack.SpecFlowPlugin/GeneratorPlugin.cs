using BoDi;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Generator.Plugins;
using TechTalk.SpecFlow.Generator.UnitTestProvider;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Utils;
using TestStack.Seleno.BrowserStack.SpecFlowPlugin;

[assembly: GeneratorPlugin(typeof(GeneratorPlugin))]

namespace TestStack.Seleno.BrowserStack.SpecFlowPlugin
{
    public class GeneratorPlugin : IGeneratorPlugin
    {
        public void RegisterDependencies(ObjectContainer container)
        {
        }

        public void RegisterCustomizations(ObjectContainer container,
            SpecFlowProjectConfiguration generatorConfiguration)
        {
            var projectSettings = container.Resolve<ProjectSettings>();
            var codeDomHelper = container.Resolve<CodeDomHelper>(projectSettings.ProjectPlatformSettings.Language);
            container.RegisterInstanceAs<IUnitTestGeneratorProvider>(new SeleniumXUnitTestGeneratorProvider(codeDomHelper));
        }

        public void RegisterConfigurationDefaults(SpecFlowProjectConfiguration specFlowConfiguration)
        {
        }

        public void Initialize(GeneratorPluginEvents generatorPluginEvents, GeneratorPluginParameters generatorPluginParameters)
        {         
        }
    }
}