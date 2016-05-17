using BoDi;
using TechTalk.SpecFlow;
using TestStack.Seleno.BrowserStack.Core.Pages;

namespace TestStack.Seleno.BrowserStack.Core.Extensions
{
    public static class PageProvider
    {
        private static IObjectContainer _container;

        private static IObjectContainer Container
        {
            get { return _container ?? (_container = ScenarioContext.Current.ScenarioContainer); }
        }

        internal static void SetContainer(IObjectContainer value)
        {
            _container = value;
        }

        public static void AndRegister<TPage>(this TPage page) where TPage : Page, new()
        {
            Container.RegisterInstance(page);
        }

        public static TPage GetPage<TPage>(string name = null) where TPage : Page, new()
        {
            return Container.Resolve<TPage>(name);
        }
    }
}