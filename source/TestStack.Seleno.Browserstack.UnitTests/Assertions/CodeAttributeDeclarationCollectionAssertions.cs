using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public AndConstraint<CodeAttributeDeclarationCollectionAssertions> OnlyContain(CodeAttributeDeclaration attribute)
        {
            Execute
              .Assertion
              .ForCondition(Subject.Any(a => a == attribute || (attribute != null && a.Name == attribute.Name && ArgumentAreEquivalents(a.Arguments, attribute.Arguments))))
              .FailWith($"Expected to only contain attribute {attribute}, but found {1}.", Subject);
            return new AndConstraint<CodeAttributeDeclarationCollectionAssertions>(this);
        }

        public AndConstraint<CodeAttributeDeclarationCollectionAssertions> OnlyContain(string attributeName, params CodeAttributeArgument[] arguments)
        {
            var matchingAttributes = Subject.Where(d => d.Name.EndsWith(attributeName, StringComparison.InvariantCultureIgnoreCase)).ToList();

            var condition = matchingAttributes.Count == 1 &&
                            ArgumentAreEquivalents(matchingAttributes[0].Arguments, arguments);
            Execute
                .Assertion
                .ForCondition(condition)
                .FailWith($@"Expected to only contain [{attributeName}({FormatAttributeArguments(arguments)})]

but found: 
{FormatMatchingAttribute(matchingAttributes)}");

            return new AndConstraint<CodeAttributeDeclarationCollectionAssertions>(this);
        }

        private string FormatMatchingAttribute(List<CodeAttributeDeclaration> matchingAttributes)
        {
            var result = "no matching attribute";
            if (matchingAttributes != null && matchingAttributes.Count == 1)
            {
                result = "[" + matchingAttributes[0].Name + "(" +
                         FormatAttributeArguments(matchingAttributes[0].Arguments) + ")]";
            }

            return result;
        }

       

        public AndConstraint<CodeAttributeDeclarationCollectionAssertions> OnlyContain(string attributeName, IEnumerable<CodeAttributeArgument> arguments)
        {
            return OnlyContain(attributeName, arguments.ToArray());
        }

        private static bool ArgumentAreEquivalents(CodeAttributeArgumentCollection testCaseAttributeArguments, CodeAttributeArgument[] arguments)
        {
            if (testCaseAttributeArguments.Count != arguments.Length)
            {
                return false;
            }

            if (testCaseAttributeArguments.Count == 0)
            {
                return true;
            }

            for (int i = 0; i < testCaseAttributeArguments.Count; i++)
            {
                if (!AreEqual(arguments[i].Value, testCaseAttributeArguments[i].Value))
                {
                    return false;
                }
            }

            return true;
        }

        private static string FormatAttributeArguments(CodeAttributeArgument[] arguments)
        {
            return string.Join(",",
                arguments.Select(
                    a =>
                        string.IsNullOrWhiteSpace(a.Name)
                            ? "\"" + FormatAttributeValue(a) + "\""
                            : a.Name + "=" + "\"" + FormatAttributeValue(a) + "\""));

        }

        private static string FormatAttributeValue(CodeAttributeArgument a)
        {

            var expression = a.Value as CodePrimitiveExpression;

            if (expression != null)
            {
                return (string)expression.Value;
            }

            var arrayExpression = a.Value as CodeArrayCreateExpression;
            var sb = new StringBuilder();
            if (a.Value is CodeArrayCreateExpression)
            {
                return string.Join(",",
                    arrayExpression.Initializers.OfType<CodeExpression>().Select(CodePrimitiveExpressionValue));

                return sb.ToString();
            }

            return string.Empty;

        }

        private static string FormatAttributeArguments(CodeAttributeArgumentCollection arguments)
        {
            return FormatAttributeArguments(arguments.OfType<CodeAttributeArgument>().ToArray());

        }

        private static bool ArgumentAreEquivalents(CodeAttributeArgumentCollection testCaseAttributeArguments, CodeAttributeArgumentCollection arguments)
        {
            return ArgumentAreEquivalents(testCaseAttributeArguments, arguments.OfType<CodeAttributeArgument>().ToArray());
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