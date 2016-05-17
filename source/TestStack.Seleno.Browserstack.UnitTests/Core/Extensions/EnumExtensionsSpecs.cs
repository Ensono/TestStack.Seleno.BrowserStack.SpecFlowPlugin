using FluentAssertions;
using NUnit.Framework;
using TestStack.Seleno.BrowserStack.Core.Extensions;
using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

namespace TestStack.Seleno.Browserstack.UnitTests.Core.Extensions
{
    [TestFixture]
    public class EnumExtensionsSpecs
    {
        public enum LifeInTheUniverse
        {
            None,
            [Description("RareHypothesis")]
            EarthlingOnly,
            [Description("DrakeEquation=R*.Fp.Ne.Fl.Fi.Fc.L|MultiverseTheory=maybeInAnotherUniverse")]
            AlienOutThere,  

            
        }

        [Test]
        public void GetDescription_ShouldReturnMemberStringRepresentationWhenNoDescriptionAttribute()
        {
            // Assert & Act
            LifeInTheUniverse.None.GetDescription().Should().Be("None");
        }

        [TestCase(LifeInTheUniverse.EarthlingOnly, "RareHypothesis")]
        [TestCase(LifeInTheUniverse.AlienOutThere, "DrakeEquation=R*.Fp.Ne.Fl.Fi.Fc.L|MultiverseTheory=maybeInAnotherUniverse")]
        public void GetDescription_ShouldReturnMemberStringRepresentationWhenNoDescriptionAttribute(LifeInTheUniverse enumeration, string description)
        {
            // Act & Assert
            enumeration.GetDescription().Should().Be(description);
        }

        [TestCase("someKey")]
        [TestCase(null)]
        [TestCase("")]
        public void GetValueFor_ShouldReturnDescriptionWhenNoKeyValueNorItemDefaultSeparators(string anyKeyName)
        {
            // Act & Assert
            LifeInTheUniverse.EarthlingOnly.GetValueFor(anyKeyName).Should().Be("RareHypothesis");
        }

        [TestCase("DrakeEquation", "R*.Fp.Ne.Fl.Fi.Fc.L")]
        [TestCase("MultiverseTheory", "maybeInAnotherUniverse")]
        public void GetValueFor_ShouldExtractValueForSpecified(string keyName, string expectedValue)
        {
            // Arrange
            const char itemSeparator = '|';

            // Act & Assert
            LifeInTheUniverse.AlienOutThere.GetValueFor(keyName, itemSeparator).Should().Be(expectedValue);
        }
    }
}