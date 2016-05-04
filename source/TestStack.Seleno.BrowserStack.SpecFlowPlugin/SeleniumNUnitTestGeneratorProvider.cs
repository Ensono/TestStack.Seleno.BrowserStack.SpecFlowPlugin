using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Utils;
using TestStack.Seleno.BrowserStack.Core.Extensions;

namespace TestStack.Seleno.BrowserStack.SpecFlowPlugin
{
    public class SeleniumNUnitTestGeneratorProvider : BaseNunitTestGeneratorProvider
    {
        private bool _scenarioSetupMethodsAdded = false;

        public SeleniumNUnitTestGeneratorProvider(CodeDomHelper codeDomHelper) : base(codeDomHelper)
        { }

        public override void SetTestMethod(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string friendlyTestName)
        {
            base.SetTestMethod(generationContext, testMethod, friendlyTestName);

            if (!_scenarioSetupMethodsAdded)
            {
                generationContext.ScenarioInitializeMethod.Statements.Add(
                    new CodeSnippetStatement(
                        @"            InitialiseAndRegisterBrowserHost(_currentBrowserConfiguration);"));
                _scenarioSetupMethodsAdded = true;
            }
            testMethod.Statements.Insert(0, new CodeSnippetStatement("            _currentBrowserConfiguration = null;"));
        }

        public override void SetTestMethodCategories(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, IEnumerable<string> scenarioCategories)
        {
            var categories = scenarioCategories as string[] ?? scenarioCategories.ToArray();

            CodeDomHelper.AddAttributeForEachValue(testMethod, CATEGORY_ATTR, categories.WithoutBrowserTag());

            var hasBrowserTag = false;

            foreach (var browser in categories.WithBrowserTag().Select(cat => cat.Replace(CategorySelection.BrowserTagName, string.Empty)))
            {
                AddTestCaseAttributeForEachBrowser(testMethod, browser);

                hasBrowserTag = true;
            }

            if (hasBrowserTag)
            {
                testMethod.Parameters.Insert(0,
                    new CodeParameterDeclarationExpression("System.string", "browserConfiguration"));

                var statement = testMethod.Statements.OfType<CodeSnippetStatement>().FirstOrDefault(c => c.Value.Contains("_currentBrowserConfiguration"));

                if (statement != null)
                {
                    testMethod.Statements.Remove(statement);
                }

                testMethod.Statements.Insert(0,
                    new CodeSnippetStatement("            _currentBrowserConfiguration = browserConfiguration;"));
            }
        }

        public override void SetRow(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, IEnumerable<string> arguments,
            IEnumerable<string> tags, bool isIgnored)
        {
            var attributeArguments = CreateRowAttributeArgumentsFrom(arguments, tags, isIgnored);

            var browsers = testMethod.UserData.Keys.OfType<string>()
                .WithBrowserTag()
                .Select(key => (string) testMethod.UserData[key]).ToArray();

            if (browsers.Any())
            {
                foreach (
                    var codeAttributeDeclaration in
                        testMethod.CustomAttributes.Cast<CodeAttributeDeclaration>()
                            .Where(attr => attr.Name == ROW_ATTR && attr.Arguments.Count == 3)
                            .ToList())
                {
                    testMethod.CustomAttributes.Remove(codeAttributeDeclaration);
                }

                var argsString = string.Join(" ,",attributeArguments.Take(attributeArguments.Count - 1).Select(arg =>
                       $"\"{((CodePrimitiveExpression)arg.Value).Value}\""));

                foreach (var browser in browsers)
                {

                    AddTestCaseAttributeForEachBrowser(
                        testMethod, 
                        browser,
                        argsString,
                        attributeArguments);
                }
            }
            else
            {
                CodeDomHelper.AddAttribute(testMethod, ROW_ATTR, attributeArguments.ToArray());
            }
        }
        
        public virtual IList<CodeAttributeArgument> CreateRowAttributeArgumentsFrom(IEnumerable<string> arguments, IEnumerable<string> tags, bool isIgnored)
        {
            var args = arguments.Select(
                arg => new CodeAttributeArgument(new CodePrimitiveExpression(arg))).ToList();

            // addressing ReSharper bug: TestCase attribute with empty string[] param causes inconclusive result - https://github.com/techtalk/SpecFlow/issues/116
            var exampleTagExpressionList = tags.Select(t => new CodePrimitiveExpression(t)).Cast<CodeExpression>().ToArray();
            var exampleTagsExpression = exampleTagExpressionList.Length == 0
                ? (CodeExpression) new CodePrimitiveExpression(null)
                : new CodeArrayCreateExpression(typeof (string[]), exampleTagExpressionList);
            args.Add(new CodeAttributeArgument(exampleTagsExpression));

            if (isIgnored)
                args.Add(new CodeAttributeArgument("Ignored", new CodePrimitiveExpression("Ignored scenario")));
            return args;
        }

        public override void SetTestClass(TestClassGenerationContext generationContext, string featureTitle, string featureDescription)
        {
            base.SetTestClass(generationContext, featureTitle, featureDescription);

            AddTestAutomationCoreAndSeleniumRemoteNamespaces(generationContext);

            AddPrivateMembers(generationContext);

            AddInitialiseAndRegisterBrowserHostMethod(generationContext);
        }

        public override void FinalizeTestClass(TestClassGenerationContext generationContext)
        {
            generationContext.TestCleanupMethod.Statements.Add(new CodeSnippetStatement(@"            if (_host != null)
            {
                _host.Dispose();
                _host = null;
            }"));
        }

        #region private methods

        private void AddPrivateMembers(TestClassGenerationContext generationContext)
        {
            generationContext.TestClass.Members.Add(new CodeMemberField("IBrowserHost", "_host"));
            generationContext.TestClass.Members.Add(new CodeMemberField("System.String", "_currentBrowserConfiguration"));
            
        }

        private static void AddInitialiseAndRegisterBrowserHostMethod(TestClassGenerationContext generationContext)
        {
            var initializeSelenoMethod = new CodeMemberMethod {Name = "InitialiseAndRegisterBrowserHost" };
            initializeSelenoMethod.Parameters.Add(new CodeParameterDeclarationExpression("System.String", "browserConfiguration = null"));
            initializeSelenoMethod.Statements.Add(new CodeSnippetStatement(@"             ScenarioContext.Current.ScenarioContainer.RegisterTypeAs<ConfigurationProvider, IConfigurationProvider>();
            ScenarioContext.Current.ScenarioContainer.RegisterTypeAs<BrowserStackService, IBrowserStackService>(); 
            ScenarioContext.Current.ScenarioContainer.RegisterTypeAs<HttpClientFactory, IHttpClientFactory>(); 

            var configurationProvider = new ConfigurationProvider();
            var remoteBrowserConfigurator =
                new RemoteBrowserConfigurator(
                    new BrowserHostFactory(configurationProvider, ScenarioContext.Current.ScenarioContainer),
                    new BrowserConfigurationParser(new BrowserStackService(configurationProvider,
                        new HttpClientFactory(configurationProvider))),
                    new CapabilitiesBuilder(configurationProvider));


            var scenarioTitle = ScenarioContext.Current.ScenarioInfo.Title;
            var featureTitle = FeatureContext.Current.FeatureInfo.Title;

            var testSpecification = new TestSpecification(scenarioTitle, featureTitle);
            _host = remoteBrowserConfigurator.CreateAndConfigure(testSpecification, browserConfiguration);
           
            ScenarioContext.Current.ScenarioContainer.RegisterInstanceAs(_host);"));

            generationContext.TestClass.Members.Add(initializeSelenoMethod);
        }

        private static void AddTestAutomationCoreAndSeleniumRemoteNamespaces(TestClassGenerationContext generationContext)
        {
            generationContext.Namespace.Imports.Add(new CodeNamespaceImport("TestStack.Seleno.BrowserStack.Core.Configuration"));
            generationContext.Namespace.Imports.Add(new CodeNamespaceImport("TestStack.Seleno.BrowserStack.Core.Services.TestSession"));
            generationContext.Namespace.Imports.Add(new CodeNamespaceImport("TestStack.Seleno.BrowserStack.Core.Capabilities"));
            generationContext.Namespace.Imports.Add(new CodeNamespaceImport("TestStack.Seleno.BrowserStack.Core.Services.Client"));
        }
        
        public virtual void AddTestCaseAttributeForEachBrowser(CodeMemberMethod testMethod, string browser,
            string rowDataAsString = null,
            IEnumerable<CodeAttributeArgument> attributeArguments = null)
        {
            var keyName = $"{CategorySelection.BrowserTagName}{browser}";
            if (!testMethod.UserData.Contains(keyName))
            {
                testMethod.UserData.Add(keyName, browser);
            }

            var browserSpecifications = browser.UserFriendlyBrowserConfiguration();
            var testName = $"{testMethod.GetDescription()} on {browserSpecifications}";
            if (!string.IsNullOrWhiteSpace(rowDataAsString))
            {
                testName += $" with: {rowDataAsString}";
            }

            var withBrowserArgs = new[] {new CodeAttributeArgument(new CodePrimitiveExpression(browser))}
                .Concat(attributeArguments ?? Enumerable.Empty<CodeAttributeArgument>())
                .Concat(new[]
                {
                    new CodeAttributeArgument("Category", new CodePrimitiveExpression(browserSpecifications)),
                    new CodeAttributeArgument("TestName", new CodePrimitiveExpression(testName))
                })
                .ToArray();

            CodeDomHelper.AddAttribute(testMethod, ROW_ATTR, withBrowserArgs);
        }

        #endregion
    }
}