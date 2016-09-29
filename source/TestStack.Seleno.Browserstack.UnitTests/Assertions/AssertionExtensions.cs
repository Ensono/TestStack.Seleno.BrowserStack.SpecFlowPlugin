using System.CodeDom;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using FluentAssertions;
using TechTalk.SpecFlow.Generator;
using TestStack.Seleno.BrowserStack.Core.Services.BrowserStack;

namespace TestStack.Seleno.Browserstack.UnitTests.Assertions
{
    public static class AssertionExtensions
    {
        public static CodeAttributeDeclarationCollectionAssertions Should(
            this CodeAttributeDeclarationCollection attributes)
        {
            return new CodeAttributeDeclarationCollectionAssertions(attributes.Cast<CodeAttributeDeclaration>());
        }

        public static NonGenericDictionaryAssertions Should(this IDictionary dictionary)
        {
            return new NonGenericDictionaryAssertions(dictionary);
        }

        public static CodeTypeDeclarationAssertions Should(this CodeTypeDeclaration testClass)
        {
            return new CodeTypeDeclarationAssertions(testClass);
        }

        public static TestClassGenerationContextAssertions Should(this TestClassGenerationContext context)
        {
            return new TestClassGenerationContextAssertions(context);
        }

        public static ProcessStartInfoAssertions Should(this ProcessStartInfo processStartInfo)
        {
            return new ProcessStartInfoAssertions(processStartInfo);
        }

    }
}