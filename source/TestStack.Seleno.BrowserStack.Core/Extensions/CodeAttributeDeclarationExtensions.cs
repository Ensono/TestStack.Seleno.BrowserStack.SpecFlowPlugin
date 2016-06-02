using System.CodeDom;
using System.Linq;
using System.Text.RegularExpressions;

namespace TestStack.Seleno.BrowserStack.Core.Extensions
{
    public static class CodeAttributeDeclarationExtensions
    {
        public static string GetDescription(this CodeMemberMethod testMethod)
        {
            var attribute =
                testMethod.CustomAttributes.OfType<CodeAttributeDeclaration>()
                    .FirstOrDefault(attr => attr.Name == XUnitAttributeConstants.DESCRIPTION_ATTR);
            var description = string.Empty;

            if (attribute != null)
            {
                description = ((CodePrimitiveExpression) attribute.Arguments[0].Value).Value.ToString();
            }
            else
            {
                description = testMethod.Name.ToSentenceCase();
            }

            return description;
        }

        public static string ToSentenceCase(this string str)
        {
            return Regex.Replace(str, "[a-z][A-Z]", m => m.Value[0] + " " + char.ToLower(m.Value[1]));
        }
    }
}