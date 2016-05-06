# TestStack.Seleno.BrowserStack.SpecFlowPlugin

- Custom plugin to generate [NUnit](http://www.nunit.org/) test to support multiple browser configurations on [BrowserStack](https://www.browserstack.com) using [ Seleno](http://seleno.teststack.net/)

- Test class generator to drive automated web ui tests on browser stack with [ Seleno](http://seleno.teststack.net/) and [SpecFlow](http://www.specflow.org/).

- Configures [SpecFlow](http://www.specflow.org/) automatically using [SpecFlow](http://www.specflow.org/)'s [BoDi](https://github.com/gasparnagy/BoDi) Container to be able to get browser.

- Creating automated web tests to test an application in addition to testing the application with unit tests is a good practice. [SpecFlow](http://www.specflow.org/) supports behavior driven development and acceptance tests driven development.

- This project was created to be able to use [ Seleno](http://seleno.teststack.net/) with [SpecFlow](http://www.specflow.org/) as easily as possible, and at the same time to be able to use it in a Continous Integration Environment.

### Release notes:

1.1) Multiple browser support with just tag placed on scenario like `@browser:firefox,43.0,OS_X,Lion`

  - list of supported capabilities (combination of browser, version, operating system):
  https://www.browserstack.com/automate/capabilities


1.2) Error notification done with after scenario hook and browser stack API to pass error message.

1.3) configure plugin in App.config and set app settings for browser stack basic settings:
```xml
<configuration>
    <specFlow>
        <plugins>
      <add name="TestStack.Seleno.BrowserStack" path="..\packages\TestStack.Seleno.BrowserStack.SpecFlowPlugin.1.0.3.29167\lib\net451" type="GeneratorAndRuntime" />
    </plugins>
    <stepAssemblies>
      <stepAssembly assembly="TestStack.Seleno.BrowserStack.Core" />
    </stepAssemblies>
    </specFlow>
	<appSettings>
    <add key="browserStackRemoteDriverUrl" value="http://hub.browserstack.com/wd/hub/" />
    <add key="buildNumber" value="" />
    <add key="browserStackApiUrl" value="https://www.browserstack.com/automate/" />
    <!--
     <add key="browserStack.user" value="<yourUserNameHere>" />
     <add key="browserstack.key" value="<yourAccessKeyHere>" />
    -->
  </appSettings>
</configuration>
```
1.4) support resolution for desktop like `@browser:chrome,50.0,Windows,10,1024x768` or `@browser:chrome,1024x768`

1.5) Maximize browser window to use full available resolution specified into browser stack capabilities (default 1024x768)

### Features:


- Sets SpecFlow tests up to run and teardown a [BrowserStack](https://www.browserstack.com)
    - The Browser is a tool which doesn't belong to your application, why write code to instantiate it?

- Annotate scenarios with scenario supporting Browsers
    -  Does every scenario describing the acceptance tests for every browser? Mark the scenario with the supported Browser.

- Adds the browser name as a TestCategory
    - Just run the tests with the categories for the browser you actually have on the environment. Example: Don't run Android browser test as I don't have an android device attached to my machine.


Get it from Nuget.org:

https://www.nuget.org/packages/TestStack.Seleno.BrowserStack.SpecFlowPlugin/

***

### Using Only SpecFlow and Seleno alone for multiple browser testing

<br/>
If you want to perform multiple browser testing for a particular scenario,
the obvious and only way would be to parameterize the browser configuration like below:
That approach clutter the scenario with environment considerations..
<br/>
<br/>

**Scenario Outline**: Add Two Numbers
```Cucumber  
Given I the browser is <browser> <version> on <osName> <osVersion> for <resolution>
  And I navigated to "http://www.theonlinecalculator.com/"
  And I have entered 10 into the calculator
  And I have entered 20 into the calculator
 When I press add
 Then the result should be 30 on the screen

Examples:
| browser | version | osName  | osVersion  | resolution |
| chrome  | 50.0    | Windows | 10         | 1024x768   |
| Safari  | 9.0     | OS X    | El Capitan | 1280x1024  |
```

  **_Which will result on following calculator step class_**

```csharp
 [Binding]
 public class CalculatorSteps
 {
     private SelenoHost _browserHost;
     private readonly DesiredCapabilities _capabilities = new DesiredCapabilities();
     private CalculatorPage _calculatorPage;

     [Given(@"I the (.*) is (.*) on (.*) (.*) for (.*)")]
     public void GivenITheBrowserIs(string browser, string version, string osName,
                                   string osVersion, string resolution)
     {
         _capabilities.SetCapability(CapabilityType.BrowserName, browser);
         _capabilities.SetCapability(CapabilityType.Version, version);
         _capabilities.SetCapability("os", version);
         _capabilities.SetCapability("resolution", resolution);
         _capabilities.SetCapability("os_version", version);
         _capabilities.SetCapability("browserstack.user",
                                     ConfigurationManager.AppSettings["browserstack.user"]);
         _capabilities.SetCapability("browserstack.usekey",
                                     ConfigurationManager.AppSettings["browserstack.usekey"]);
         _browserHost = new SelenoHost();
     }

     [Given(@"I navigated to ""(.*)""")]
     public void GivenINavigatedTo(string url)
     {
         _browserHost.Run(config => config
                                     .WithRemoteWebDriver(BrowserStackRemoteDriver(url))
                                     .WithWebServer(new InternetWebServer(url)));
         _calculatorPage = _browserHost.NavigateToInitialPage<CalculatorPage>();
     }

     [Given(@"I have entered (.*) into the calculator")]
     public void GivenIHaveEnteredIntoTheCalculator(int number)
     {
         _calculatorPage.Enter(number);
     }

     [When(@"I press add")]
     public void WhenIPressAdd()
     {
         _calculatorPage.ClickOnAddButton();
     }

     [Then(@"the result should be (.*) on the screen")]
     public void ThenTheResultShouldBeOnTheScreen(int expectedResult)
     {
         _calculatorPage.Result.Should().Be(expectedResult);
     }

     private Func<RemoteWebDriver> BrowserStackRemoteDriver(string url)
     {
         return () => new RemoteWebDriver(new Uri(url), _capabilities);
     }

     [AfterScenario]
     public void CloseBrowserHost()
     {
        _browserHost.Dispose();
        _browserHost = null;
     }
 }

```

### Using TestStack.Seleno.BrowserStack.SpecFlowPlugin

**Scenario**: Add Two Numbers
```Cucumber  
@browser:chrome,50.0,Windows,10,1024x768
@browser:safari,9.0,OS_X,El_Capitan,1280x1024
@browser:IE,1024x768

Given I navigated to "http://www.theonlinecalculator.com/"
  And I have entered 10 into the calculator
  And I have entered 20 into the calculator
 When I press add
 Then the result should be 30 on the screen

```

**_Which will result on following calculator step class_**

```csharp
[Binding]
public class CalculatorSteps
{
   private IBrowserHost _browserHost;
   private CalculatorPage _calculatorPage;

   public CalculatorSteps(IBrowserHost browserHost)
   {
      _browserHost = browserHost;
   }

   [Given(@"I navigated to ""(.*)""")]
   public void GivenINavigatedTo(string url)
   {
       _calculatorPage = _browserHost.NavigateToInitialPage<CalculatorPage>(url);
   }

   [Given(@"I have entered (.*) into the calculator")]
   public void GivenIHaveEnteredIntoTheCalculator(int number)
   {
       _calculatorPage.Enter(number);
   }

   [When(@"I press add")]
   public void WhenIPressAdd()
   {
       _calculatorPage.ClickOnAddButton();
   }

   [Then(@"the result should be (.*) on the screen")]
   public void ThenTheResultShouldBeOnTheScreen(int expectedResult)
   {
       _calculatorPage.Result.Should().Be(expectedResult);
   }  
}
```

If the scenario fails for any reason a API call is made to Browser stack API to notify the test has failed with whichever reason it had failed tool.
Disposing the browser host and internal remote web driver will be done also automatically behind the scenes.
