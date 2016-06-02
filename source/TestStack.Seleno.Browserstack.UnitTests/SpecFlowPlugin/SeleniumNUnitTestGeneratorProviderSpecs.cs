using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using Gherkin.Ast;
using NSubstitute;
using NUnit.Framework;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.UnitTestProvider;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Utils;
using TestStack.Seleno.Browserstack.UnitTests.Assertions;
using TestStack.Seleno.BrowserStack.SpecFlowPlugin;
using static System.String;

namespace TestStack.Seleno.Browserstack.UnitTests.SpecFlowPlugin
{
    [TestFixture]
    public class SeleniumNUnitTestGeneratorProviderSpecs
    {
        private SeleniumXUnitTestGeneratorProvider _sut;
        private CodeDomHelper _codeDomHelper;
        private IUnitTestGeneratorProvider _unitTestGeneratorProvider;

        [SetUp]
        public void SetUp()
        {
            _unitTestGeneratorProvider = Substitute.For<IUnitTestGeneratorProvider>();
            _codeDomHelper = new CodeDomHelper(CodeDomProviderLanguage.CSharp);
            _sut = Substitute.ForPartsOf<SeleniumXUnitTestGeneratorProvider>(_codeDomHelper);
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
            var descritionAttributeArgument = new CodeAttributeArgument(new CodePrimitiveExpression(myFriendTestName));


            // Act
            _sut.SetTestMethod(generationContext, testMethod, myFriendTestName);

            // Assert
            scenarioInitializedMethodStatements
                .Should()
                .OnlyContain(CodeSnippet("InitialiseAndRegisterBrowserHost(_currentBrowserConfiguration);"));

            testMethodStatements.Should().OnlyContain(CodeSnippet("_currentBrowserConfiguration = null;"));
            testMethod.CustomAttributes.Should().OnlyContain("Xunit.FactAttribute");
            testMethod.CustomAttributes.Should().OnlyContain("Xunit.TraitAttribute", descritionAttributeArgument);
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
            testClassInitilizeMethodStatements
                .Should()
                .OnlyContain(CodeSnippet(@"var configurationProvider = new ConfigurationProvider();
            _remoteBrowserConfigurator = new RemoteBrowserConfigurator(new BrowserHostFactory(configurationProvider),
                new BrowserConfigurationParser(new BrowserStackService(configurationProvider,
                    new HttpClientFactory(configurationProvider))),
                new CapabilitiesBuilder(configurationProvider), 
                configurationProvider);"));
        }

        [Test]
        public void FinalizeTestClass_ShouldDisposeBrowserHost()
        {
            // Arrange
            var generationContext = CreateTestClassGenerationContext();
            var testCleanUpMethodStatements =
                generationContext.TestCleanupMethod.Statements.OfType<CodeSnippetStatement>();

            // Act
            _sut.FinalizeTestClass(generationContext);

            // Assert
            
            testCleanUpMethodStatements.Should().OnlyContain(CodeSnippet(@"if (_host != null)
            {
                _host.Dispose();
                _host = null;
            }"));

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
                scenarioCategories.Where(c => !c.StartsWith("browser", StringComparison.InvariantCultureIgnoreCase)).ToList();
            var browsers =
                scenarioCategories.Except(scenarioCategoriesNotStartingWithBrowser)
                    .Select(b => b.Replace("browser:", Empty))
                    .ToList();

            // Act
            _sut.SetTestMethodCategories(generationContext, testMethod, scenarioCategories);

            // Assert
            scenarioCategoriesNotStartingWithBrowser
                .ForEach(category => testMethod.CustomAttributes.Should().Contain("Xunit.TraitAttribute", SimpleAttributeArgument(category)));

            browsers.ForEach(browser => _sut.Received(1).AddTestCaseAttributeForEachBrowser(testMethod, browser, null, null));
        }

       

        [TestCase(true)]
        [TestCase(false)]
        public void SetTestMethodCategories_ShouldInsertBrowserConfigurationParameterToTestMethodAndInitialiseCurrentBrowserConfigurationField(bool initialiseCurrentBrowserConfigurationWithNull)
        {
            // Arrange
            var testMethod = new CodeMemberMethod();
            var generationContext = CreateTestClassGenerationContext();
            var scenarioCategories = new[] { "My first category", "browser:chrome", "some other category" };
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
            testMethod.UserData.Should().OnlyContain("browser:chrome", "chrome");
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
            testMethod.UserData.Should().OnlyContain("browser:chrome", "chromium");
        }


        [Test]
        public void AddTestCaseAttributeForEachBrowser_ShoulAddTestCaseAttributeForBrowserWithCategoryAndTestNameFromTestMethodDescription()
        {

            // Arrange
            var testMethod = new CodeMemberMethod();
            const string browser = "chrome,48.0,Windows,10";
            const string myFriendlyTestName = "This is my friendly test method";
            var browserFriendlySpec = browser.Replace(",", " ");
            testMethod.CustomAttributes.Add(new CodeAttributeDeclaration("Xunit.TraitAttribute",
                new CodeAttributeArgument(new CodePrimitiveExpression(myFriendlyTestName))));
            var arguments = new[]
            {
                new CodeAttributeArgument(new CodePrimitiveExpression(browser)),
                new CodeAttributeArgument("Category", new CodePrimitiveExpression(browserFriendlySpec)),
                new CodeAttributeArgument("TestName", new CodePrimitiveExpression(myFriendlyTestName + " on chrome 48.0 Windows 10")),
            };


            // Act
            _sut.AddTestCaseAttributeForEachBrowser(testMethod, browser);

            //Assert
            testMethod.CustomAttributes.Should().OnlyContain("Xunit.InlineDataAttribute", arguments);
        }

        [Test]
        public void AddTestCaseAttributeForEachBrowser_ShoulAddTestCaseAttributeForBrowserWithCategoryAndSpecifiedTestName()
        {
            // Arrange
            var testMethod = new CodeMemberMethod();
            const string browser = "chrome,48.0,Windows,10";
            const string myFriendlyTestName = "This is my friendly test method";
            var browserFriendlySpec = browser.Replace(",", " ");

            var expectedArguments = new[]
            {
                new CodeAttributeArgument(new CodePrimitiveExpression(browser)),
                new CodeAttributeArgument("Category", new CodePrimitiveExpression(browserFriendlySpec)),
                new CodeAttributeArgument("TestName", new CodePrimitiveExpression(myFriendlyTestName + " on " + browserFriendlySpec)),
            };
            testMethod.CustomAttributes.Add(new CodeAttributeDeclaration("Xunit.TraitAttribute",
                SimpleAttributeArgument(myFriendlyTestName)));
            
            // Act
            _sut.AddTestCaseAttributeForEachBrowser(testMethod, browser);

            //Assert
            testMethod.CustomAttributes.Should().OnlyContain("Xunit.InlineDataAttribute", expectedArguments);
        }

        [Test]
        public void AddTestCaseAttributeForEachBrowser_ShoulAddTestCaseAttributeForBrowserWithCategoryAndSpecifiedTestNameAndRowData()
        {
            // Arrange
            var testMethod = new CodeMemberMethod();
            const string browser = "chrome,48.0,Windows,10";
            const string myFriendlyTestName = "This is my friendly test method";
            const string rowData = "\\\"Some example row data\\\", \\\"another row data\\\", \\\"last row data\\\"";
            var browserFriendlySpec = browser.Replace(",", " ");

            var expectedArguments = new[]
            {
                new CodeAttributeArgument(new CodePrimitiveExpression(browser)),
                new CodeAttributeArgument("Category", new CodePrimitiveExpression(browserFriendlySpec)),
                new CodeAttributeArgument("TestName", new CodePrimitiveExpression(myFriendlyTestName + " on "+ browserFriendlySpec + " with: " + rowData)),
            };
            testMethod.CustomAttributes.Add(new CodeAttributeDeclaration("Xunit.TraitAttribute",
                SimpleAttributeArgument(myFriendlyTestName)));

            // Act
            _sut.AddTestCaseAttributeForEachBrowser(testMethod, browser, rowData);

            //Assert
            testMethod.CustomAttributes.Should().OnlyContain("Xunit.InlineDataAttribute", expectedArguments);
        }

        [Test]
        public void AddTestCaseAttributeForEachBrowser_ShoulAddTestCaseAttributeWithExistingArgumentsAndBrowserArgument()
        {
            // Arrange
            var testMethod = new CodeMemberMethod();
            const string browser = "chrome,48.0,Windows,10";
            const string myFriendlyTestName = "This is my friendly test method";
            var browserFriendlySpec = browser.Replace(",", " ");

            var additionalArguments = new[]
            {
                new CodeAttributeArgument(new CodePrimitiveExpression(browser)),
                new CodeAttributeArgument("Category", new CodePrimitiveExpression(browserFriendlySpec)),
                new CodeAttributeArgument("TestName", new CodePrimitiveExpression(myFriendlyTestName +" on chrome 48.0 Windows 10")),
            };
            var existingAttributeArguments = new []
            {
                new CodeAttributeArgument("Author",new CodePrimitiveExpression("FT")),
                new CodeAttributeArgument("Ignored",new CodePrimitiveExpression("Not quite ready")),
            };

            var expectedArguments =additionalArguments.Take(1).Concat(existingAttributeArguments).Concat(additionalArguments.Skip(1));
            testMethod.CustomAttributes.Add(new CodeAttributeDeclaration("Xunit.TraitAttribute",
                SimpleAttributeArgument(myFriendlyTestName)));

            // Act
            _sut.AddTestCaseAttributeForEachBrowser(testMethod, browser, string.Empty, existingAttributeArguments);

            //Assert
            testMethod.CustomAttributes.Should().OnlyContain("Xunit.InlineDataAttribute", expectedArguments);
        }

       
        [Test]
        public void SetRow_ShouldAddTestCaseAttributeWithAnArgumentForEachArgumentsWhenNoTagsSpecified()
        {
            // Arrange
            var generationContext = CreateTestClassGenerationContext();
            var testMethod = new CodeMemberMethod();
            var arguments = new [] {"London", "Bath","HereFord"};
            var tags = new string[0];
            var expectedArguments = arguments.Select(SimpleAttributeArgument).ToList();
            expectedArguments.Add(SimpleAttributeArgument(null));

            // Act
            _sut.SetRow(generationContext, testMethod, arguments, tags, false);

            // Assert
            testMethod.CustomAttributes.Should().OnlyContain("Xunit.InlineDataAttribute", expectedArguments);
        }


        [Test]
        public void SetRow_ShouldAddTestCaseAttributeWithAnArgumentForEachForEachArgumentsAndTagsWhenBothSpecified()
        {
            // Arrange
            var generationContext = CreateTestClassGenerationContext();
            var testMethod = new CodeMemberMethod();
            var arguments = new[] { "London", "Bath", "HereFord" };
            var tags = new [] {"my tag","another tag"};

            var expectedArguments = arguments.Select(SimpleAttributeArgument).ToList();
            var tagExpressions = tags.Select(tag => (CodeExpression) new CodePrimitiveExpression(tag)).ToArray();
            expectedArguments.Add(new CodeAttributeArgument(new CodeArrayCreateExpression(typeof(string[]), tagExpressions)));

            // Act
            _sut.SetRow(generationContext, testMethod, arguments, tags, false);

            // Assert
            testMethod.CustomAttributes.Should().OnlyContain("Xunit.InlineDataAttribute", expectedArguments);
        }

        [Test]
        public void SetRow_ShouldAddTestCaseAttributeWithAnIgnoredArgumentWhenTestMethodIsIgnored()
        {
            // Arrange
            var generationContext = CreateTestClassGenerationContext();
            var testMethod = new CodeMemberMethod();
            var arguments = new string[0];
            var tags = new string[0];
            var expectedArguments = new[]
            {
                SimpleAttributeArgument(null),
                new CodeAttributeArgument("Ignored", new CodePrimitiveExpression("Ignored scenario"))
            };

            // Act
            _sut.SetRow(generationContext, testMethod, arguments, tags, true);

            // Assert
            testMethod.CustomAttributes.Should().OnlyContain("Xunit.InlineDataAttribute", expectedArguments);
        }

        [Test]
        public void SetRow_ShouldRemoveAllTestCaseAttributeWith3ArgumentsWhenUserDataHasBrowser()
        {
            // Arrange
            var generationContext = CreateTestClassGenerationContext();
            var testMethod = new CodeMemberMethod();
            var arguments = new string[0];
            var tags = new string[0];
            

            testMethod.UserData.Add("browser:chrome,48.0,Windows,10", "chrome,48.0,Windows,10");
            //testMethod.UserData.Add("browser:iPhone,iPhone_6S_Plus", "iPhone,iPhone_6S_Plus");
            var remainingTestCaseAttribute = new CodeAttributeDeclaration("Xunit.InlineDataAttribute", SimpleAttributeArgument("Something"));
            testMethod.CustomAttributes.AddRange(new[]
            {
                new CodeAttributeDeclaration("Xunit.InlineDataAttribute", Enumerable.Repeat(SimpleAttributeArgument(null), 3).ToArray()),
                remainingTestCaseAttribute,
                new CodeAttributeDeclaration("Xunit.InlineDataAttribute", Enumerable.Repeat(SimpleAttributeArgument("data"), 3).ToArray()),
            });

            // Act
            _sut.SetRow(generationContext, testMethod, arguments, tags, false);

            // Assert
            testMethod.CustomAttributes.Should().OnlyContain(remainingTestCaseAttribute);
        }


        [Test]
        public void SetRow_ShouldAddTestCaseAttributeForEachBrowser()
        {
            // Arrange
            var generationContext = CreateTestClassGenerationContext();
            var testMethod = new CodeMemberMethod();
            var arguments = new string[0];
            var tags = new string[0];
            var browsers = new List<string> {"browser:chrome,48.0,Windows,10", "browser:firefox", "browser:iPhone,iPhone_6S_Plus"};
            var attributeArguments = new List<CodeAttributeArgument> { SimpleAttributeArgument("First"), SimpleAttributeArgument("Second"), SimpleAttributeArgument(null) };
            const string rowDataAsString = "\"First\" ,\"Second\"";
            browsers.ForEach(browser => testMethod.UserData.Add(browser, Sanitize(browser)));

            _sut.WhenForAnyArgs(x => x.AddTestCaseAttributeForEachBrowser(null, null, null, null)).DoNotCallBase();
            _sut.CreateRowAttributeArgumentsFrom(arguments, tags, false).Returns(attributeArguments);

            // Act
            _sut.SetRow(generationContext, testMethod, arguments, tags, false);

            // Assert
            browsers
                .ForEach(browser => _sut.Received(1).AddTestCaseAttributeForEachBrowser(testMethod, Sanitize(browser), rowDataAsString, attributeArguments));
        }

        public string Sanitize(string value)
        {
            return value.Replace("browser:", Empty);
        }

        //[Test]
        //public void SetTestClass_ShouldAddTestFixtureAndDescriptionAttributesWithFeatureTitleToTestClass()
        //{
        //    // Arrange
        //    var generationContext = CreateTestClassGenerationContext();
        //    const string featureTitle = "Awesome feature title!";
        //    const string featureDescription = "It is doing something amazing!";
        //
        //    // Act
        //    _sut.SetTestClass(generationContext, featureTitle, featureDescription);
        //
        //    // Assert
        //   generationContext.TestClass.CustomAttributes
        //        .Should()
        //        .OnlyContain("Xunit.TraitAttribute", SimpleAttributeArgument(featureTitle));
        //}

        [Test]
        public void SetTestClass_ShouldAddRequiredNameSpaces()
        {
            // Arrange
            var generationContext = CreateTestClassGenerationContext();
            const string featureTitle = "Awesome feature title!";
            const string featureDescription = "It is doing something amazing!";

            // Act
            _sut.SetTestClass(generationContext, featureTitle, featureDescription);

            // Assert
            generationContext
                .Should()
                .HaveNumberOfImports(4)
                .And
                .ContainImportedNamespace("TestStack.Seleno.BrowserStack.Core.Configuration")
                .And
                .ContainImportedNamespace("TestStack.Seleno.BrowserStack.Core.Services.TestSession")
                .And
                .ContainImportedNamespace("TestStack.Seleno.BrowserStack.Core.Capabilities")
                .And
                .ContainImportedNamespace("TestStack.Seleno.BrowserStack.Core.Services.Client");

        }

        [Test]
        public void SetTestClass_ShouldAddPrivateMembers()
        {
            // Arrange
            var generationContext = CreateTestClassGenerationContext();
            const string featureTitle = "Awesome feature title!";
            const string featureDescription = "It is doing something amazing!";

            // Act
            _sut.SetTestClass(generationContext, featureTitle, featureDescription);

            // Assert
            generationContext.TestClass
                .Should()
                .HaveNumberOfFieldMembers(3)
                .And
                .ContainMemberField("IBrowserHost", "_host")
                .And
                .ContainMemberField("System.String", "_currentBrowserConfiguration")
                .And
                .ContainMemberField("RemoteBrowserConfigurator", "_remoteBrowserConfigurator");
        }

        [Test]
        public void SetTestClass_ShouldAddInitializeSelenoMethodToTestClass()
        {
            // Arrange
            var generationContext = CreateTestClassGenerationContext();
            const string featureTitle = "Awesome feature title!";
            const string featureDescription = "It is doing something amazing!";
            var initialiseAndRegisterBrowserHostMethod = new CodeMemberMethod
            {
                Name = "InitialiseAndRegisterBrowserHost",
                Parameters = { new CodeParameterDeclarationExpression("System.String", "browserConfiguration = null") },
                Statements =
                {
                    new CodeSnippetStatement(
                        @"             ScenarioContext.Current.ScenarioContainer.RegisterTypeAs<ConfigurationProvider, IConfigurationProvider>();
            ScenarioContext.Current.ScenarioContainer.RegisterTypeAs<BrowserStackService, IBrowserStackService>(); 
            ScenarioContext.Current.ScenarioContainer.RegisterTypeAs<HttpClientFactory, IHttpClientFactory>(); 

            var scenarioTitle = ScenarioContext.Current.ScenarioInfo.Title;
            var featureTitle = FeatureContext.Current.FeatureInfo.Title;

            var testSpecification = new TestSpecification(scenarioTitle, featureTitle);
            _host = _remoteBrowserConfigurator.CreateAndConfigure(testSpecification, browserConfiguration);
           
            ScenarioContext.Current.ScenarioContainer.RegisterInstanceAs(_host);")
                }
            };

            // Act
            _sut.SetTestClass(generationContext, featureTitle, featureDescription);

            // Assert
            generationContext.TestClass.Should().OnlyContainMemberMethod(initialiseAndRegisterBrowserHostMethod);
        }



        #region private helper methods


        private static CodeAttributeArgument SimpleAttributeArgument(string ctg)
        {
            return new CodeAttributeArgument(new CodePrimitiveExpression(ctg));
        }

        private static Expression<Func<CodeSnippetStatement, bool>> CodeSnippet(string value)
        {
            return x => x.Value.Trim() == value.Trim();
        }

        private static Expression<Func<CodeParameterDeclarationExpression, bool>> ParameterWith(string type, string name)
        {
            return p => p.Name == name && p.Type.BaseType == type;
        }

        private TestClassGenerationContext CreateTestClassGenerationContext()
        {
            return new TestClassGenerationContext(_unitTestGeneratorProvider,
                new SpecFlowFeature(new[] { new Tag(new Location(), "something") }, new Location(), Empty,
                    Empty, Empty, Empty,
                    new Background(new Location(), Empty, Empty, Empty, new Step[0]),
                    new ScenarioDefinition[0], new Comment[0], Empty), new CodeNamespace(),
                new CodeTypeDeclaration(), new CodeMemberField(),
                new CodeMemberMethod(), new CodeMemberMethod(), new CodeMemberMethod(), new CodeMemberMethod(), new CodeMemberMethod(),
                new CodeMemberMethod(), new CodeMemberMethod(), false);
        }

        #endregion
    }
}