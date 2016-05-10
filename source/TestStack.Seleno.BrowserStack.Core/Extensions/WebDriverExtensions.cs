using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace TestStack.Seleno.BrowserStack.Core.Extensions
{
    public static class WebDriverExtensions
    {
        public static string TitleWithWait(this IWebDriver webDriver, TimeSpan maxWait = default(TimeSpan))
        {
            try
            {
                if (maxWait == default(TimeSpan))
                {
                    maxWait = TimeSpan.FromSeconds(5);
                }

                return
                    new WebDriverWait(webDriver, maxWait)
                        .Until(d => new[] {d.Title}.Select(GetTitle).First());
            }
            catch
            {
                return null;
            }
        }

        public static bool IsDisplayedOnNarrowedWidth(this IWebDriver webDriver, int widthBreakPoint)
        {
            return webDriver != null && webDriver.Manage().Window.Size.Width <= widthBreakPoint;
        }

        private static string GetTitle(string title)
        {
            return !string.IsNullOrEmpty(title) ? title : null;
        }
    }
}