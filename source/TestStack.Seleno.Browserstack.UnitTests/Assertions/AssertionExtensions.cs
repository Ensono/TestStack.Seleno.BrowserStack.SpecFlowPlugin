using System.CodeDom;
using System.Collections;
using System.Linq;
using TechTalk.SpecFlow.Generator;

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

    }
}