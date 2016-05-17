using BoDi;
using TechTalk.SpecFlow;
using TestStack.Seleno.BrowserStack.Core.Pages;

namespace TestStack.Seleno.BrowserStack.Core.Extensions
{
    public static class PageProvider
    {
        internal static IObjectContainer Container { get;  set; }
        private static IObjectContainer InitialisedContainer
        {
            get { return (Container ?? ScenarioContext.Current.ScenarioContainer); }
        }

        public static void AndRegister<TPage>(this TPage page) where TPage : Page, new()
        {
            InitialisedContainer.RegisterInstance(page);
        }

        

        public static TPage GetPage<TPage>(string name = null) where TPage : Page, new()
        {
            return (Container ?? ScenarioContext.Current.ScenarioContainer).Resolve<TPage>(name);
        }
    }
}