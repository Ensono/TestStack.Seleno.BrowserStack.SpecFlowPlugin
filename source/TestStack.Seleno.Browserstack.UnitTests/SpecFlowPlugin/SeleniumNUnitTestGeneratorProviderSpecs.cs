using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using FluentAssertions;
using Gherkin.Ast;
using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.UnitTestProvider;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Utils;
using TestStack.Seleno.BrowserStack.SpecFlowPlugin;

namespace TestStack.Seleno.Browserstack.UnitTests.SpecFlowPlugin
{
    [TestFixture]
    public class SeleniumNUnitTestGeneratorProviderSpecs
    {
        private SeleniumNUnitTestGeneratorProvider _sut;
        private CodeDomHelper _codeDomHelper;
        private IUnitTestGeneratorProvider _unitTestGeneratorProvider;

        [SetUp]
        public void SetUp()
        {
            _unitTestGeneratorProvider = Substitute.For<IUnitTestGeneratorProvider>();
            _codeDomHelper = new CodeDomHelper(CodeDomProviderLanguage.CSharp);
            _sut = Substitute.ForPartsOf<SeleniumNUnitTestGeneratorProvider>(_codeDomHelper);
        }

        [Test]
        public void SetTestMethod_ShouldCallSetTestMethodBaseAndInitialiseAndRegisterBrowserHostWithinScenarioInitializeMethod()
        {
            // Arrrange
            var generationContext = CreateTestClassGenerationContext();
            var testMethod = new CodeMemberMethod();
            const string myFriendTestName = "My friend test name";
            var scenarioInitializedMethodStatements =
                generationContext
                    .ScenarioInitializeMethod
                    .Statements
                    .OfType<CodeSnippetStatement>();
            var testMethodStatements = testMethod.Statements.OfType<CodeSnippetStatement>();
            var testMethodAttributes = testMethod.CustomAttributes.OfType<CodeAttributeDeclaration>();


            // Act
            _sut.SetTestMethod(generationContext, testMethod, myFriendTestName);

            // Assert
            scenarioInitializedMethodStatements
                .Should()
                .OnlyContain(
                    x => x.Value == "            InitialiseAndRegisterBrowserHost(_currentBrowserConfiguration);");

            testMethodStatements.Should().OnlyContain(x => x.Value == "            _currentBrowserConfiguration = null;");
            testMethodAttributes.Should().Contain(x => x.Name == "NUnit.Framework.TestAttribute");
            testMethodAttributes.Should().Contain(DescriptionAttributeWith(myFriendTestName));
        }

        [Test]
        public void SetTestClassInitializeMethod_Should()
        {
            // Arrange
            var generationContext = CreateTestClassGenerationContext();
            var testClassInitializeMethodAttributes =
                generationContext.TestClassInitializeMethod.CustomAttributes.OfType<CodeAttributeDeclaration>();
            var testClassInitilizeMethodStatements =
                generationContext.TestClassInitializeMethod.Statements.OfType<CodeSnippetStatement>();

            // Act
            _sut.SetTestClassInitializeMethod(generationContext);

            // Assert
            testClassInitializeMethodAttributes.Should().OnlyContain(x => x.Name == "NUnit.Framework.TestFixtureSetUpAttribute");
            testClassInitilizeMethodStatements.Should()
                .OnlyContain(x => x.Value == @"             var configurationProvider = new ConfigurationProvider();
            _remoteBrowserConfigurator = new RemoteBrowserConfigurator(new BrowserHostFactory(configurationProvider),
                new BrowserConfigurationParser(new BrowserStackService(configurationProvider,
                    new HttpClientFactory(configurationProvider))),
                new CapabilitiesBuilder(configurationProvider));");
        }

        [Test]
        public void FinalizeTestClass_ShouldDisposeBrowserHost()
        {
            // Arrange
            var generationContext = CreateTestClassGenerationContext();

            // Act
            _sut.FinalizeTestClass(generationContext);

            // Assert
            generationContext.TestCleanupMethod.Statements.OfType<CodeSnippetStatement>()
                .Should()
                .OnlyContain(x => x.Value == @"            if (_host != null)
            {
                _host.Dispose();
                _host = null;
            }");

        }

    
        [Test]
        public void SetTestMethodCategories_ShouldNotAddTestCaseAttributesWhenNoneOfScenarioCategoriesStartsWithBrowser()
        {
            // Arrange
            var testMethod = new CodeMemberMethod();
            var generationContext = CreateTestClassGenerationContext();
            var scenarioCategories = new[] { "My first category", "chrome","some other category" };
            var testMethodParameters = testMethod.Parameters.OfType<CodeParameterDeclarationExpression>();
            var testMethodStatements = testMethod.Statements.OfType<CodeSnippetStatement>();

             // Act
            _sut.SetTestMethodCategories(generationContext, testMethod, scenarioCategories);

            // Assert
            _sut.DidNotReceive().AddTestCaseAttributeForEachBrowser(Arg.Any<CodeMemberMethod>(), Arg.Any<string>());
            testMethodParameters.Should().BeEmpty();
            testMethodStatements.Should().BeEmpty();
        }

        public void SetTestMethodCategories_ShouldNotAddTestCaseAttibutesWhenNoneOfScenarioCategoriesStartsWithBrowser()
        {
            // Arrange
            var testMethod = new CodeMemberMethod();
            var generationContext = CreateTestClassGenerationContext();
            var scenarioCategories = new[] { "My first category", "chrome", "some other category" };
            var testMethodParameters = testMethod.Parameters.OfType<CodeParameterDeclarationExpression>();
            var testMethodStatements = testMethod.Statements.OfType<CodeSnippetStatement>();

            // Act
            _sut.SetTestMethodCategories(generationContext, testMethod, scenarioCategories);

            // Assert
            _sut.DidNotReceive().AddTestCaseAttributeForEachBrowser(Arg.Any<CodeMemberMethod>(), Arg.Any<string>());
            testMethodParameters.Should().BeEmpty();
            testMethodStatements.Should().BeEmpty();
        }

        [Test]
        public void SetTestMethodCategories_ShouldCreateCategoryAttributeForEachScenarioCategoryThatDoesnStartWithBrowser()
        {
            // Arrange
            var testMethod = new CodeMemberMethod();
            var generationContext = CreateTestClassGenerationContext();
            var scenarioCategories = new[] { "My first category", "browser:chrome", "some other category","browser:iPhone,iPhone 6S Plus" };
            var scenarioCategoriesNotStartingWithBrowser =
                scenarioCategories.Where(c => !c.StartsWith("browser", StringComparison.InvariantCultureIgnoreCase));
            var testMethodAttributes = testMethod.CustomAttributes.OfType<CodeAttributeDeclaration>();

            // Act
            _sut.SetTestMethodCategories(generationContext, testMethod, scenarioCategories);

            // Assert
            foreach (var scenarioCategory in scenarioCategoriesNotStartingWithBrowser)
            {
                testMethodAttributes.Should().Contain(CategoryAttributeWith(scenarioCategory));
            }
        }


        [TestCase(true)]
        [TestCase(false)]
        public void SetTestMethodCategories_ShouldInsertBrowserConfigurationParameterToTestMethodAndInitialiseCurrentBrowserConfigurationField(bool initialiseCurrentBrowserConfigurationWithNull)
        {
            // Arrange
            var testMethod = new CodeMemberMethod();
            var generationContext = CreateTestClassGenerationContext();
            var scenarioCategories = new[] { "My first category", "browser:chrome", "some other category" };
            var testMethodAttributes = testMethod.CustomAttributes.OfType<CodeAttributeDeclaration>();
            var testMethodParameters = testMethod.Parameters.OfType<CodeParameterDeclarationExpression>();

            if (initialiseCurrentBrowserConfigurationWithNull)
            {
                testMethod.Statements.Add(new CodeSnippetStatement("         _currentBrowserConfiguration = null;"));
            }
            var testMethodStatements = testMethod.Statements.OfType<CodeSnippetStatement>();

            // Act
            _sut.SetTestMethodCategories(generationContext, testMethod, scenarioCategories);

            // Assert
            testMethodParameters.Should().OnlyContain(ParameterWith("System.string", "browserConfiguration"));
            testMethodStatements.Should().OnlyContain(CodeSnippet("_currentBrowserConfiguration = browserConfiguration;"));
        }

        [Test]
        public void AddTestCaseAttributeForEachBrowser_ShouldAddBrowserInTestMethodUserDataDictionary()
        {

            // Arrange
            var testMethod = new CodeMemberMethod();
            const string browser = "chrome";

            // Act
            _sut.AddTestCaseAttributeForEachBrowser(testMethod, browser);

            //Assert
            testMethod.UserData.Keys.Should().Contain("browser:chrome");
            testMethod.UserData.Values.Should().Contain("chrome");
        }


        [Test]
        public void AddTestCaseAttributeForEachBrowser_ShouldNotAddBrowserInTestMethodUserDataDictionaryWhenKeyExists()
        {

            // Arrange
            var testMethod = new CodeMemberMethod();
            testMethod.UserData.Add("browser:chrome","chromium");
            const string browser = "chrome";

            // Act
            _sut.AddTestCaseAttributeForEachBrowser(testMethod, browser);

            //Assert
            testMethod.UserData.Keys.Should().Contain("browser:chrome");
            testMethod.UserData.Values.Should().Contain("chromium");
        }


        [Ignore("In Progress")]
        public void AddTestCaseAttributeForEachBrowser_ShoulAddTestCaseAttributeForBrowser()
        {

            // Arrange
            var testMethod = new CodeMemberMethod();
            const string browser = "chrome,48.0,Windows,10";
            const string myFriendlyTestName = "This is my friendly test method";


            testMethod.CustomAttributes.Add(new CodeAttributeDeclaration("NUnit.Framework.DescriptionAttribute",
                new CodeAttributeArgument(new CodePrimitiveExpression(myFriendlyTestName))));
            var testMethodAttributes = testMethod.CustomAttributes.OfType<CodeAttributeDeclaration>();
            var arguments = new Dictionary<string,string>()
            {
                {"", browser},
                {"Category",  "chrome 48.0 Windows 10"},
                {"TestCase",  myFriendlyTestName + " on chrome 48.0 Windows 10"}
            };


            // Act
            _sut.AddTestCaseAttributeForEachBrowser(testMethod, browser);

            //Assert
            testMethodAttributes.Should().Contain(TestCaseAttributeWith(arguments));
        }

        private Expression<Func<CodeAttributeDeclaration, bool>> TestCaseAttributeWith(IEnumerable<KeyValuePair<string,string>> arguments)
        {
            return
                x =>
                    x.Name == "NUnit.Framework.TestCaseAttribute" && x.Arguments.Count == 3;

        }

        #region private helper methods

        private static Expression<Func<CodeSnippetStatement, bool>> CodeSnippet(string value)
        {
            return x => x.Value.Trim() == value.Trim();
        }

        private static Expression<Func<CodeParameterDeclarationExpression, bool>> ParameterWith(string type, string name)
        {
            return p => p.Name == name && p.Type.BaseType == type;
        }

        private static Expression<Func<CodeAttributeDeclaration, bool>> DescriptionAttributeWith(string myFriendTestName)
        {
            return x =>
                x.Name == "NUnit.Framework.DescriptionAttribute" &&
                x.Arguments.Count == 1 & x.Arguments[0].Value is CodePrimitiveExpression &&
                ((CodePrimitiveExpression) x.Arguments[0].Value).Value.Equals(myFriendTestName);
        }

        private static Expression<Func<CodeAttributeDeclaration, bool>> CategoryAttributeWith(string scenarioCategory)
        {
            return x => x.Name == "NUnit.Framework.CategoryAttribute" && x.Arguments.Count == 1 &&
                        ((CodePrimitiveExpression)x.Arguments[0].Value).Value.Equals(scenarioCategory);
        }



        private TestClassGenerationContext CreateTestClassGenerationContext()
        {
            return new TestClassGenerationContext(_unitTestGeneratorProvider,
                new SpecFlowFeature(new[] {new Tag(new Location(), "something")}, new Location(), string.Empty,
                    string.Empty, string.Empty, string.Empty,
                    new Background(new Location(), string.Empty, string.Empty, string.Empty, new Step[0]),
                    new ScenarioDefinition[0], new Comment[0], string.Empty), new CodeNamespace(),
                new CodeTypeDeclaration(), new CodeMemberField(),
                new CodeMemberMethod(), new CodeMemberMethod(), new CodeMemberMethod(), new CodeMemberMethod(), new CodeMemberMethod(),
                new CodeMemberMethod(), new CodeMemberMethod(), false);
        }

        #endregion
    }
}