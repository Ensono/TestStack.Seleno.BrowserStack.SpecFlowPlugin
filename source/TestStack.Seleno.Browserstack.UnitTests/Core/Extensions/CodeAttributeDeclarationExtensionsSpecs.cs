using System.CodeDom;
using FluentAssertions;
using NUnit.Framework;
using TestStack.Seleno.BrowserStack.Core.Extensions;

namespace TestStack.Seleno.Browserstack.UnitTests.Core.Extensions
{
    [TestFixture]
    public class CodeAttributeDeclarationExtensionsSpecs
    {
        const string NunitDescriptionattribute = "Xunit.TraitAttribute";
        const string NunitTestAttribute = "Xunit.FactAttribute";
        const string NunitIgnoreAttribute = "Xunit.FactAttribute(Skip=\"Ignored\")";

        [Test]
        public void GetDescription_ShouldExtractTheMethodDescriptionAttributeValue()
        {
            // Arrange
            var codeMember = new CodeMemberMethod();
            const string descriptionTitle = "Grand description";
            codeMember.CustomAttributes.AddRange(new[]
            {
                new CodeAttributeDeclaration(NunitTestAttribute),

                new CodeAttributeDeclaration(NunitDescriptionattribute,
                    new CodeAttributeArgument(new CodePrimitiveExpression(descriptionTitle))),
                new CodeAttributeDeclaration(NunitIgnoreAttribute),

            });

            // Act
            var result = codeMember.GetDescription();

            // Assert
            result.Should().Be(descriptionTitle);
        }

        [Test]
        public void GetDescription_ShouldReturnTestMethodNameAsSentenceWhenNoDescriptionFound()
        {
            // Arrange
            var codeMember = new CodeMemberMethod() {Name = "SuperAwesomeMethodName"};
            codeMember.CustomAttributes.AddRange(new[]
            {
                new CodeAttributeDeclaration(NunitTestAttribute),
                new CodeAttributeDeclaration(NunitIgnoreAttribute),

            });

            // Act
            var result = codeMember.GetDescription();

            // Assert
            result.Should().Be("Super awesome method name");
        }

        [Test]
        public void ToSentence_ShouldReturnTestMethodNameAsSentenceWhenNoDescriptionFound()
        {
            // Act
            var result = "SuperAwesomeMethodName".ToSentenceCase();

            // Assert
            result.Should().Be("Super awesome method name");
        }

    }
}