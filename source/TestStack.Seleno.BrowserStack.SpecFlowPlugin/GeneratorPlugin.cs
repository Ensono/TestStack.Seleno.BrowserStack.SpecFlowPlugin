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
        public void Initialize(GeneratorPluginEvents generatorPluginEvents, GeneratorPluginParameters generatorPluginParameters)
        {            
            generatorPluginEvents.RegisterDependencies += 
                (sender, args) => 
                    args.ObjectContainer.RegisterInstanceAs<IUnitTestGeneratorProvider>(
                        new SeleniumNUnitTestGeneratorProvider(
                            args.ObjectContainer.Resolve<CodeDomHelper>(
                                args.ObjectContainer.Resolve<ProjectSettings>().ProjectPlatformSettings.Language)));
        }
    }
}