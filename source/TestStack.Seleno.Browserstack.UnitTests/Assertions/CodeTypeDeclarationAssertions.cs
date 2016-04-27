using System;
using System.CodeDom;
using System.Linq;
using System.Text;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;

namespace TestStack.Seleno.Browserstack.UnitTests.Assertions
{
    public class CodeTypeDeclarationAssertions : ReferenceTypeAssertions<CodeTypeDeclaration, CodeTypeDeclarationAssertions>
    {

        public CodeTypeDeclarationAssertions(CodeTypeDeclaration testClass)
        {
            Subject = testClass;
        }

        

        public AndConstraint<CodeTypeDeclarationAssertions> HaveNumberOfFieldMembers(int numberOfFieldMembers)
        {
            var count = Subject.Members.OfType<CodeMemberField>().Count();
            Execute
                .Assertion
                .ForCondition(count== numberOfFieldMembers)
                .FailWith(
                    $@"Expected class to have {numberOfFieldMembers} private field members, but found {count}.");
            return new AndConstraint<CodeTypeDeclarationAssertions>(this);
        }

        public AndConstraint<CodeTypeDeclarationAssertions> OnlyContainMemberMethod(CodeMemberMethod privateMethod)
        {

            Execute
                .Assertion
                .ForCondition(SingleMatchFor(privateMethod))
                .FailWith(
                    $@"Expected class to only contain private member method 
{CodeMemberMethodPrint(privateMethod)}

But found private member methods :

{string.Join(",\r", Subject.Members.OfType<CodeMemberMethod>().Select(CodeMemberMethodPrint))}.");

            return new AndConstraint<CodeTypeDeclarationAssertions>(this);
        }

        public AndConstraint<CodeTypeDeclarationAssertions> ContainMemberField(string type, string name)
        {
            var codeMemberFields = Subject.Members.OfType<CodeMemberField>().ToList();
            Execute
               .Assertion
               .ForCondition(codeMemberFields.Any(m => m.Type.BaseType == type && m.Name == name))
               .FailWith($@"Expected class to contain member field {type} {name}, but found {string.Join(",\r",codeMemberFields.Select(m => $"{m.Type.BaseType} {m.Name}"))}.");
            return new AndConstraint<CodeTypeDeclarationAssertions>(this);
        }

        private bool SingleMatchFor(CodeMemberMethod privateMethod)
        {
            var methods = Subject.Members.OfType<CodeMemberMethod>().ToList();

            return
                methods.Count == 1 &&
                methods[0].Parameters.Count == 1 && privateMethod.Parameters.Count == 1 &&
                methods[0].Parameters[0].Type.BaseType == privateMethod.Parameters[0].Type.BaseType &&
                methods[0].Parameters[0].Name == privateMethod.Parameters[0].Name &&
                methods[0].Statements.Count == 1 && privateMethod.Statements.Count == 1 &&
                ((CodeSnippetStatement) methods[0].Statements[0]).Value ==
                ((CodeSnippetStatement) privateMethod.Statements[0]).Value;
        }

        private string CodeMemberMethodPrint(CodeMemberMethod codeMemberMethod)
        {
            return
                $@"{codeMemberMethod.ReturnType.BaseType} {codeMemberMethod.Name}({FormatParameters(codeMemberMethod)})
                    {{ 
                       {FormatBody(codeMemberMethod)}
                    }}";
        }

        private string FormatBody(CodeMemberMethod codeMemberMethod)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < codeMemberMethod.Statements.Count; i++)
            {
                var statement = codeMemberMethod.Statements[i] as CodeSnippetStatement;
                if (statement != null)
                {
                    sb.AppendLine(statement.Value);
                }
            }

            return sb.ToString();
        }

        private static string FormatParameters(CodeMemberMethod codeMemberMethod)
        {
            return string.Join(",",codeMemberMethod.Parameters.OfType<CodeParameterDeclarationExpression>().Select(x => x.Type.BaseType +" " + x.Name));
        }

        protected override string Context { get { return "TestClass"; } }


       
    }
}