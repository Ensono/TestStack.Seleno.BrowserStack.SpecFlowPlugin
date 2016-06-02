using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.UnitTestProvider;
using TechTalk.SpecFlow.Utils;
using TestStack.Seleno.BrowserStack.Core;

namespace TestStack.Seleno.BrowserStack.SpecFlowPlugin
{
    public class BaseXunitTestGeneratorProvider : XUnitAttributeConstants, IUnitTestGeneratorProvider
    {
        public BaseXunitTestGeneratorProvider(CodeDomHelper codeDomHelper)
        {
            CodeDomHelper = codeDomHelper;
        }

        protected CodeDomHelper CodeDomHelper { get; private set; }

        public virtual void SetTestMethodCategories(TestClassGenerationContext generationContext,
            CodeMemberMethod testMethod, IEnumerable<string> scenarioCategories)
        {
            CodeDomHelper.AddAttributeForEachValue(testMethod, CATEGORY_ATTR, scenarioCategories);
        }

        public virtual void SetRow(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, IEnumerable<string> arguments, IEnumerable<string> tags, bool isIgnored)
        {
            var list = arguments.Select(arg => new CodeAttributeArgument(new CodePrimitiveExpression(arg))).ToList();
            var tagArray = tags as string[] ?? tags.ToArray();
            var num = tagArray.Any() ? 1 : 0;
            var source = tagArray.Select(t => (CodeExpression)new CodePrimitiveExpression(t)).ToArray();
            var codeExpression1 = num != 0 ? new CodePrimitiveExpression(null) : (CodeExpression)new CodeArrayCreateExpression(typeof(string[]), source);
            list.Add(new CodeAttributeArgument(codeExpression1));
            if (num != 0)
            {
                var codeExpression2 = (CodeExpression)new CodePrimitiveExpression(string.Join(",", tagArray.ToArray()));
                list.Add(new CodeAttributeArgument("Category", codeExpression2));
            }
            if (isIgnored)
                list.Add(new CodeAttributeArgument("Ignored", new CodePrimitiveExpression((object)true)));
            CodeDomHelper.AddAttribute(testMethod, ROW_ATTR, list.ToArray());
        }

        public virtual UnitTestGeneratorTraits GetTraits()
        {
            return UnitTestGeneratorTraits.RowTests;
        }

        public virtual void SetTestClass(TestClassGenerationContext generationContext, string featureTitle, string featureDescription)
        {            
        }

        public virtual void SetTestClassCategories(TestClassGenerationContext generationContext, IEnumerable<string> featureCategories)
        {
            CodeDomHelper.AddAttributeForEachValue(generationContext.TestClass, CATEGORY_ATTR, featureCategories);
        }

        public virtual void SetTestClassIgnore(TestClassGenerationContext generationContext)
        {
            CodeDomHelper.AddAttribute(generationContext.TestClass, IGNORE_ATTR);
        }

        public virtual void FinalizeTestClass(TestClassGenerationContext generationContext)
        {
        }

        public virtual void SetTestClassInitializeMethod(TestClassGenerationContext generationContext)
        {            
        }

        public virtual void SetTestClassCleanupMethod(TestClassGenerationContext generationContext)
        {
        }

        public virtual void SetTestInitializeMethod(TestClassGenerationContext generationContext)
        {
        }

        public virtual void SetTestCleanupMethod(TestClassGenerationContext generationContext)
        {            
        }

//        public virtual void SetTestClassInitializeMethod(TestClassGenerationContext generationContext)
//        {
//            CodeDomHelper.AddAttribute(generationContext.TestClassInitializeMethod, TESTFIXTURESETUP_ATTR);
//        }

//        public virtual void SetTestClassCleanupMethod(TestClassGenerationContext generationContext)
//        {
//            CodeDomHelper.AddAttribute(generationContext.TestClassCleanupMethod, TESTFIXTURETEARDOWN_ATTR);
//        }

//        public virtual void SetTestInitializeMethod(TestClassGenerationContext generationContext)
//        {
//            CodeDomHelper.AddAttribute(generationContext.TestInitializeMethod, TESTSETUP_ATTR);
//        }

//        public virtual void SetTestCleanupMethod(TestClassGenerationContext generationContext)
//        {
//            CodeDomHelper.AddAttribute(generationContext.TestCleanupMethod, TESTTEARDOWN_ATTR);
//        }

        public virtual void SetTestMethod(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string friendlyTestName)
        {
            CodeDomHelper.AddAttribute(testMethod, TEST_ATTR);
            CodeDomHelper.AddAttribute(testMethod, DESCRIPTION_ATTR, friendlyTestName);
        }

        public virtual void SetTestMethodIgnore(TestClassGenerationContext generationContext, CodeMemberMethod testMethod)
        {
            CodeDomHelper.AddAttribute(testMethod, IGNORE_ATTR);
        }

        public virtual void SetRowTest(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string scenarioTitle)
        {
            SetTestMethod(generationContext, testMethod, scenarioTitle);
        }

        public virtual void SetTestMethodAsRow(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string scenarioTitle, string exampleSetName, string variantName, IEnumerable<KeyValuePair<string, string>> arguments)
        {
        }
    }
}