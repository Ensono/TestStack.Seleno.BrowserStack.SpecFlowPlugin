using System;
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

        private CodeTypeDeclaration _currentFixtureDataTypeDeclaration = null;

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
        }

        public virtual void FinalizeTestClass(TestClassGenerationContext generationContext)
        {
        }

        public virtual void SetTestClassInitializeMethod(TestClassGenerationContext generationContext)
        {
            // xUnit uses IUseFixture<T> on the class

            generationContext.TestClassInitializeMethod.Attributes |= MemberAttributes.Static;
            generationContext.TestRunnerField.Attributes |= MemberAttributes.Static;

            _currentFixtureDataTypeDeclaration = CodeDomHelper.CreateGeneratedTypeDeclaration("FixtureData");
            generationContext.TestClass.Members.Add(_currentFixtureDataTypeDeclaration);

            var fixtureDataType =
                CodeDomHelper.CreateNestedTypeReference(generationContext.TestClass, _currentFixtureDataTypeDeclaration.Name);

            var useFixtureType = CreateFixtureInterface(fixtureDataType);

            CodeDomHelper.SetTypeReferenceAsInterface(useFixtureType);

            generationContext.TestClass.BaseTypes.Add(useFixtureType);
            // generationContext.TestClass.BaseTypes.Add(DISPOSABLE);

            // public void SetFixture(T) { } // explicit interface implementation for generic interfaces does not work with codedom

            CodeMemberMethod setFixtureMethod = new CodeMemberMethod();
            setFixtureMethod.Attributes = MemberAttributes.Public;
            setFixtureMethod.Name = "SetFixture";
            setFixtureMethod.Parameters.Add(new CodeParameterDeclarationExpression(fixtureDataType, "fixtureData"));
            setFixtureMethod.ImplementationTypes.Add(useFixtureType);
            generationContext.TestClass.Members.Add(setFixtureMethod);

            // public <_currentFixtureTypeDeclaration>() { <fixtureSetupMethod>(); }
            CodeConstructor ctorMethod = new CodeConstructor();
            ctorMethod.Attributes = MemberAttributes.Public;
            _currentFixtureDataTypeDeclaration.Members.Add(ctorMethod);
            ctorMethod.Statements.Add(
                new CodeMethodInvokeExpression(
                    new CodeTypeReferenceExpression(new CodeTypeReference(generationContext.TestClass.Name)),
                    generationContext.TestClassInitializeMethod.Name));
        }

        public virtual void SetTestClassCleanupMethod(TestClassGenerationContext generationContext)
        {
            // xUnit uses IUseFixture<T> on the class

            generationContext.TestClassCleanupMethod.Attributes |= MemberAttributes.Static;

            _currentFixtureDataTypeDeclaration.BaseTypes.Add(typeof(IDisposable));

            // void IDisposable.Dispose() { <fixtureTearDownMethod>(); }

            CodeMemberMethod disposeMethod = new CodeMemberMethod();
            disposeMethod.PrivateImplementationType = new CodeTypeReference(typeof(IDisposable));
            disposeMethod.Name = "Dispose";
            _currentFixtureDataTypeDeclaration.Members.Add(disposeMethod);

            disposeMethod.Statements.Add(
                new CodeMethodInvokeExpression(
                    new CodeTypeReferenceExpression(new CodeTypeReference(generationContext.TestClass.Name)),
                    generationContext.TestClassCleanupMethod.Name));
        }

        public virtual void SetTestInitializeMethod(TestClassGenerationContext generationContext)
        {
        }

        public void SetTestCleanupMethod(TestClassGenerationContext generationContext)
        {
            // xUnit supports test tear down through the IDisposable interface

            generationContext.TestClass.BaseTypes.Add(typeof(IDisposable));

            // void IDisposable.Dispose() { <memberMethod>(); }

            CodeMemberMethod disposeMethod = new CodeMemberMethod();
            disposeMethod.PrivateImplementationType = new CodeTypeReference(typeof(IDisposable));
            disposeMethod.Name = "Dispose";
            generationContext.TestClass.Members.Add(disposeMethod);

            disposeMethod.Statements.Add(
                new CodeMethodInvokeExpression(
                    new CodeThisReferenceExpression(),
                    generationContext.TestCleanupMethod.Name));
        }

        public virtual void SetTestMethod(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string friendlyTestName)
        {
            CodeDomHelper.AddAttribute(testMethod, TEST_ATTR);
            CodeDomHelper.AddAttribute(testMethod, DESCRIPTION_ATTR, "Description", friendlyTestName);
        }

        public virtual void SetTestMethodIgnore(TestClassGenerationContext generationContext, CodeMemberMethod testMethod)
        {
            var factAttr = testMethod.CustomAttributes.OfType<CodeAttributeDeclaration>()
                .FirstOrDefault(codeAttributeDeclaration => codeAttributeDeclaration.Name == FACT_ATTRIBUTE);

            if (factAttr != null)
            {
                // set [FactAttribute(Skip="reason")]
                factAttr.Arguments.Add
                    (
                        new CodeAttributeArgument(FACT_ATTRIBUTE_SKIP_PROPERTY_NAME, new CodePrimitiveExpression(SKIP_REASON))
                    );
            }

            var theoryAttr = testMethod.CustomAttributes.OfType<CodeAttributeDeclaration>()
                .FirstOrDefault(codeAttributeDeclaration => codeAttributeDeclaration.Name == THEORY_ATTRIBUTE);

            if (theoryAttr != null)
            {
                // set [TheoryAttribute(Skip="reason")]
                theoryAttr.Arguments.Add
                    (
                        new CodeAttributeArgument(THEORY_ATTRIBUTE_SKIP_PROPERTY_NAME, new CodePrimitiveExpression(SKIP_REASON))
                    );
            }
        }

        public virtual void SetRowTest(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string scenarioTitle)
        {
            CodeDomHelper.AddAttribute(testMethod, THEORY_ATTRIBUTE);

            SetProperty(testMethod, FEATURE_TITLE_PROPERTY_NAME, generationContext.Feature.Name);
            SetDescription(testMethod, scenarioTitle);
        }

        public virtual void SetTestMethodAsRow(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string scenarioTitle, string exampleSetName, string variantName, IEnumerable<KeyValuePair<string, string>> arguments)
        {
        }

        protected void SetProperty(CodeTypeMember codeTypeMember, string name, string value)
        {
            CodeDomHelper.AddAttribute(codeTypeMember, TRAIT_ATTRIBUTE, name, value);
        }

        protected void SetDescription(CodeTypeMember codeTypeMember, string description)
        {
            // xUnit doesn't have a DescriptionAttribute so using a TraitAttribute instead
            SetProperty(codeTypeMember, DESCRIPTION_PROPERTY_NAME, description);
        }
        
        protected virtual CodeTypeReference CreateFixtureInterface(CodeTypeReference fixtureDataType)
        {
            return new CodeTypeReference(IUSEFIXTURE_INTERFACE, fixtureDataType);
        }
    }
}