using System;
using BoDi;
using OpenQA.Selenium;
using TestStack.Seleno.BrowserStack.Core.Extensions;

namespace TestStack.Seleno.BrowserStack.Core
{
    public abstract class Page : PageObjects.Page
    {
        internal IObjectContainer Container { get; set; }

        public void ExecuteScript(string script)
        {
            Execute.Script(script);
        }

        protected TPage NavigateTo<TPage>(By by, TimeSpan maxwait = default(TimeSpan)) where TPage : Page, new()
        {
            return NavigateToAndRegister(() => Navigate.To<TPage>(by, maxwait));
        }
     
        protected TPage NavigateTo<TPage>(PageObjects.Locators.By.jQueryBy jQueryElementToClick, TimeSpan maxWait = default(TimeSpan))
            where TPage : Page, new()
        {
            return NavigateToAndRegister(() => Navigate.To<TPage>(jQueryElementToClick, maxWait));
        }

        protected TPage NavigateTo<TPage>(string url) where TPage : Page, new()
        {
            return NavigateToAndRegister(() => Navigate.To<TPage>(url));
        }

        protected TPage NavigateToAndRegister<TPage>(Func<TPage> pageNavigator) where TPage : Page, new()
        {
            var page = pageNavigator();
            if (Container != null)
            {
                Container.RegisterInstance(page);
            }
            page.Container = Container;

            return page;
        }

    }
}