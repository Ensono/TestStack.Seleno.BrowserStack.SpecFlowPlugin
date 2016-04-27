using System.CodeDom;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using TechTalk.SpecFlow.Generator;

namespace TestStack.Seleno.Browserstack.UnitTests.Assertions
{
    public class TestClassGenerationContextAssertions :
        ReferenceTypeAssertions<TestClassGenerationContext, TestClassGenerationContextAssertions>
    {
        public TestClassGenerationContextAssertions(TestClassGenerationContext generationContext)
        {
            Subject = generationContext;
        }

        public AndConstraint<TestClassGenerationContextAssertions> HaveNumberOfImports(int value)
        {
            var count = Subject.Namespace.Imports.OfType<CodeNamespaceImport>().Count();
            Execute
                .Assertion
                .ForCondition(count == value)
                .FailWith(
                    $@"Expected to have {value} namespace imports, but found {count}.");
            return new AndConstraint<TestClassGenerationContextAssertions>(this);
        }

        protected override string Context
        {
            get { return "TestClassGenerationContext"; }
        }

        public AndConstraint<TestClassGenerationContextAssertions> ContainImportedNamespace(string namespaceImport)
        {
            var namespaceImports = Subject.Namespace.Imports.OfType<CodeNamespaceImport>().ToList();

            Execute
               .Assertion
               .ForCondition(namespaceImports.Any(n => n.Namespace == namespaceImport))
               .FailWith(
                   $@"Expected to contain imported {namespaceImport}, but found {string.Join(",",namespaceImports.Select(n => n.Namespace))}.");

            return new AndConstraint<TestClassGenerationContextAssertions>(this);
        }
    }
}