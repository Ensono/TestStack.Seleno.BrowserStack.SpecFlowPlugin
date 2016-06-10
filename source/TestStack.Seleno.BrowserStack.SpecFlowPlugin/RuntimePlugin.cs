using TechTalk.SpecFlow.Plugins;
using TechTalk.SpecFlow.UnitTestProvider;
using TestStack.Seleno.BrowserStack.SpecFlowPlugin;

[assembly: RuntimePlugin(typeof(RuntimePlugin))]
namespace TestStack.Seleno.BrowserStack.SpecFlowPlugin
{
    public class RuntimePlugin : IRuntimePlugin
    {    
        public void Initialize(RuntimePluginEvents runtimePluginEvents, RuntimePluginParameters runtimePluginParameters)
        {
            runtimePluginEvents.RegisterGlobalDependencies += (sender, args) =>
                args.ObjectContainer.RegisterInstanceAs<IUnitTestRuntimeProvider>(new NUnitRuntimeProvider(), "SeleniumNUnit");

        }
    }
}