using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Collections;
using FluentAssertions.Execution;

namespace TestStack.Seleno.Browserstack.UnitTests.Assertions
{
    public class CodeAttributeDeclarationCollectionAssertions : GenericCollectionAssertions<CodeAttributeDeclaration>
    {
        public CodeAttributeDeclarationCollectionAssertions(IEnumerable<CodeAttributeDeclaration> actualValue) : base(actualValue)
        {
        }

        public AndConstraint<CodeAttributeDeclarationCollectionAssertions> Contain(string attributeName, params CodeAttributeArgument[] arguments)
        {
            var attribute = Subject.FirstOrDefault(d => d.Name.EndsWith(attributeName, StringComparison.InvariantCultureIgnoreCase) && ArgumentAreEquivalents(d.Arguments, arguments));
            Execute
                .Assertion
                .ForCondition(attribute != null)
                .FailWith("Expected to contain attribute named {0} with arguments {1}, but found {1}.", attributeName, arguments, attribute);

            return new AndConstraint<CodeAttributeDeclarationCollectionAssertions>(this);
        }


        public AndConstraint<CodeAttributeDeclarationCollectionAssertions> OnlyContain(string attributeName, params CodeAttributeArgument[] arguments)
        {
            var matchingAttributes = Subject.Where(d => d.Name.EndsWith(attributeName, StringComparison.InvariantCultureIgnoreCase)).ToList();

            Execute
                .Assertion
                .ForCondition(matchingAttributes.Count == 1 && ArgumentAreEquivalents(matchingAttributes[0].Arguments,arguments))
                .FailWith("Expected to only contain attribute named {0} with arguments {1}, but found {1}.", attributeName, arguments, matchingAttributes[0]);

            return new AndConstraint<CodeAttributeDeclarationCollectionAssertions>(this);
        }

        public AndConstraint<CodeAttributeDeclarationCollectionAssertions> OnlyContain(string attributeName, IEnumerable<CodeAttributeArgument> arguments)
        {
            return OnlyContain(attributeName, arguments.ToArray());
        }

        private static bool ArgumentAreEquivalents(CodeAttributeArgumentCollection testCaseAttributeArguments, CodeAttributeArgument[] arguments)
        {
            return
                testCaseAttributeArguments.Count == arguments.Length &&
                (testCaseAttributeArguments.Count == 0 ||
                 arguments.Where((arg, i) => arg.Name.Equals(testCaseAttributeArguments[i].Name) && AreEqual(arg.Value, testCaseAttributeArguments[i].Value)).Any());
        }

        private static string CodePrimitiveExpressionValue(CodeExpression codeExpression)
        {
            var expression = codeExpression as CodePrimitiveExpression;
            var result = string.Empty;
            if (expression?.Value is string)
            {
                result = (string)expression.Value;
            }
            return result;
        }

        private static bool AreEqual(CodeExpression codeExpression, CodeExpression anotherCodeExpression)
        {
            if (codeExpression.GetType() != anotherCodeExpression.GetType()) return false;

            if (codeExpression is CodePrimitiveExpression)
            {
                return
                    CodePrimitiveExpressionValue(codeExpression)
                        .Equals(CodePrimitiveExpressionValue(anotherCodeExpression),
                            StringComparison.InvariantCultureIgnoreCase);
            }

            if (codeExpression is CodeArrayCreateExpression)
            {
                var codeExpressions = (CodeArrayCreateExpression) codeExpression;
                var anotherCodeExpressions = (CodeArrayCreateExpression) anotherCodeExpression;

                if (codeExpressions.CreateType.BaseType != anotherCodeExpressions.CreateType.BaseType ||
                    codeExpressions.Initializers.Count != anotherCodeExpressions.Initializers.Count)
                {
                    return false;
                }

                for (var i = 0; i < codeExpressions.Initializers.Count; i++)
                {
                    if (!AreEqual(codeExpressions.Initializers[i], anotherCodeExpressions.Initializers[i]))
                    {
                        return false;
                    }
                }
                return true;
            }

            return false;
        }
    }
}