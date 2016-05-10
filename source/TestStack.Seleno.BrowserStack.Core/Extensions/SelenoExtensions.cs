using System;
using OpenQA.Selenium.Support.UI;
using TestStack.Seleno.PageObjects.Actions;
using TestStack.Seleno.PageObjects.Locators;

namespace TestStack.Seleno.BrowserStack.Core.Extensions
{
    public static class SelenoExtensions
    {
        public static SelectElement DropDown(this IElementFinder finder, By.jQueryBy by,
            TimeSpan maxWait = default(TimeSpan))
        {
            return new SelectElement(finder.Element(@by, maxWait));
        }

        public static void SelectByDisplayedText(this SelectElement select, string text)
        {
            select.WrappedElement.Click();
            select.SelectByText(text);
            select.WrappedElement.Click();
        }
    }
}