using System;
using OpenQA.Selenium;
using TestStack.Seleno.BrowserStack.Core;
using TestStack.Seleno.PageObjects.Actions;

namespace TestStack.Seleno.Browserstack.UnitTests.Core
{
    public class TestPage : Page
    {
        private new IPageNavigator Navigate { get; set; }

        public TestPage(IPageNavigator navigate)
        {
            Navigate = navigate;
        }

        public TPage NavigateTo<TPage>(By by, TimeSpan maxwait = default(TimeSpan)) where TPage : Page, new()
        {
            return NavigateToAndRegister(() => Navigate.To<TPage>(by, maxwait));
        }

        public TPage NavigateTo<TPage>(PageObjects.Locators.By.jQueryBy byJQuery, TimeSpan maxWait = default(TimeSpan)) where TPage : Page, new()
        {
            return NavigateToAndRegister(() => Navigate.To<TPage>(byJQuery, maxWait));
        }

        public TPage NavigateTo<TPage>(string url) where TPage : Page, new()
        {
            return NavigateToAndRegister(() => Navigate.To<TPage>(url));
        }
    }
}