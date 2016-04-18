using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TestStack.Seleno.BrowserStack.SpecFlowPlugin.Extensions
{
    public static class CategorySelection
    {
        public const string BrowserTagName = "browser:";

        private static readonly Func<string, bool> HasBrowserTag =
            c => c.StartsWith(BrowserTagName, StringComparison.InvariantCultureIgnoreCase);


        public static IEnumerable<string> WithBrowserTag(this IEnumerable<string> scenarioCategories)
        {
            return scenarioCategories?.Where(HasBrowserTag) ?? Enumerable.Empty<string>();
        }

        public static IEnumerable<string> WithoutBrowserTag(this IEnumerable<string> scenarioCategories)
        {
            return scenarioCategories?.Where(c => !HasBrowserTag(c)) ?? Enumerable.Empty<string>();
        }

        public static string UserFriendlyBrowserConfiguration(this string browser)
        {
            return Regex.Replace(browser, "_|,", " ");
        }
    }
}