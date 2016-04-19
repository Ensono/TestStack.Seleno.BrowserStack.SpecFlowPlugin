using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using TestStack.Seleno.BrowserStack.Core.Extensions;

namespace TestStack.Seleno.Browserstack.UnitTests.SpecFlowPlugin.Extensions
{
    [TestFixture]
    public class CategorySelectionSpecs
    {
        private readonly string[] values = new[]
        {"", "Something", "Browser: chome", "browser: internet explorer", "blah"};

        [Test]
        public void WithBrowserTag_ShouldReturnEmptyCollectionWhenValuesIsNull()
        {
            // Arrange
            IEnumerable<string> nullCollection = null;
            
            // Act
            var results = nullCollection.WithBrowserTag();
            
            // Assert
            results.Should().BeEmpty();
        }

        [Test]
        public void WithBrowserTag_ShouldFilterOnlyValuesStartingWithBrowser()
        {
            // Act
            var results = values.WithBrowserTag();

            // Assert
            results.Should().BeEquivalentTo(new[] {"Browser: chome", "browser: internet explorer"});
        }

        [Test]
        public void WithoutBrowserTag_ShouldReturnEmptyCollectionWhenValuesIsNull()
        {
            // Arrange
            IEnumerable<string> nullCollection = null;

            // Act
            var results = nullCollection.WithoutBrowserTag();

            // Assert
            results.Should().BeEmpty();
        }

        [Test]
        public void WithoutBrowserTag_ShouldFilterOnlyValuesStartingWithBrowser()
        {
            // Act
            var results = values.WithoutBrowserTag();

            // Assert
            results.Should().BeEquivalentTo(new[] { "", "Something", "blah" });
        }

        [Test]
        public void UserFriendlyBrowserConfiguration_ShouldReplaceSeparators()
        {
            // Arrange
            const string compactBrowserConfiguration = "firefox,43.0,OS_X,Lion";

            // Act
            var result = compactBrowserConfiguration.UserFriendlyBrowserConfiguration();

            // Assert
            result.Should().Be("firefox 43.0 OS X Lion");
        }
    }
}